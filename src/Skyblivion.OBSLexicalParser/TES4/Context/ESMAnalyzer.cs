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
        private readonly Dictionary<string, ITES5Type> edidLowerCache = new Dictionary<string, ITES5Type>();
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
                string edid = scpt.GetSubrecordTrim("EDID");
                if (edid == "") { throw new InvalidOperationException("EDID was empty."); }//WTM:  Note:  I doubt this will ever happen.
                ITES5Type scriptType;
                //bool isQuest = ((int)schr[16]) != 0;
                bool isActiveMagicEffect = ((int)schr[17]) != 0;
                /*if (isQuest)
                {
                    scriptType = TES5BasicType.T_QUEST;
                }
                else */if (isActiveMagicEffect)
                {
                    scriptType = TES5BasicType.T_ACTIVEMAGICEFFECT;
                }
                else
                {
                    //scriptType = TES5BasicType.T_OBJECTREFERENCE;
                    //WTM:  Change:  Replaced above with the below:
                    TES4LoadedRecord[] tes4Records = ESM.GetRecordsBySCRI(scpt.GetFormId());
                    ITES5Type? esmType = GetTypeByRecords(tes4Records, edid, edid, TypeMapperMode.CompatibilityForScripts);
                    scriptType = esmType != null ? esmType : TES5BasicType.T_FORM;
                    //WTM:  Change:  Added:
                    //The below Oblivion script files do not exist.
                    //Compilation will fail for any script that tries to declare properties by these names.
                    if (edid == "DAPeryiteDoorToTamrielSCRIPT" || edid == "MillonaUmbranoxScript" || edid == "SE11bQuestScript" || edid == "SE36QuestScript")
                    {//Instead, I replace them with their native types.
                        scriptType = scriptType.NativeType;
                    }
                }
                scriptTypes.Add(edid.ToLower(), scriptType);
            }
            return scriptTypes;
        }

        private TES5GlobalVariables GetGlobalVariables()
        {
            List<TES5GlobalVariable> globalArray = new List<TES5GlobalVariable>();
            List<TES4Grup> globals = ESM.GetGrup(TES4RecordType.GLOB);
            foreach (ITES4Record global in globals.SelectMany(g => g.Select(r => r)))
            {
                string edid = global.GetSubrecordTrim("EDID");
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

        public TES4LoadedRecord? TryGetRecordByEDIDInTES4Collection(string edid)
        {
            return ESM.TryGetRecordByEDID(edid);
        }
        public TES4LoadedRecord GetRecordByEDIDInTES4Collection(string edid)
        {
            return ESM.GetRecordByEDID(edid);
        }

        /*
             * @throws ConversionException
        */
        private TES5BasicType? TryGetTypeByEDID(string edid, bool throwException)
        {
            TES4LoadedRecord? record = TryGetRecordByEDIDInTES4Collection(edid);
            if (record == null)
            {
                //WTM:  Change:  These EDIDs can't be found, so I've written them into the code.
                if (edid == "SE02FIN") { return TES5BasicType.T_QUEST; }
                //if (edid == "LvlSpell") { return TES5BasicType.T_SPELL; }
                //if (edid == "GatekeeperRef") { return TES5BasicType.T_ACTOR; }
                if (throwException)
                {
                    throw new ConversionException("Cannot find type for EDID " + edid);
                }
                return null;
            }
            return TypeMapper.GetTES5BasicType(record.RecordType,
#if !ALTERNATE_TYPE_MAPPING
                TypeMapperMode.Strict,
#endif
                out _);
        }
        public TES5BasicType? TryGetTypeByEDID(string edid)
        {
            return TryGetTypeByEDID(edid, false);
        }
        public TES5BasicType GetTypeByEDID(string edid)
        {
            return TryGetTypeByEDID(edid, true)!;
        }

        private static TES5BasicType? GetCommonBaseTES5Type(IList<TES4LoadedRecord> tes4Records, string edid, TypeMapperMode typeMapperMode, out bool compatibility)
        {
            compatibility = false;
            if (!tes4Records.Any()) { return null; }
            bool allCompatibility = true;
            TES5BasicType[] tes5Types = tes4Records.Select(r =>
            {
                bool currentCompatibility;
                TES5BasicType basicType = TypeMapper.GetTES5BasicType(r.RecordType,
#if !ALTERNATE_TYPE_MAPPING
                    typeMapperMode,
#endif
                    out currentCompatibility);
                if (!currentCompatibility) { allCompatibility = false; }
                return basicType;
            }).ToArray();
            compatibility = allCompatibility;
            TES5BasicType commonBaseType = TES5InheritanceGraphAnalyzer.GetCommonBaseType(tes5Types);
            return commonBaseType;
        }

        //WTM:  Note:  For the type inferencer (and eventually compilation) to work, I need to allow inference for some types.  I'm guessing this will cause runtime problems.
        private static readonly string[] mayRevertToFormScriptNames = new string[] { "NoActivationScript", "SE32GhostObject", "SE38MuseumItemSCRIPT", "SE38OdditySCRIPT", "SE39ObjectScript" };
        private static ITES5Type? GetTypeByRecords(IList<TES4LoadedRecord> tes4Records, string? scriptName, string edid, TypeMapperMode typeMapperMode)
        {
            bool compatibility;
            TES5BasicType? commonBaseType = GetCommonBaseTES5Type(tes4Records, edid, typeMapperMode, out compatibility);
            if (commonBaseType == null) { return null; }
            ITES5Type tes5Type;
            if (scriptName != null)
            {
                if (!compatibility && mayRevertToFormScriptNames.Contains(scriptName)) { compatibility = true; }
                tes5Type = TES5TypeFactory.MemberByValue(scriptName, commonBaseType, compatibility);
            }
            else
            {
                tes5Type =
#if ALTERNATE_TYPE_MAPPING
                    compatibility ? new TES5BasicTypeRevertible(commonBaseType) : (ITES5Type)
#endif
                    commonBaseType;
            }
            return tes5Type;
        }

        public ITES5Type? GetTypeByEDIDWithFollow(string edid, TypeMapperMode typeMapperMode, bool useScriptTypeCache = true)
        {
            string? scriptName;
            TES4LoadedRecord[] records = TryGetRecordsByEDIDFollowNAMEAndLookUpSCRIIfNeeded(edid, out scriptName);
            if (useScriptTypeCache && scriptName != null) { return GetScriptTypeByScriptNameFromCache(scriptName); }
            string typeEDID = scriptName != null ? scriptName : edid;
            return GetTypeByRecords(records, scriptName, typeEDID, typeMapperMode);
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type GetScriptTypeByScriptNameFromCache(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            ITES5Type value;
            if (ScriptTypes.TryGetValue(scriptNameLower, out value)) { return value; }
            string scriptNameLowerNoPoundSymbols = scriptNameLower.Replace("#", "");
            if (ScriptTypes.TryGetValue(scriptNameLowerNoPoundSymbols, out value)) { return value; }
            throw new ConversionException("Script " + scriptNameLower + " not found in ESM - cannot find its script type.");
        }

        public TES5BasicType GetScriptNativeTypeByScriptNameFromCache(string scriptName)
        {
            return GetScriptTypeByScriptNameFromCache(scriptName).NativeType;
        }

        private TES4LoadedRecord? TryGetRecordByEDIDAndFollowNAME(string edid, bool throwException)
        {
            TES4LoadedRecord? record = TryGetRecordByEDIDInTES4Collection(edid);
            if (record == null)
            {
                if (throwException)
                {
                    throw new ConversionException("Cannot resolve script type by searching its base form edid - no record found, " + edid);
                }
                return null;
            }
            TES4RecordType recordType = record.RecordType;
            if (recordType == TES4RecordType.REFR || recordType == TES4RecordType.ACRE || recordType == TES4RecordType.ACHR)
            {
                //Resolve the reference
                int baseFormid = record.GetSubrecordAsFormid("NAME");
                record = ESM.GetRecordByFormID(baseFormid);
            }
            return record;
        }
        public TES4LoadedRecord GetRecordByEDIDAndFollowNAME(string edid)
        {
            return TryGetRecordByEDIDAndFollowNAME(edid, true)!;
        }
        public TES4LoadedRecord? TryGetRecordByEDIDAndFollowNAME(string edid)
        {
            return TryGetRecordByEDIDAndFollowNAME(edid, false);
        }

        private TES4LoadedRecord[] TryGetRecordsByEDIDFollowNAMEAndLookUpSCRIIfNeeded(string edid, out string? scriptEDID)
        {
            scriptEDID = null;
            TES4LoadedRecord? record = TryGetRecordByEDIDAndFollowNAME(edid, false);
            if (record == null)
            {
                return new TES4LoadedRecord[] { };
            }
            TES4RecordType recordType = record.RecordType;
            if (recordType == TES4RecordType.SCPT)
            {
                //Resolve the reference
                TES4LoadedRecord[] recordsBySCRI = ESM.GetRecordsBySCRI(record.GetFormId());
                if (recordsBySCRI.Any())
                {
                    return recordsBySCRI;
                }
            }
            else
            {
                Nullable<int> scri = record.GetSubrecordAsFormidNullable("SCRI");
                if (scri != null)
                {
                    TES4LoadedRecord scptRecord = ESM.GetRecordByFormID(scri.Value);
                    scriptEDID = scptRecord.GetSubrecordTrimNullable("EDID");
                }
            }
            return new TES4LoadedRecord[] { record };
        }

        /*
        * @todo REFACTOR, it"s really ugly!
        * @throws ConversionException
        */
        public ITES5Type GetScriptTypeByEDID(string edid)
        {
            string edidLower = edid.ToLower();
            ITES5Type value;
            if (this.edidLowerCache.TryGetValue(edidLower, out value)) { return value; }
            TES4LoadedRecord attachedNameRecord = GetRecordByEDIDAndFollowNAME(edid);

            Nullable<int> scriptFormid = attachedNameRecord.GetSubrecordAsFormidNullable("SCRI");
            if (scriptFormid == null)
            {
                throw new ConversionException("Cannot resolve script type for " + edid + " - Asked base record has no script bound.");
            }

            TES4LoadedRecord scriptRecord = ESM.GetRecordByFormID(scriptFormid.Value);
            string? subrecord = scriptRecord.GetSubrecordTrimNullable("EDID");
            if (subrecord == null) { throw new InvalidOperationException(nameof(subrecord) + " was null for EDID."); }
            ITES5Type customType = TES5TypeFactory.MemberByValue(subrecord, this);
            this.edidLowerCache.Add(edidLower, customType);
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