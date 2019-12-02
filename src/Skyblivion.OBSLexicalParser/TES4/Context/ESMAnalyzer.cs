using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Context
{
    /*
     * Class ESMAnalyzer
     *
     * Answers the questions regarding the context within the binary data file
     * Acts as a legacy adapter interface between ScriptConverter and ESReader
     *
     */
    class ESMAnalyzer
    {
        private Lazy<TES4Collection>? esmLazy = null;
        private TES4Collection ESM { get { if (esmLazy == null) { throw new NullableException(nameof(esmLazy)); } return esmLazy.Value; } }
        private readonly Lazy<Dictionary<string, ITES5Type>> scriptTypesLazy;
        private Dictionary<string, ITES5Type> ScriptTypes => scriptTypesLazy.Value;
        private readonly Lazy<TES5GlobalVariables> globalVariablesLazy;
        public TES5GlobalVariables GlobalVariables => globalVariablesLazy.Value;
        private readonly Dictionary<string, ITES5Type> attachedNameCache = new Dictionary<string, ITES5Type>();
        public ESMAnalyzer(bool loadLazily)
        {
            esmLazy = new Lazy<TES4Collection>(() => GetESM());
            scriptTypesLazy = new Lazy<Dictionary<string, ITES5Type>>(GetScriptTypes);
            globalVariablesLazy = new Lazy<TES5GlobalVariables>(GetGlobalVariables);
            if (!loadLazily) { LoadLazyObjects(); }
        }

        private static TES4Collection GetESM()
        {
            return TES4CollectionFactory.Create(DataDirectory.GetESMDirectoryPath(), DataDirectory.TES4GameFileName);
        }

        private Dictionary<string, ITES5Type> GetScriptTypes()
        {
            Dictionary<string, ITES5Type> scriptTypes = new Dictionary<string, ITES5Type>();
            List<TES4Grup> scpts = ESM.GetGrup(TES4RecordType.SCPT);
            foreach (ITES4Record scpt in scpts.SelectMany(s => s.Select(r => r)))
            {
                string schr = scpt.GetSubrecordString("SCHR");
                string? edid = scpt.GetSubrecordTrimLower("EDID");
                if (string.IsNullOrWhiteSpace(schr) || string.IsNullOrWhiteSpace(edid) || edid == null/*Needed for nullable reference types, but string.IsNullOrWhiteSpace should have sufficed.*/)
                {
                    continue;
                }

                TES5BasicType scriptType;
                if (edid == "dark09skeletonsuicidescript" || edid == "xpebroccabossscript")//WTM:  Change:  Added special condition
                {
                    scriptType = TES5BasicType.T_ACTOR;
                }
                else
                {
                    bool isQuest = ((int)schr.Substring(16, 1)[0]) != 0;
                    bool isActiveMagicEffect = ((int)schr.Substring(17, 1)[0]) != 0;
                    if (isQuest)
                    {
                        scriptType = TES5BasicType.T_QUEST;
                    }
                    else if (isActiveMagicEffect)
                    {
                        scriptType = TES5BasicType.T_ACTIVEMAGICEFFECT;
                    }
                    else
                    {
                        scriptType = TES5BasicType.T_OBJECTREFERENCE;
                    }
                }
                scriptTypes.Add(edid, scriptType);
            }
            return scriptTypes;
        }

        private TES5GlobalVariables GetGlobalVariables()
        {
            List<TES5GlobalVariable> globalArray = new List<TES5GlobalVariable>();
            List<TES4Grup> globals = ESM.GetGrup(TES4RecordType.GLOB);
            foreach (ITES4Record global in globals.SelectMany(g => g.Select(r => r)))
            {
                string? edid = global.GetSubrecordTrim("EDID");
                if (string.IsNullOrWhiteSpace(edid) || edid == null/*Needed for nullable reference types, but string.IsNullOrWhiteSpace should have sufficed.*/)
                {
                    continue;
                }

                globalArray.Add(new TES5GlobalVariable(edid));
            }

            /*
             * Hacky - add infamy into the globals array
             * Probably we should extract this from this class and put this into other place
             */
            globalArray.Add(new TES5GlobalVariable("Infamy"));
            return new TES5GlobalVariables(globalArray);
        }

        /*
         * This loads the lazies so that ESM data will be immediately available later.
         * If this method isn't called, the first attempt to access these properties will result in loading the ESM, which is time consuming.
         * This greatly improves the performance of TestStageMap and BuildFileDeleteCommand.
        */
        private void LoadLazyObjects()
        {
            _ = ScriptTypes;
            _ = GlobalVariables;
        }

        public TES4LoadedRecord? TryFindInTES4Collection(string edid)
        {
            return ESM.TryFindByEDID(edid);
        }
        public TES4LoadedRecord FindInTES4Collection(string edid)
        {
            return ESM.FindByEDID(edid);
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type GetFormTypeByEDID(string edid)
        {
            TES4LoadedRecord? record = TryFindInTES4Collection(edid);
            if (record == null)
            {
                //WTM:  Change:  These EDIDs can't be found, so I've written them into the code.
                if (edid == "SE02FIN") { return TES5BasicType.T_QUEST; }
                if (edid == "LvlSpell") { return TES5BasicType.T_SPELL; }
                throw new ConversionException("Cannot find type for EDID " + edid);
            }
            return TypeMapper.Map(record.RecordType, this);
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type GetScriptType(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            ITES5Type value;
            if (ScriptTypes.TryGetValue(scriptNameLower, out value)) { return value; }
            string scriptNameLowerNoPoundSymbols = scriptNameLower.Replace("#", "");
            if (ScriptTypes.TryGetValue(scriptNameLowerNoPoundSymbols, out value)) { return value; }
            throw new ConversionException("Script " + scriptNameLower + " not found in ESM - cannot find its script type.");
        }

        /*
        * @todo REFACTOR, it"s really ugly!
        * @throws ConversionException
        */
        public ITES5Type ResolveScriptTypeByItsAttachedName(string attachedName)
        {
            string attachedNameLower = attachedName.ToLower();
            ITES5Type value;
            if (this.attachedNameCache.TryGetValue(attachedNameLower, out value)) { return value; }
            TES4LoadedRecord? attachedNameRecord = TryFindInTES4Collection(attachedName);
            if (attachedNameRecord == null)
            {
                throw new ConversionException("Cannot resolve script type by searching its base form edid - no record found, " + attachedName);
            }
            TES4RecordType attachedNameRecordType = attachedNameRecord.RecordType;
            if (attachedNameRecordType == TES4RecordType.REFR || attachedNameRecordType == TES4RecordType.ACRE || attachedNameRecordType == TES4RecordType.ACHR)
            {
                //Resolve the reference
                int baseFormid = attachedNameRecord.GetSubrecordAsFormid("NAME");
                attachedNameRecord = ESM.FindByFormid(baseFormid);
            }

            Nullable<int> scriptFormid = attachedNameRecord.GetSubrecordAsFormidNullable("SCRI");
            if (scriptFormid == null)
            {
                throw new ConversionException("Cannot resolve script type for " + attachedName + " - Asked base record has no script bound.");
            }

            TES4LoadedRecord scriptRecord = ESM.FindByFormid(scriptFormid.Value);
            string? subrecord = scriptRecord.GetSubrecordTrim("EDID");
            if (subrecord == null) { throw new InvalidOperationException(nameof(subrecord) + " was null for EDID."); }
            ITES5Type customType = TES5TypeFactory.MemberByValue(subrecord, null, this);
            this.attachedNameCache.Add(attachedNameLower, customType);
            return customType;
        }

        /*
        * Makes the adapter unusable by deallocating the esm object.
         * This really ought to be more clean, but until this class is used statically we have no other choice
        */
        public void Deallocate()
        {
            //Drop the ref
            esmLazy = null;
            //Force the GC
            GC.Collect();
        }
    }
}