using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.PHP;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ReferenceFactory
    {
        public const string MESSAGEBOX_VARIABLE_CONST = TES5TypeFactory.TES4Prefix + "_MESSAGEBOX_RESULT";
        public static readonly Regex ReferenceAndPropertyNameRegex = new Regex(@"([0-9A-Za-z]+)\.([0-9A-Za-z]+)", RegexOptions.Compiled);
        private const string tContainerName = "tContainer";
        private const string cyrodiilCrimeFactionName = "CyrodiilCrimeFaction";
        private const string TES4Attr = TES5TypeFactory.TES4Prefix + "Attr";
        public const string qf_Prefix = "qf_";
        public const string tif_Prefix = "tif_";
        //Those are used to hook in the internal Skyblivion systems.
        private readonly static Dictionary<string, ITES5Type> specialConversions = new Dictionary<string, ITES5Type>()
        {
            { TES4Attr + "Strength",  TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Intelligence", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Willpower", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Agility", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Speed", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Endurance", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Personality", TES5BasicType.T_GLOBALVARIABLE },
            { TES4Attr + "Luck", TES5BasicType.T_GLOBALVARIABLE },
            { tContainerName, TES5BasicType.T_SKYBCONTAINER },//Data container
            { cyrodiilCrimeFactionName, TES5BasicType.T_FACTION },//global cyrodiil faction
            { MESSAGEBOX_VARIABLE_CONST, TES5BasicType.T_INT },//set by script instead of original messageBox
            { TES5PlayerReference.PlayerRefName, TES5PlayerReference.TES5TypeStatic }
        };
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly ESMAnalyzer esmAnalyzer;
        public TES5ReferenceFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer esmAnalyzer)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.esmAnalyzer = esmAnalyzer;
        }

        public static string GetTES4AttrPlusName(string name)
        {
            return TES4Attr + PHPFunction.UCWords(name);
        }

        public static TES5SelfReference CreateReferenceToSelf(TES5GlobalScope globalScope)
        {
            //todo perhaps move tes5scriptAsVariable to a new factory
            return new TES5SelfReference(new TES5ScriptAsVariable(globalScope.ScriptHeader));
        }

        public static TES5Reference CreateReferenceToVariableOrProperty(ITES5VariableOrProperty variable)
        {
            return new TES5Reference(variable);
        }

        public static TES5PlayerReference CreateReferenceToPlayer(TES5GlobalScope globalScope)
        {
            globalScope.AddPlayerRefPropertyIfNotExists();
            return new TES5PlayerReference();
        }

        /*
        * Extracts implicit reference from calls.
         * Returns a reference from calls like:
         * Enable
         * Disable
         * Activate
         * GetInFaction whatsoever
        */
        public ITES5Referencer ExtractImplicitReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            ITES5Type type = globalScope.ScriptHeader.ScriptType.NativeType;
            if (type == TES5BasicType.T_ACTIVEMAGICEFFECT)
            {
                TES5SelfReference self = CreateReferenceToSelf(globalScope);
                return this.objectCallFactory.CreateObjectCall(self, "GetTargetActor");
            }
            if (type == TES5BasicType.T_QUEST)
            {
                //todo - this should not be done like this
                //we should actually not try to extract the implicit reference on the non-reference oblivion functions like "stopQuest"
                //think of this line as a hacky way to just get code forward.
                return CreateReferenceToSelf(globalScope);
            }
            if (type == TES5BasicType.T_TOPICINFO)
            {
                //TIF Fragments
                return this.CreateReadReference("akSpeakerRef", globalScope, multipleScriptsScope, localScope);
            }
            //WTM:  Change:  I made this the new default result instead of the previous algorithm of exhaustively listing types and throwing an exception if no type matches.
            return CreateReferenceToSelf(globalScope);
        }

        /*
         * Create the "read reference."
         * Read reference is used ( as you might think ) in read contexts.
        */
        public ITES5Referencer CreateReadReference(string referenceName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope, TES4Comment? comment = null)
        {
            ITES5Referencer rawReference = this.CreateReference(referenceName, globalScope, multipleScriptsScope, localScope);
            if (rawReference.TES5Type == TES5BasicType.T_GLOBALVARIABLE)
            {
                //Changed to int implementation.
                return this.objectCallFactory.CreateObjectCall(rawReference, "GetValue", comment: comment);
            }
            else
            {
                return rawReference;
            }
        }

        public ITES5Referencer CreateContainerReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return CreateReadReference(tContainerName, globalScope, multipleScriptsScope, localScope);
        }

        public ITES5Referencer CreateCyrodiilCrimeFactionReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return CreateReadReference(cyrodiilCrimeFactionName, globalScope, multipleScriptsScope, localScope);
        }

        private ITES5Type GetPropertyTypeAndFormID(string referenceName, string? tes4ReferenceNameForType, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, out Nullable<int> tes4FormID)
        {
            tes4FormID = null;
            ITES5Type? specialConversion;
            if (specialConversions.TryGetValue(referenceName, out specialConversion))
            {
                return specialConversion;
            }
            else if (multipleScriptsScope.ContainsGlobalVariable(referenceName))
            {
                return TES5BasicType.T_GLOBALVARIABLE;
            }
            else
            {
                //propertyType = TES5BasicType.T_FORM;
                //WTM:  Change:  I commented the above and added the below:
                if (!referenceName.StartsWith(MessageBoxFactory.MessageBoxPrefix) && referenceName != GetPlayerInSEWorldFactory.SEWorldLocationPropertyName)
                {
                    var scroData = GetTypeFromSCRO(esmAnalyzer, globalScope.ScriptHeader.EDID, referenceName);
                    if (scroData != null)
                    {
                        tes4FormID = scroData.Value.Key;
                        return scroData.Value.Value;
                    }
                }
                if (tes4ReferenceNameForType == null) { throw new NullableException(nameof(tes4ReferenceNameForType)); }
                ITES5Type? esmType = esmAnalyzer.GetTypeByEDIDWithFollow(tes4ReferenceNameForType, true);
                return esmType != null ? esmType : TES5BasicType.T_FORM;
            }
        }

        /*
        * Create a generic-purpose reference.
        */
        private ITES5Referencer CreateReference(string referenceName, TES5BasicType? typeForNewProperty, string? tes4ReferenceNameForType, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            if (TES5PlayerReference.EqualsPlayer(referenceName))
            {
                return CreateReferenceToPlayer(globalScope);
            }

            Match match = ReferenceAndPropertyNameRegex.Match(referenceName);
            if (match.Success)
            {
                TES5ObjectProperty propertyReference = this.objectPropertyFactory.CreateObjectProperty(match.Groups[1].Value, match.Groups[2].Value, this, localScope, globalScope, multipleScriptsScope);
                return propertyReference;
            }

            ITES5VariableOrProperty? property = localScope.TryGetVariable(referenceName);
            if (property == null)
            {
                property = globalScope.TryGetPropertyByOriginalName(referenceName); //todo rethink how to unify the prefix searching
                if (property == null)
                {
                    Nullable<int> tes4FormID = null;
                    ITES5Type propertyType =
                        typeForNewProperty != null ? typeForNewProperty :
                        GetPropertyTypeAndFormID(referenceName, tes4ReferenceNameForType, globalScope, multipleScriptsScope, out tes4FormID);
                    TES5Property propertyToAddToGlobalScope = TES5PropertyFactory.ConstructWithTES4FormID(referenceName, propertyType, referenceName, tes4FormID);
                    globalScope.AddProperty(propertyToAddToGlobalScope);
                    property = propertyToAddToGlobalScope;
                }
            }

            return CreateReferenceToVariableOrProperty(property);
        }
        public ITES5Referencer CreateReference(string referenceName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return CreateReference(referenceName, null, referenceName, globalScope, multipleScriptsScope, localScope);
        }
        public ITES5Referencer CreateReference(string referenceName, TES5BasicType typeForNewProperty, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return CreateReference(referenceName, typeForNewProperty, null, globalScope, multipleScriptsScope, localScope);
        }
        public ITES5Referencer CreateReference(string referenceName, string tes4ReferenceNameForType, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return CreateReference(referenceName, null, tes4ReferenceNameForType, globalScope, multipleScriptsScope, localScope);
        }

        private readonly static Regex fileNameRE = new Regex(@"(qf|tif)_[A-Za-z0-9]*_([0-9a-f]+)(_([0-9]+)_([0-9]+))?", RegexOptions.Compiled);
        private static Tuple<int, StageIndexAndLogIndex?> GetTES4FormIDStageIndexAndLogIndex(string fileNameNoExt, TES5FragmentType fragmentType)
        {
            Match fileNameMatch = fileNameRE.Match(fileNameNoExt);
            if (!fileNameMatch.Success)
            {
                throw new ConversionException(fileNameNoExt + " did not match pattern.");
            }
            string scriptTES5FormIDHex = fileNameMatch.Groups[2].Value;
            StageIndexAndLogIndex? stageIndexAndLogIndex = null;
            if (fragmentType == TES5FragmentType.T_QF)
            {
                stageIndexAndLogIndex = new StageIndexAndLogIndex(int.Parse(fileNameMatch.Groups[4].Value), int.Parse(fileNameMatch.Groups[5].Value));
            }
            int scriptTES4FormID = Convert.ToInt32(scriptTES5FormIDHex, 16) - 0x01000000;
            return new Tuple<int, StageIndexAndLogIndex?>(scriptTES4FormID, stageIndexAndLogIndex);
        }

        public static Dictionary<string, KeyValuePair<int, TES5BasicType>> GetTypesFromSCRO(ESMAnalyzer esmAnalyzer, string fileNameNoExt, TES5FragmentType fragmentType)
        {
            Tuple<int, StageIndexAndLogIndex?> formIDStageIndexAndLogIndex = GetTES4FormIDStageIndexAndLogIndex(fileNameNoExt, fragmentType);
            return esmAnalyzer.GetTypesFromSCRO(formIDStageIndexAndLogIndex.Item1, formIDStageIndexAndLogIndex.Item2);
        }

        private static Nullable<KeyValuePair<int, TES5BasicType>> GetTypeFromSCRO(ESMAnalyzer esmAnalyzer, string edidOrFileNameNoExt, string propertyName)
        {
            TES5FragmentType? fragmentType = GetFragmentType(edidOrFileNameNoExt);
            if (fragmentType != null)
            {
                Tuple<int, StageIndexAndLogIndex?> formIDStageIndexAndLogIndex = GetTES4FormIDStageIndexAndLogIndex(edidOrFileNameNoExt, fragmentType);
                return esmAnalyzer.GetTypeFromSCRO(formIDStageIndexAndLogIndex.Item1, formIDStageIndexAndLogIndex.Item2, propertyName);
            }
            return esmAnalyzer.GetTypeFromSCRO(edidOrFileNameNoExt, propertyName);
        }

        private static TES5FragmentType? GetFragmentType(string edidOrFileNameNoExt)
        {
            return edidOrFileNameNoExt.StartsWith(qf_Prefix) ? TES5FragmentType.T_QF : edidOrFileNameNoExt.StartsWith(tif_Prefix) ? TES5FragmentType.T_TIF : null;
        }
    }
}