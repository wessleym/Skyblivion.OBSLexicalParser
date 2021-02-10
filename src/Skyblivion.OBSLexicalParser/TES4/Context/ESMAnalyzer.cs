using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
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
    class ESMAnalyzer : IDisposable
    {
        private Lazy<TES4Collection>? esmLazy = null;
        private TES4Collection ESM { get { if (esmLazy == null) { throw new NullableException(nameof(esmLazy)); } return esmLazy.Value; } }
        private readonly Lazy<Dictionary<string, ITES5Type>> scriptTypesLazy;
        private Dictionary<string, ITES5Type> ScriptTypes => scriptTypesLazy.Value;
        private readonly Lazy<TES5GlobalVariables> globalVariablesLazy;
        public TES5GlobalVariables GlobalVariables => globalVariablesLazy.Value;
        private readonly Dictionary<string, ITES5Type> edidLowerCache = new Dictionary<string, ITES5Type>();
        private ESMAnalyzer(bool loadLazily)
        {
            esmLazy = new Lazy<TES4Collection>(() => GetESM());
            scriptTypesLazy = new Lazy<Dictionary<string, ITES5Type>>(GetScriptTypes);
            globalVariablesLazy = new Lazy<TES5GlobalVariables>(GetGlobalVariables);
            if (!loadLazily) { LoadLazyObjects(); }
        }

        public static ESMAnalyzer Load()
        {
            return new ESMAnalyzer(false);
        }

        private static TES4Collection GetESM()
        {
            return TES4CollectionFactory.Create(DataDirectory.GetESMDirectoryPath(), DataDirectory.TES4GameFileName);
        }

        private Dictionary<string, ITES5Type> GetScriptTypes()
        {
            Dictionary<string, ITES5Type> scriptTypes = new Dictionary<string, ITES5Type>();
            List<TES4Grup> scpts = GetGrup(TES4RecordType.SCPT);
            foreach (ITES4Record scpt in scpts.SelectMany(s => s.Select(r => r)))
            {
                string schr = scpt.GetSubrecordString("SCHR");
                string edid = scpt.GetSubrecordTrim("EDID");
                if (edid == "") { throw new InvalidOperationException("EDID was empty."); }//WTM:  Note:  I doubt this will ever happen.
                ITES5Type scriptType;
                bool isQuest = ((int)schr[16]) != 0;
                bool isActiveMagicEffect = ((int)schr[17]) != 0;
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
                    scriptType =
                        //WTM:  Change:  Special Case:
                        edid == "Dark09SkeletonSuicideSCRIPT" ? TES5BasicType.T_ACTOR ://Compilation fails without this.
                        TES5BasicType.T_OBJECTREFERENCE;
                    /*
                    //WTM:  Change:  Replaced above with the below:
                    TES4LoadedRecord[] tes4Records = GetRecordsBySCRI(scpt.GetFormId());
                    ITES5Type? esmType = GetTypeByRecords(tes4Records, edid, edid, TypeMapperMode.CompatibilityForScripts);
                    scriptType = esmType != null ? esmType : TES5BasicType.T_FORM;
                    //WTM:  Change:  Added:
                    //The below Oblivion script files do not exist.
                    //Compilation will fail for any script that tries to declare properties by these names.
                    if (edid == "DAPeryiteDoorToTamrielSCRIPT" || edid == "MillonaUmbranoxScript" || edid == "SE11bQuestScript" || edid == "SE36QuestScript")
                    {//Instead, I replace them with their native types.
                        scriptType = scriptType.NativeType;
                    }
                    */
                }
                scriptTypes.Add(edid.ToLower(), scriptType);
            }
            return scriptTypes;
        }

        private TES5GlobalVariables GetGlobalVariables()
        {
            List<TES5GlobalVariable> globalArray = new List<TES5GlobalVariable>();
            List<TES4Grup> globals = GetGrup(TES4RecordType.GLOB);
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
        */
        private void LoadLazyObjects()
        {
            _ = ScriptTypes;
            _ = GlobalVariables;
        }

        public List<TES4Grup> GetGrup(TES4RecordType type)
        {
            return ESM.GetGrup(type);
        }

        public TES4LoadedRecord? TryGetRecordByEDIDInTES4Collection(string edid)
        {
            return ESM.TryGetRecordByEDID(edid);
        }
        public TES4LoadedRecord GetRecordByEDIDInTES4Collection(string edid)
        {
            return ESM.GetRecordByEDID(edid);
        }

        public TES4LoadedRecord GetRecordByFormID(int formID)
        {
            return ESM.GetRecordByFormID(formID);
        }

        public string? GetEDIDByFormIDNullable(int formID)
        {
            return ESM.GetEDIDByFormIDNullable(formID);
        }

        public string GetEDIDByFormID(int formID)
        {
            return ESM.GetEDIDByFormID(formID);
        }

        public TES4LoadedRecord[] GetRecordsBySCRI(int formID)
        {
            return ESM.GetRecordsBySCRI(formID);
        }

        public IEnumerable<TES4LoadedRecord> GetRecords()
        {
            return ESM;
        }

        /*
             * @throws ConversionException
        */
        private TES5BasicType? TryGetTypeByEDID(string edid, bool throwException)
        {
            TES4LoadedRecord? record = TryGetRecordByEDIDInTES4Collection(edid);
            if (record == null)
            {
                //WTM:  Change:  Special Cases:
                //These EDIDs can't be found, so I've written them into the code.
                if (edid == "SE02FIN") { return TES5BasicType.T_QUEST; }
                //if (edid == "LvlSpell") { return TES5BasicType.T_SPELL; }
                //if (edid == "GatekeeperRef") { return TES5BasicType.T_ACTOR; }
                if (throwException)
                {
                    throw new ConversionException("Cannot find type for EDID " + edid);
                }
                return null;
            }
            return TypeMapper.GetTES5BasicType(record.RecordType);
        }
        public TES5BasicType? TryGetTypeByEDID(string edid)
        {
            return TryGetTypeByEDID(edid, false);
        }
        public TES5BasicType GetTypeByEDID(string edid)
        {
            return TryGetTypeByEDID(edid, true)!;
        }

        private static TES5BasicType? GetCommonBaseTES5Type(IList<TES4LoadedRecord> tes4Records)
        {
            if (!tes4Records.Any()) { return null; }
            TES5BasicType[] tes5Types = tes4Records.Select(r =>
            {
                TES5BasicType basicType = TypeMapper.GetTES5BasicType(r.RecordType);
                return basicType;
            }).ToArray();
            TES5BasicType commonBaseType = TES5InheritanceGraphAnalyzer.GetCommonBaseType(tes5Types);
            return commonBaseType;
        }

        private static ITES5Type? GetTypeByRecords(IList<TES4LoadedRecord> tes4Records, string? scriptName)
        {
            TES5BasicType? commonBaseType = GetCommonBaseTES5Type(tes4Records);
            if (commonBaseType == null) { return null; }
            ITES5Type tes5Type;
            if (scriptName != null)
            {
                tes5Type = TES5TypeFactory.MemberByValue(scriptName, commonBaseType);
            }
            else
            {
                tes5Type = commonBaseType;
            }
            return tes5Type;
        }

        public ITES5Type? GetTypeByEDIDWithFollow(string edid, bool followName, bool useScriptTypeCache = true)
        {
            string? scriptName;
            TES4LoadedRecord[] records = TryGetRecordsByEDIDFollowNAMEAndLookUpSCRIIfNeeded(edid, followName, out scriptName);
            if (useScriptTypeCache && scriptName != null) { return GetScriptTypeByScriptNameFromCache(scriptName); }
            return GetTypeByRecords(records, scriptName);
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type GetScriptTypeByScriptNameFromCache(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            ITES5Type? value;
            if (ScriptTypes.TryGetValue(scriptNameLower, out value)) { return value; }
            string scriptNameLowerNoPoundSymbols = scriptNameLower.Replace("#", "");
            if (ScriptTypes.TryGetValue(scriptNameLowerNoPoundSymbols, out value)) { return value; }
            throw new ConversionException("Script " + scriptNameLower + " not found in ESM - cannot find its script type.");
        }

        public TES5BasicType GetScriptNativeTypeByScriptNameFromCache(string scriptName)
        {
            return GetScriptTypeByScriptNameFromCache(scriptName).NativeType;
        }

        private TES4LoadedRecord? TryGetRecordByEDIDAndFollowNAME(string edid, bool followName, bool throwException)
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
            if (followName && (recordType == TES4RecordType.REFR || recordType == TES4RecordType.ACRE || recordType == TES4RecordType.ACHR))
            {
                //Resolve the reference
                int baseFormid = record.GetSubrecordAsFormid("NAME");
                record = GetRecordByFormID(baseFormid);
            }
            return record;
        }
        public TES4LoadedRecord GetRecordByEDIDAndFollowNAME(string edid)
        {
            return TryGetRecordByEDIDAndFollowNAME(edid, true, true)!;
        }
        public TES4LoadedRecord? TryGetRecordByEDIDAndFollowNAME(string edid, bool followName)
        {
            return TryGetRecordByEDIDAndFollowNAME(edid, followName, false);
        }

        private TES4LoadedRecord[] TryGetRecordsByEDIDFollowNAMEAndLookUpSCRIIfNeeded(string edid, bool followName, out string? scriptEDID)
        {
            scriptEDID = null;
            TES4LoadedRecord? record = TryGetRecordByEDIDAndFollowNAME(edid, followName);
            if (record == null)
            {
                return new TES4LoadedRecord[] { };
            }
            TES4RecordType recordType = record.RecordType;
            if (recordType == TES4RecordType.SCPT)
            {
                //Resolve the reference
                TES4LoadedRecord[] recordsBySCRI = GetRecordsBySCRI(record.FormID);
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
                    scriptEDID = GetEDIDByFormID(scri.Value);
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
            ITES5Type? value;
            if (this.edidLowerCache.TryGetValue(edidLower, out value)) { return value; }
            TES4LoadedRecord attachedNameRecord = GetRecordByEDIDAndFollowNAME(edid);

            Nullable<int> scriptFormid = attachedNameRecord.GetSubrecordAsFormidNullable("SCRI");
            if (scriptFormid == null)
            {
                throw new ConversionException("Cannot resolve script type for " + edid + " - Asked base record has no script bound.");
            }

            string scriptRecordEDID = GetEDIDByFormID(scriptFormid.Value);
            TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptRecordEDID, TypeMapper.GetTES5BasicType(attachedNameRecord.RecordType), TES5TypeFactory.TES4Prefix, false);
            this.edidLowerCache.Add(edidLower, scriptHeader.ScriptType);
            return scriptHeader.ScriptType;
        }

        public Dictionary<string, KeyValuePair<int, TES5BasicType>> GetTypesFromSCRO(TES4LoadedRecord scriptTES4Record, Nullable<int> index)
        {
            IEnumerable<byte[]> scroRecords = scriptTES4Record.GetSCRORecords(index).ToArray();
            HashSet<int> foundFormIDs = new HashSet<int>();
            return scroRecords
                .Select(r => BitConverter.ToInt32(r, 0))
                .Where(formID => !foundFormIDs.Contains(formID))
                .Select(formID =>
                {
                    foundFormIDs.Add(formID);
                    string edid;
                    TES5BasicType tes5Type;
                    if (formID == TES5PlayerReference.FormID)
                    {
                        edid = TES5PlayerReference.PlayerRefName;
                        tes5Type = TES5PlayerReference.TES5TypeStatic;
                    }
                    else
                    {
                        TES4LoadedRecord record = GetRecordByFormID(formID);
                        edid = record.GetSubrecordTrim("EDID");
                        /*
                        if (record.RecordType == TES4RecordType.CREA || record.RecordType == TES4RecordType.NPC_)
                        {
                            //These probably just need to be ActorBase, but that doesn't work in all situations.
                            List<int>? formIDsFromName = TryGetFormIDsByName(formID);
                            if (formIDsFromName == null || formIDsFromName.Count != 1)
                            {//If no form IDs were found or if not exactly one form ID was found, refer to the ActorBase.
                                tes5Type = TES5BasicType.T_ACTORBASE;
                            }
                            else
                            {//If exactly one form ID was found, refer to the Actor;
                                if (formIDsFromName != null) { formIDs = formIDsFromName; }
                                tes5Type = TES5BasicType.T_ACTOR;
                            }
                            tes5Type = TES5BasicType.T_ACTORBASE;
                        }
                        else
                        {
                            tes5Type = TypeMapper.GetTES5BasicType(record.RecordType, out _);
                        }
                        */
                        tes5Type = TypeMapper.GetTES5BasicType(record.RecordType);
                        //It seems Oblivion.esm's scroll BOOK records are now SCRL records in Skyblivion.esm with the same form ID.
                        if (tes5Type == TES5BasicType.T_BOOK && edid.IndexOf("scroll", StringComparison.OrdinalIgnoreCase) != -1 &&
                            !(edid == "MG05FingersScroll" || edid == "TG11ElderScroll" || edid == "TG04ElderScrollsHistory")/*These actually are BOOKs.*/)
                        {
                            tes5Type = TES5BasicType.T_SCROLL;
                        }
                    }
                    return new KeyValuePair<string, KeyValuePair<int, TES5BasicType>>(edid, new KeyValuePair<int, TES5BasicType>(formID, tes5Type));
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value,
                    StringComparer.OrdinalIgnoreCase//WTM:  Note:  required since cases aren't consistent (e.g., MS40HalLiurzCourtyardMark vs. MS40HalliurzCourtyardMark--notice second L)
                    );
        }

        public Dictionary<string, KeyValuePair<int, TES5BasicType>> GetTypesFromSCRO(int scriptTES4FormID, Nullable<int> index)
        {
            TES4LoadedRecord scriptTES4Record = GetRecordByFormID(scriptTES4FormID);
            return GetTypesFromSCRO(scriptTES4Record, index);
        }

        private Nullable<KeyValuePair<int, TES5BasicType>> GetTypeFromSCRO(TES4LoadedRecord scriptTES4Record, Nullable<int> index, string propertyName, bool throwException)
        {
            var types = GetTypesFromSCRO(scriptTES4Record, index);
            KeyValuePair<int, TES5BasicType> type;
            if (types.TryGetValue(propertyName, out type))
            {
                return type;
            }
            if (throwException)
            {
                throw new ConversionException("Form " + scriptTES4Record.FormID.ToString() + (index != null ? ", index " + index.Value.ToString() : "") + " did not have a property named " + propertyName + ".");
            }
            return null;
        }
        public Nullable<KeyValuePair<int, TES5BasicType>> GetTypeFromSCRO(string scriptEDID, string propertyName)
        {
            TES4LoadedRecord scriptTES4Record = GetRecordByEDIDInTES4Collection(scriptEDID);
            return GetTypeFromSCRO(scriptTES4Record, null, propertyName, !(scriptTES4Record.FormID == 211721 && propertyName == "test"));//WTM:  Note:  Special Case:  property not found in SCRO records
        }
        public Nullable<KeyValuePair<int, TES5BasicType>> GetTypeFromSCRO(int scriptTES4FormID, Nullable<int> index, string propertyName)
        {
            TES4LoadedRecord scriptTES4Record = GetRecordByFormID(scriptTES4FormID);
            return GetTypeFromSCRO(scriptTES4Record, index, propertyName, !(scriptTES4FormID == 73629 && index == 150 && propertyName == "SE02FIN"));//WTM:  Note:  Special Case:  property not found in SCRO records
        }

        public void Dispose()
        {
            esmLazy = null;
            GC.Collect();
        }
    }
}