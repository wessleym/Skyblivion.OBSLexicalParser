using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ReferenceFactory
    {
        public const string MESSAGEBOX_VARIABLE_CONST = TES5TypeFactory.TES4Prefix + "_MESSAGEBOX_RESULT";
        public static readonly Regex PropertyNameRegex = new Regex(@"([0-9a-zA-Z]+)\.([0-9a-zA-Z]+)", RegexOptions.Compiled);
        private const string tContainerName = "tContainer";
        private const string tTimerName = "tTimer";
        private const string tGSPLocalTimerName = "tGSPLocalTimer";
        private const string cyrodiilCrimeFactionName = "CyrodiilCrimeFaction";
        public const string TES4Attr = TES5TypeFactory.TES4Prefix + "Attr";
        //Those are used to hook in the internal Skyblivion systems.
        private static readonly Dictionary<string, ITES5Type> specialConversions = new Dictionary<string, ITES5Type>()
            {
                { TES4Attr + "Strength",  TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Intelligence", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Willpower", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Agility", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Speed", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Endurance", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Personality", TES5BasicType.T_GLOBALVARIABLE },
                { TES4Attr + "Luck", TES5BasicType.T_GLOBALVARIABLE },
                { tContainerName, TES5TypeFactory.memberByValue(TES5BasicType.TES4ContainerName, TES5BasicType.T_QUEST) },//Data container
                { tTimerName, TES5TypeFactory.memberByValue(TES5BasicType.TES4TimerHelperName, TES5BasicType.T_QUEST) },//Timer functions
                { tGSPLocalTimerName, TES5BasicType.T_FLOAT },//used for get seconds passed logical conversion
                { cyrodiilCrimeFactionName, TES5BasicType.T_FACTION },//global cyrodiil faction, WE HAVE BETTER CRIME SYSTEM IN CYRODIIL DAWG
                { MESSAGEBOX_VARIABLE_CONST, TES5BasicType.T_INT }//set by script instead of original messageBox
            };
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        public TES5ReferenceFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectPropertyFactory objectPropertyFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectPropertyFactory = objectPropertyFactory;
        }

        public static TES5SelfReference CreateReferenceToSelf(TES5GlobalScope globalScope)
        {
            //todo perhaps move tes5scriptAsVariable to a new factory
            return new TES5SelfReference(new TES5ScriptAsVariable(globalScope.ScriptHeader));
        }

        public static TES5Reference CreateReferenceToVariable(ITES5Variable variable)
        {
            return new TES5Reference(variable);
        }

        public static ITES5Referencer CreateReferenceToPlayer()
        {
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
        public ITES5Referencer extractImplicitReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            ITES5Type type = globalScope.ScriptHeader.BasicScriptType;
            if (type == TES5BasicType.T_OBJECTREFERENCE || type==TES5BasicType.T_ACTOR)//Change:  WTM:  Added Actor here.
            {
                return CreateReferenceToSelf(globalScope);
            }
            else if (type == TES5BasicType.T_ACTIVEMAGICEFFECT)
            {
                TES5SelfReference self = CreateReferenceToSelf(globalScope);
                return this.objectCallFactory.CreateObjectCall(self, "GetTargetActor", multipleScriptsScope);
            }
            else if (type == TES5BasicType.T_QUEST)
            {
                //todo - this should not be done like this
                //we should actually not try to extract the implicit reference on the non-reference oblivion functions like "stopQuest"
                //think of this line as a hacky way to just get code forward.
                return CreateReferenceToSelf(globalScope);
            }
            else if (type == TES5BasicType.T_TOPICINFO)
            {
                /*
                 * TIF Fragments
                 */
                return this.createReadReference("akSpeakerRef", globalScope, multipleScriptsScope, localScope);
            }

            throw new ConversionException("Cannot extract implicit reference - unknown basic script type.");
        }

        /*
        * Create the ,,read reference".
         * Read reference is used ( as you might think ) in read contexts.
        */
        public ITES5Referencer createReadReference(string referenceName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            ITES5Referencer rawReference = this.createReference(referenceName, globalScope, multipleScriptsScope, localScope);
            if (rawReference.TES5Type == TES5BasicType.T_GLOBALVARIABLE)
            {
                //Changed to int implementation.
                return this.objectCallFactory.CreateObjectCall(rawReference, "GetValueInt", multipleScriptsScope);
            }
            else
            {
                return rawReference;
            }
        }

        public ITES5Referencer CreateContainerReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return createReadReference(tContainerName, globalScope, multipleScriptsScope, localScope);
        }

        public ITES5Referencer CreateTimerReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return createReadReference(tTimerName, globalScope, multipleScriptsScope, localScope);
        }

        public ITES5Referencer CreateGSPLocalTimerReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return createReadReference(tGSPLocalTimerName, globalScope, multipleScriptsScope, localScope);
        }

        public ITES5Referencer CreateCyrodiilCrimeFactionReadReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            return createReadReference(cyrodiilCrimeFactionName, globalScope, multipleScriptsScope, localScope);
        }

        /*
        * Create a generic-purpose reference.
        */
        public ITES5Referencer createReference(string referenceName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            if (TES5PlayerReference.EqualsPlayer(referenceName))
            {
                return CreateReferenceToPlayer();
            }

            referenceName = PapyrusCompiler.FixReferenceName(referenceName);
            Match match = PropertyNameRegex.Match(referenceName);
            if (match.Success)
            {
                ITES5Referencer mainReference = this.createReference(match.Groups[1].Value, globalScope, multipleScriptsScope, localScope);
                TES5ObjectProperty propertyReference = this.objectPropertyFactory.createObjectProperty(multipleScriptsScope, mainReference, match.Groups[2].Value); //Todo rethink the prefix adding
                return propertyReference;
            }

            ITES5Variable property = localScope.GetVariable(referenceName);
            if (property == null)
            {
                property = globalScope.getPropertyByName(referenceName); //todo rethink how to unify the prefix searching
                if (property == null)
                {
                    TES5Property propertyToAddToGlobalScope = null;
                    ITES5Type specialConversion;
                    if (specialConversions.TryGetValue(referenceName, out specialConversion))
                    {
                        propertyToAddToGlobalScope = new TES5Property(referenceName, specialConversion, referenceName);
                    }

                    if (propertyToAddToGlobalScope == null)
                    {
                        if (!multipleScriptsScope.hasGlobalVariable(referenceName))
                        {
                            propertyToAddToGlobalScope = new TES5Property(referenceName, TES5BasicType.T_FORM, referenceName);
                        }
                        else
                        {
                            propertyToAddToGlobalScope = new TES5Property(referenceName, TES5BasicType.T_GLOBALVARIABLE, referenceName);
                        }
                    }
                    globalScope.Add(propertyToAddToGlobalScope);
                    property = propertyToAddToGlobalScope;
                }
            }

            return new TES5Reference(property);
        }
    }
}