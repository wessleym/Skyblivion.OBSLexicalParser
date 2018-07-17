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
        private static Lazy<TES4Collection> esmLazy = null;
        private static TES4Collection ESM => esmLazy.Value;
        private Lazy<Dictionary<string, ITES5Type>> scriptTypesLazy;
        private Dictionary<string, ITES5Type> ScriptTypes => scriptTypesLazy.Value;
        private Lazy<TES5GlobalVariables> globalVariablesLazy;
        public TES5GlobalVariables GlobalVariables => globalVariablesLazy.Value;
        private static ESMAnalyzer instance;
        private readonly Dictionary<string, ITES5Type> attachedNameCache = new Dictionary<string, ITES5Type>();
        public ESMAnalyzer(bool loadLazily, string dataFile = DataDirectory.TES4GameFileName)
        {
            if (esmLazy == null)
            {
                esmLazy = new Lazy<TES4Collection>(() => GetESM(dataFile));
            }
            LoadScriptTypes();
            LoadGlobalVariables();
            if (!loadLazily) { LoadLazyObjects(); }
            if (instance == null)
            {
                instance = this;
            }
        }

        private static TES4Collection GetESM(string dataFile)
        {
            TES4Collection collection = new TES4Collection(DataDirectory.GetESMDirectoryPath());
            collection.Add(dataFile);
            //NOTE - SCRI record load scheme is a copypasta, as in, i didnt check which records do actually might have SCRI
            //Doesnt really matter for other purposes than cleaniness
            TES4FileLoadScheme fileScheme = new TES4FileLoadScheme();
            TES4GrupLoadScheme grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.GMST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.GMST, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.GLOB, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.GLOB, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CLAS, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CLAS, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.FACT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.FACT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.HAIR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.HAIR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.EYES, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.EYES, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.RACE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.RACE, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SOUN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SOUN, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SKIL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SKIL, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.MGEF, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.MGEF, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SCPT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI", "SCHR" }));
            fileScheme.add(TES4RecordType.SCPT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LTEX, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LTEX, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.ENCH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.ENCH, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SPEL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SPEL, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.BSGN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.BSGN, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.ACTI, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.ACTI, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.APPA, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.APPA, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.ARMO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.ARMO, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.BOOK, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.BOOK, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CLOT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CLOT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CONT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CONT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.DOOR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.DOOR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.INGR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.INGR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LIGH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LIGH, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.MISC, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.MISC, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.STAT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.STAT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.GRAS, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.GRAS, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.TREE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.TREE, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.FLOR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.FLOR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.FURN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.FURN, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.WEAP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.WEAP, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.AMMO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.AMMO, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.NPC_, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.NPC_, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CREA, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CREA, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LVLC, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LVLC, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SLGM, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SLGM, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.KEYM, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.KEYM, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.ALCH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.ALCH, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SBSP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SBSP, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.SGST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.SGST, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LVLI, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LVLI, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.WTHR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.WTHR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CLMT, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CLMT, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.REGN, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.REGN, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CELL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI", "FULL" }));//WTM:  Change:  Added "Full" for use from GetInCellFactory.cs.
            grupScheme.Add(TES4RecordType.REFR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            grupScheme.Add(TES4RecordType.ACHR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            grupScheme.Add(TES4RecordType.ACRE, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            fileScheme.add(TES4RecordType.CELL, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.WRLD, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            grupScheme.Add(TES4RecordType.CELL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            grupScheme.Add(TES4RecordType.REFR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            grupScheme.Add(TES4RecordType.ACHR, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            grupScheme.Add(TES4RecordType.ACRE, new TES4RecordLoadScheme(new string[] { "EDID", "NAME" }));
            fileScheme.add(TES4RecordType.WRLD, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.DIAL, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.DIAL, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.QUST, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.QUST, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.IDLE, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.IDLE, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.PACK, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.PACK, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.CSTY, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.CSTY, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LSCR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LSCR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.LVSP, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.LVSP, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.ANIO, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.ANIO, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.WATR, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.WATR, grupScheme);
            grupScheme = new TES4GrupLoadScheme();
            grupScheme.Add(TES4RecordType.EFSH, new TES4RecordLoadScheme(new string[] { "EDID", "SCRI" }));
            fileScheme.add(TES4RecordType.EFSH, grupScheme);
            collection.Load(fileScheme);
            return collection;
        }

        private void LoadScriptTypes()
        {
            scriptTypesLazy = new Lazy<Dictionary<string, ITES5Type>>(() =>
              {
                  Dictionary<string, ITES5Type> scriptTypes = new Dictionary<string, ITES5Type>();
                  List<TES4Grup> scpts = ESM.GetGrup(TES4RecordType.SCPT);
                  foreach (ITES4Record scpt in scpts.SelectMany(s => s.Select(r => r)))
                  {
                      string schr = scpt.GetSubrecordString("SCHR");
                      string edid = scpt.GetSubrecordTrimLower("EDID");
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
                      scriptTypes.Add(edid, scriptType);
                  }
                  return scriptTypes;
              });
        }

        private void LoadGlobalVariables()
        {
            globalVariablesLazy = new Lazy<TES5GlobalVariables>(() =>
              {
                  List<TES5GlobalVariable> globalArray = new List<TES5GlobalVariable>();
                  List<TES4Grup> globals = ESM.GetGrup(TES4RecordType.GLOB);
                  foreach (ITES4Record global in globals.SelectMany(g => g.Select(r => r)))
                  {
                      string edid = global.GetSubrecordTrim("EDID");
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
                  return new TES5GlobalVariables(globalArray);
              });
        }

        /*
         * This loads the lazies so that ESM data will be immediately available later.
         * If this method isn't called, the first attempt to access these properties will result in loading the ESM, which is time consuming.
         * This greatly improves the performance of TestStageMap and BuildFileDeleteCommand.
        */
        private void LoadLazyObjects()
        {
            var scriptTypes = ScriptTypes;
            var globalVariables = GlobalVariables;
        }

        public static ESMAnalyzer GetInstance()
        {
            if (instance == null)
            {
                return new ESMAnalyzer(false);
            }
            return instance;
        }

        public TES4LoadedRecord FindInTES4Collection(string edid, bool throwException)
        {
            return ESM.FindByEDID(edid, false);
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type GetFormTypeByEDID(string edid)
        {
            TES4LoadedRecord record = FindInTES4Collection(edid, false);
            if (record == null)
            {
                //WTM:  Change:  These EDIDs can't be found, so I've written them into the code.
                if (edid == "SE02FIN") { return TES5BasicType.T_QUEST; }
                if (edid == "LvlSpell") { return TES5BasicType.T_SPELL; }
                throw new ConversionException("Cannot find type for EDID " + edid);
            }
            return TypeMapper.Map(record.RecordType);
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
            TES4LoadedRecord attachedNameRecord = FindInTES4Collection(attachedName, false);
            if (attachedNameRecord == null)
            {
                throw new ConversionException("Cannot resolve script type by searching its base form edid - no record found, " + attachedName);
            }
            TES4RecordType attachedNameRecordType = attachedNameRecord.RecordType;
            if (attachedNameRecordType == TES4RecordType.REFR || attachedNameRecordType == TES4RecordType.ACRE || attachedNameRecordType == TES4RecordType.ACHR)
            {
                //Resolve the reference
                Nullable<int> baseFormid = attachedNameRecord.GetSubrecordAsFormid("NAME");
                attachedNameRecord = ESM.FindByFormid(baseFormid.Value);
            }

            Nullable<int> scriptFormid = attachedNameRecord.GetSubrecordAsFormid("SCRI");
            if (scriptFormid == null)
            {
                throw new ConversionException("Cannot resolve script type for " + attachedName + " - Asked base record has no script bound.");
            }

            TES4LoadedRecord scriptRecord = ESM.FindByFormid(scriptFormid.Value);
            ITES5Type customType = TES5TypeFactory.MemberByValue(scriptRecord.GetSubrecordTrim("EDID"));
            this.attachedNameCache.Add(attachedNameLower, customType);
            return customType;
        }

        /*
        * Makes the adapter unusable by deallocating the esm object.
         * This really ought to be more clean, but until this class is used statically we have no other choice
        */
        public static void Deallocate()
        {
            //Drop the ref
            esmLazy = null;
            //Force the GC
            GC.Collect();
        }
    }
}