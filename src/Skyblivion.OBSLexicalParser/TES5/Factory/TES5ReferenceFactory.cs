using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ReferenceFactory
    {
        public const string MESSAGEBOX_VARIABLE_CONST = "TES4_MESSAGEBOX_RESULT";
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private Dictionary<string, ITES5Type> special_conversions;
        public TES5ReferenceFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectPropertyFactory objectPropertyFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            //Those are used to hook in the internal Skyblivion systems.
            special_conversions = new Dictionary<string, ITES5Type>()
            {
                { "TES4AttrStrength",  TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrIntelligence", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrWillpower", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrAgility", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrSpeed", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrEndurance", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrPersonality", TES5BasicType.T_GLOBALVARIABLE },
                { "TES4AttrLuck", TES5BasicType.T_GLOBALVARIABLE },
                { "tContainer", TES5TypeFactory.memberByValue("TES4Container", TES5BasicType.T_QUEST) },//Data container
                { "tTimer", TES5TypeFactory.memberByValue("TES4TimerHelper", TES5BasicType.T_QUEST) },//Timer functions
                { "tGSPLocalTimer", TES5BasicType.T_FLOAT },//used for get seconds passed logical conversion
                { "CyrodiilCrimeFaction", TES5BasicType.T_FACTION },//global cyrodiil faction, WE HAVE BETTER CRIME SYSTEM IN CYRODIIL DAWG
                { MESSAGEBOX_VARIABLE_CONST, TES5BasicType.T_INT }//set by script instead of original messageBox
            };
        }

        public TES5StaticReference createReferenceToStaticClass(string name)
        {
            return new TES5StaticReference(name);
        }

        public TES5SelfReference createReferenceToSelf(TES5GlobalScope globalScope)
        {
            //todo perhaps move tes5scriptAsVariable to a new factory
            return new TES5SelfReference(new TES5ScriptAsVariable(globalScope.getScriptHeader()));
        }

        public TES5Reference createReferenceToVariable(ITES5Variable variable)
        {
            return new TES5Reference(variable);
        }

        public TES5PlayerReference createReferenceToPlayer()
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
         * 
         * @throws \Ormin\OBSLexicalParser\TES5\Exception\ConversionException
        */
        public ITES5Referencer extractImplicitReference(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            ITES5Type type = globalScope.getScriptHeader().getBasicScriptType();
            if (type == TES5BasicType.T_OBJECTREFERENCE)
            {
                return this.createReferenceToSelf(globalScope);
            }
            else if (type == TES5BasicType.T_ACTIVEMAGICEFFECT)
            {
                TES5SelfReference self = this.createReferenceToSelf(globalScope);
                return this.objectCallFactory.createObjectCall(self, "GetTargetActor", multipleScriptsScope);
            }
            else if (type == TES5BasicType.T_QUEST)
            {
                //todo - this should not be done like this
                //we should actually not try to extract the implicit reference on the non-reference oblivion functions like "stopQuest"
                //think of this line as a hacky way to just get code forward.
                return this.createReferenceToSelf(globalScope);
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
            if (rawReference.getType() == TES5BasicType.T_GLOBALVARIABLE)
            {
                //Changed to int implementation.
                return this.objectCallFactory.createObjectCall(rawReference, "GetValueInt", multipleScriptsScope);
            }
            else
            {
                return rawReference;
            }
        }

        /*
        * Create a generic-purpose reference.
        */
        public ITES5Referencer createReference(string referenceName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, TES5LocalScope localScope)
        {
            //Papyrus compiler somehow treats properties with ,,temp" in them in a special way, so we change them to tmp to accomodate that.
            referenceName = Regex.Replace(referenceName, "temp", "tmp", RegexOptions.IgnoreCase);

            if (referenceName.ToLower() == "player")
            {
                return this.createReferenceToPlayer();
            }

            Match match = Regex.Match(referenceName, @"([0-9a-zA-Z]+)\.([0-9a-zA-Z]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                ITES5Referencer mainReference = this.createReference(match.Groups[1].Value, globalScope, multipleScriptsScope, localScope);
                TES5ObjectProperty propertyReference = this.objectPropertyFactory.createObjectProperty(multipleScriptsScope, mainReference, match.Groups[2].Value); //Todo rethink the prefix adding
                return propertyReference;
            }

            ITES5Variable property = localScope.getVariableByName(referenceName);
            if (property == null)
            {
                property = globalScope.getPropertyByName(referenceName); //todo rethink how to unify the prefix searching
                if (property == null)
                {
                    TES5Property propertyToAddToGlobalScope = null;
                    if (this.special_conversions.ContainsKey(referenceName))
                    {
                        propertyToAddToGlobalScope = new TES5Property(referenceName, this.special_conversions[referenceName], referenceName);
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
                    globalScope.add(propertyToAddToGlobalScope);
                    property = propertyToAddToGlobalScope;
                }
            }

            return new TES5Reference(property);
        }
    }
}