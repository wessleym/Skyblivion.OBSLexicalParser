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
        private Dictionary<string, ITES5Type> scriptTypes;
        private TES5GlobalVariables globals;
        private Dictionary<string, ITES5Type> attachedNameCache = new Dictionary<string, ITES5Type>();
        private static TES4Collection esm;
        private static ESMAnalyzer instance;
        public ESMAnalyzer(string dataFile = DataDirectory.TES4GameFileName)
        {
            if (esm == null)
            {
                TES4Collection collection = new TES4Collection(DataDirectory.GetESMDirectoryPath());
                collection.add(dataFile);
                //NOTE - SCRI record load scheme is a copypasta, as in, i didnt check which records do actually might have SCRI
                //Doesnt really matter for other purposes than cleaniness
                TES4FileLoadScheme fileScheme = new TES4FileLoadScheme();
                TES4GrupLoadScheme grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.GMST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.GMST, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.GLOB, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.GLOB, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CLAS, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CLAS, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.FACT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.FACT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.HAIR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.HAIR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.EYES, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.EYES, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.RACE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.RACE, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SOUN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SOUN, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SKIL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SKIL, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.MGEF, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.MGEF, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SCPT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI", "SCHR" }));
                fileScheme.add(TES4RecordType.SCPT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LTEX, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LTEX, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.ENCH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.ENCH, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SPEL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SPEL, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.BSGN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.BSGN, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.ACTI, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.ACTI, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.APPA, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.APPA, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.ARMO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.ARMO, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.BOOK, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.BOOK, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CLOT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CLOT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CONT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CONT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.DOOR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.DOOR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.INGR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.INGR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LIGH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LIGH, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.MISC, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.MISC, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.STAT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.STAT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.GRAS, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.GRAS, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.TREE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.TREE, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.FLOR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.FLOR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.FURN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.FURN, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.WEAP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.WEAP, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.AMMO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.AMMO, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.NPC_, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.NPC_, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CREA, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CREA, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LVLC, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LVLC, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SLGM, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SLGM, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.KEYM, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.KEYM, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.ALCH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.ALCH, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SBSP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SBSP, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.SGST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.SGST, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LVLI, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LVLI, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.WTHR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.WTHR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CLMT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CLMT, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.REGN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.REGN, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CELL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                grupScheme.add(TES4RecordType.REFR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                grupScheme.add(TES4RecordType.ACHR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                grupScheme.add(TES4RecordType.ACRE, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                fileScheme.add(TES4RecordType.CELL, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.WRLD, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                grupScheme.add(TES4RecordType.CELL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                grupScheme.add(TES4RecordType.REFR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                grupScheme.add(TES4RecordType.ACHR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                grupScheme.add(TES4RecordType.ACRE, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
                fileScheme.add(TES4RecordType.WRLD, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.DIAL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.DIAL, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.QUST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.QUST, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.IDLE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.IDLE, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.PACK, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.PACK, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.CSTY, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.CSTY, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LSCR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LSCR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.LVSP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.LVSP, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.ANIO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.ANIO, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.WATR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.WATR, grupScheme);
                grupScheme = new TES4GrupLoadScheme();
                grupScheme.add(TES4RecordType.EFSH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
                fileScheme.add(TES4RecordType.EFSH, grupScheme);
                collection.load(fileScheme);
                esm = collection;
            }

            if (this.scriptTypes == null)
            {
                this.scriptTypes = new Dictionary<string, ITES5Type>();
                List<TES4Grup> scpts = esm.getGrup(TES4RecordType.SCPT);
                foreach (ITES4Record scpt in scpts.SelectMany(s=>s.Select(r=>r)))
                {
                    string schr = scpt.getSubrecordString("SCHR");
                    string edid = scpt.getSubrecordTrimLower("EDID");
                    if (string.IsNullOrWhiteSpace(schr) || string.IsNullOrWhiteSpace(edid))
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

                    this.scriptTypes.Add(edid, scriptType);
                }
            }

            if (this.globals == null)
            {
                List<TES4Grup> globals = esm.getGrup(TES4RecordType.GLOB);
                List<TES5GlobalVariable> globalArray = new List<TES5GlobalVariable>();
                foreach (ITES4Record global in globals.SelectMany(g => g.Select(r => r)))
                {
                    string edid = global.getSubrecordTrim("EDID");
                    if (string.IsNullOrWhiteSpace(edid))
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
                this.globals = new TES5GlobalVariables(globalArray);
            }

            if (instance == null)
            {
                instance = this;
            }
        }
        
        public static ESMAnalyzer _instance()
        {
            if (instance == null)
            {
                return new ESMAnalyzer();
            }

            return instance;
        }

        public TES5GlobalVariables getGlobalVariables()
        {
            return this.globals;
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type getFormTypeByEDID(string edid)
        {
            TES4LoadedRecord record = esm.findByEDID(edid, false);
            if (record == null)
            {
                //WTM:  Change:  These EDIDs can't be found, so I've written them into the code.
                if (edid == "SE02FIN") { return TES5BasicType.T_QUEST; }
                if (edid == "LvlSpell") { return TES5BasicType.T_SPELL; }
                throw new ConversionException("Cannot find type for EDID " + edid);
            }
            return TypeMapper.map(record.getType());
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type getScriptType(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            ITES5Type value;
            if(this.scriptTypes.TryGetValue(scriptNameLower, out value)) { return value; }
            string scriptNameLowerNoPoundSymbols = scriptNameLower.Replace("#", "");
            if (this.scriptTypes.TryGetValue(scriptNameLowerNoPoundSymbols, out value)) { return value; }
            throw new ConversionException("Script " + scriptNameLower + " not found in ESM - cannot find its script type.");
        }

        /*
        * @todo REFACTOR, it"s really ugly!
        * @throws ConversionException
        */
        public ITES5Type resolveScriptTypeByItsAttachedName(string attachedName)
        {
            string attachedNameLower = attachedName.ToLower();
            ITES5Type value;
            if (this.attachedNameCache.TryGetValue(attachedNameLower, out value)) { return value; }
            TES4LoadedRecord attachedNameRecord = esm.findByEDID(attachedName, false);
            if (attachedNameRecord == null)
            {
                throw new ConversionException("Cannot resolve script type by searching its base form edid - no record found, " + attachedName);
            }
            TES4RecordType attachedNameRecordType = attachedNameRecord.getType();
            if (attachedNameRecordType == TES4RecordType.REFR || attachedNameRecordType == TES4RecordType.ACRE || attachedNameRecordType == TES4RecordType.ACHR)
            {
                //Resolve the reference
                Nullable<int> baseFormid = attachedNameRecord.getSubrecordAsFormid("NAME");
                attachedNameRecord = esm.findByFormid(baseFormid.Value);
            }

            Nullable<int> scriptFormid = attachedNameRecord.getSubrecordAsFormid("SCRI");
            if (scriptFormid == null)
            {
                throw new ConversionException("Cannot resolve script type for " + attachedName + " - Asked base record has no script bound.");
            }

            TES4LoadedRecord scriptRecord = esm.findByFormid(scriptFormid.Value);
            ITES5Type customType = TES5TypeFactory.memberByValue(scriptRecord.getSubrecordTrim("EDID"));
            this.attachedNameCache.Add(attachedNameLower, customType);
            return customType;
        }

        /*
        * Makes the adapter unusable by deallocating the esm object.
         * This really ought to be more clean, but until this class is used statically we have no other choice
        */
        public static void deallocate()
        {
            //Drop the ref
            esm = null;
            //Force the GC
            GC.Collect();
        }
    }
}