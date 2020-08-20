using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Context
{
    static class TypeMapper//WTM:  Change:  Added
    {
        public static TES5BasicType GetTES5BasicType(TES4RecordType recordType)
        {
            //For information about record types:  https://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
            if (recordType == TES4RecordType.ACHR) { return TES5BasicType.T_ACTOR; }//Placed NPC_
            if (recordType == TES4RecordType.ACRE) { return TES5BasicType.T_ACTOR; }//Placed CREA
            if (recordType == TES4RecordType.ACTI) { return TES5BasicType.T_ACTIVATOR; }
            if (recordType == TES4RecordType.ALCH) { return TES5BasicType.T_POTION; }
            if (recordType == TES4RecordType.APPA) { return TES5BasicType.T_APPARATUS; }
            if (recordType == TES4RecordType.AMMO) { return TES5BasicType.T_AMMO; }
            if (recordType == TES4RecordType.ARMO) { return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.BOOK) { return TES5BasicType.T_BOOK; }
            if (recordType == TES4RecordType.CELL) { return TES5BasicType.T_CELL; }
            if (recordType == TES4RecordType.CLAS) { return TES5BasicType.T_CLASS; }
            if (recordType == TES4RecordType.CLOT) { return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.CREA) { return TES5BasicType.T_ACTORBASE; }
            if (recordType == TES4RecordType.CONT) { return TES5BasicType.T_CONTAINER; }
            if (recordType == TES4RecordType.CSTY) { return TES5BasicType.T_COMBATSTYLE; }
            if (recordType == TES4RecordType.DIAL) { return TES5BasicType.T_TOPIC; }
            if (recordType == TES4RecordType.DOOR) { return TES5BasicType.T_DOOR; }
            if (recordType == TES4RecordType.EFSH) { return TES5BasicType.T_EFFECTSHADER; }
            if (recordType == TES4RecordType.FACT) { return TES5BasicType.T_FACTION; }
            if (recordType == TES4RecordType.FLOR) { return TES5BasicType.T_FLORA; }
            if (recordType == TES4RecordType.FURN) { return TES5BasicType.T_FURNITURE; }
            if (recordType == TES4RecordType.GLOB) { return TES5BasicType.T_GLOBALVARIABLE; }
            if (recordType == TES4RecordType.INGR) { return TES5BasicType.T_INGREDIENT; }
            if (recordType == TES4RecordType.KEYM) { return TES5BasicType.T_KEY; }
            if (recordType == TES4RecordType.LIGH) { return TES5BasicType.T_LIGHT; }
            if (recordType == TES4RecordType.LVLC) { return TES5BasicType.T_LEVELEDACTOR; }
            if (recordType == TES4RecordType.LVLI) { return TES5BasicType.T_LEVELEDITEM; }
            if (recordType == TES4RecordType.MGEF) { return TES5BasicType.T_MAGICEFFECT; }
            if (recordType == TES4RecordType.MISC) { return TES5BasicType.T_MISCOBJECT; }
            if (recordType == TES4RecordType.NPC_) { return TES5BasicType.T_ACTORBASE; }
            if (recordType == TES4RecordType.TREE) { return TES5BasicType.T_FORM; }//Skyrim doesn't have a Tree type.
            if (recordType == TES4RecordType.PACK) { return TES5BasicType.T_PACKAGE; }
            if (recordType == TES4RecordType.QUST) { return TES5BasicType.T_QUEST; }
            if (recordType == TES4RecordType.RACE) { return TES5BasicType.T_RACE; }
            if (recordType == TES4RecordType.REFR) { return TES5BasicType.T_OBJECTREFERENCE; }
            if (recordType == TES4RecordType.SCPT) { return TES5BasicType.T_FORM; }//WTM:  Note:  Apparently only used by TrapHungerStatue05SCRIPT.timer
            if (recordType == TES4RecordType.SLGM) { return TES5BasicType.T_SOULGEM; }
            if (recordType == TES4RecordType.SOUN) { return TES5BasicType.T_SOUND; }
            if (recordType == TES4RecordType.SPEL) { return TES5BasicType.T_SPELL; }
            if (recordType == TES4RecordType.STAT) { return TES5BasicType.T_STATIC; }
            if (recordType == TES4RecordType.WTHR) { return TES5BasicType.T_WEATHER; }
            if (recordType == TES4RecordType.WEAP) { return TES5BasicType.T_WEAPON; }
            if (recordType == TES4RecordType.WRLD) { return TES5BasicType.T_WORLDSPACE; }
            throw new ArgumentException(nameof(recordType) + " " + recordType.Name + " could not be converted to a " + nameof(TES5BasicType) + ".", nameof(recordType));
        }
    }
}