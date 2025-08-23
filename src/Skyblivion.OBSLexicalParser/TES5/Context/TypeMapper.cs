using Skyblivion.ESReader;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Context
{
    static class TypeMapper//WTM:  Change:  Added
    {
        public static TES5BasicType GetTES5BasicType(ESRecordType recordType)
        {
            //For information about record types:  https://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
            if (recordType == ESRecordType.ACHR) { return TES5BasicType.T_ACTOR; }//Placed NPC_
            if (recordType == TES4RecordType.ACRE) { return TES5BasicType.T_ACTOR; }//Placed CREA
            if (recordType == ESRecordType.ACTI) { return TES5BasicType.T_ACTIVATOR; }
            if (recordType == ESRecordType.ALCH) { return TES5BasicType.T_POTION; }
            if (recordType == ESRecordType.APPA) { return TES5BasicType.T_APPARATUS; }
            if (recordType == ESRecordType.AMMO) { return TES5BasicType.T_AMMO; }
            if (recordType == ESRecordType.ARMO) { return TES5BasicType.T_ARMOR; }
            if (recordType == ESRecordType.BOOK) { return TES5BasicType.T_BOOK; }
            if (recordType == ESRecordType.CELL) { return TES5BasicType.T_CELL; }
            if (recordType == ESRecordType.CLAS) { return TES5BasicType.T_CLASS; }
            if (recordType == TES4RecordType.CLOT) { return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.CREA) { return TES5BasicType.T_ACTORBASE; }
            if (recordType == ESRecordType.CONT) { return TES5BasicType.T_CONTAINER; }
            if (recordType == ESRecordType.CSTY) { return TES5BasicType.T_COMBATSTYLE; }
            if (recordType == ESRecordType.DIAL) { return TES5BasicType.T_TOPIC; }
            if (recordType == ESRecordType.DOOR) { return TES5BasicType.T_DOOR; }
            if (recordType == ESRecordType.EFSH) { return TES5BasicType.T_EFFECTSHADER; }
            if (recordType == ESRecordType.FACT) { return TES5BasicType.T_FACTION; }
            if (recordType == ESRecordType.FLOR) { return TES5BasicType.T_FLORA; }
            if (recordType == ESRecordType.FURN) { return TES5BasicType.T_FURNITURE; }
            if (recordType == ESRecordType.GLOB) { return TES5BasicType.T_GLOBALVARIABLE; }
            if (recordType == ESRecordType.INGR) { return TES5BasicType.T_INGREDIENT; }
            if (recordType == ESRecordType.KEYM) { return TES5BasicType.T_KEY; }
            if (recordType == ESRecordType.LIGH) { return TES5BasicType.T_LIGHT; }
            if (recordType == TES4RecordType.LVLC) { return TES5BasicType.T_LEVELEDACTOR; }
            if (recordType == ESRecordType.LVLI) { return TES5BasicType.T_LEVELEDITEM; }
            if (recordType == ESRecordType.MGEF) { return TES5BasicType.T_MAGICEFFECT; }
            if (recordType == ESRecordType.MISC) { return TES5BasicType.T_MISCOBJECT; }
            if (recordType == ESRecordType.NPC_) { return TES5BasicType.T_ACTORBASE; }
            if (recordType == ESRecordType.TREE) { return TES5BasicType.T_FORM; }//Skyrim doesn't have a Tree type.
            if (recordType == ESRecordType.PACK) { return TES5BasicType.T_PACKAGE; }
            if (recordType == ESRecordType.QUST) { return TES5BasicType.T_QUEST; }
            if (recordType == ESRecordType.RACE) { return TES5BasicType.T_RACE; }
            if (recordType == ESRecordType.REFR) { return TES5BasicType.T_OBJECTREFERENCE; }
            if (recordType == TES4RecordType.SCPT) { return TES5BasicType.T_FORM; }//WTM:  Note:  Apparently only used by TrapHungerStatue05SCRIPT.timer
            if (recordType == ESRecordType.SLGM) { return TES5BasicType.T_SOULGEM; }
            if (recordType == ESRecordType.SOUN) { return TES5BasicType.T_SOUND; }
            if (recordType == ESRecordType.SPEL) { return TES5BasicType.T_SPELL; }
            if (recordType == ESRecordType.STAT) { return TES5BasicType.T_STATIC; }
            if (recordType == ESRecordType.WTHR) { return TES5BasicType.T_WEATHER; }
            if (recordType == ESRecordType.WEAP) { return TES5BasicType.T_WEAPON; }
            if (recordType == ESRecordType.WRLD) { return TES5BasicType.T_WORLDSPACE; }
            throw new ArgumentException(nameof(recordType) + " " + recordType.Name + " could not be converted to a " + nameof(TES5BasicType) + ".", nameof(recordType));
        }
    }
}