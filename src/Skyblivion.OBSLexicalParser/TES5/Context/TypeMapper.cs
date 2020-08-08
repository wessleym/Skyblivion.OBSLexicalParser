using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Context
{
    static class TypeMapper
    {
#if !ALTERNATE_TYPE_MAPPING
        //WTM:  Change:  Added
        public static TES5BasicType GetTES5BasicType(TES4RecordType recordType, TypeMapperMode mode, out bool returnedFormForCompatibility)
        {
            returnedFormForCompatibility = false;
            //For information about record types:  https://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
            //Activators, Containers, Doors, Statics, and Weapons seem to have the most problems with calling methods on them.  I'm leaving them as Forms.
            //Some of these records get converted to ObjectReferences if they are a specific variable that has problems with the type inferencer.  Please see ESMAnalyzer.forcedObjectReferenceExceptionsEDIDs.
            if (recordType == TES4RecordType.ACTI) { if (mode == TypeMapperMode.Strict) { return TES5BasicType.T_ACTIVATOR; } returnedFormForCompatibility = true; return TES5BasicType.T_FORM; }
            if (recordType == TES4RecordType.ALCH) { if (mode == TypeMapperMode.CompatibilityForReferenceFactory) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_POTION; }
            if (recordType == TES4RecordType.APPA) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_APPARATUS; }
            if (recordType == TES4RecordType.AMMO) { return TES5BasicType.T_AMMO; }
            if (recordType == TES4RecordType.ARMO) { if (mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.BOOK) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_BOOK; }
            if (recordType == TES4RecordType.CELL) { return TES5BasicType.T_CELL; }
            if (recordType == TES4RecordType.CLAS) { return TES5BasicType.T_CLASS; }
            if (recordType == TES4RecordType.CLOT) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForReferenceFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.CREA) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.CONT) { if (mode == TypeMapperMode.Strict) { return TES5BasicType.T_CONTAINER; } returnedFormForCompatibility = true; return TES5BasicType.T_FORM; }
            if (recordType == TES4RecordType.CSTY) { return TES5BasicType.T_COMBATSTYLE; }
            if (recordType == TES4RecordType.DIAL) { return TES5BasicType.T_TOPIC; }
            if (recordType == TES4RecordType.DOOR) { if (mode == TypeMapperMode.Strict) { return TES5BasicType.T_DOOR; } returnedFormForCompatibility = true; return TES5BasicType.T_FORM; }
            if (recordType == TES4RecordType.EFSH) { return TES5BasicType.T_EFFECTSHADER; }
            if (recordType == TES4RecordType.FACT) { return TES5BasicType.T_FACTION; }
            if (recordType == TES4RecordType.FLOR) { if (mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_FLORA; }
            if (recordType == TES4RecordType.FURN) { if (mode == TypeMapperMode.CompatibilityForReferenceFactory || mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_FURNITURE; }
            if (recordType == TES4RecordType.INGR) { if (mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_INGREDIENT; }
            if (recordType == TES4RecordType.KEYM) { if (mode == TypeMapperMode.CompatibilityForReferenceFactory) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_KEY; }
            if (recordType == TES4RecordType.LIGH) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForReferenceFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_LIGHT; }
            if (recordType == TES4RecordType.LVLC) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForReferenceFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_LEVELEDACTOR; }
            if (recordType == TES4RecordType.LVLI) { return TES5BasicType.T_LEVELEDITEM; }
            if (recordType == TES4RecordType.MGEF) { return TES5BasicType.T_MAGICEFFECT; }
            if (recordType == TES4RecordType.MISC) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForReferenceFactory || mode == TypeMapperMode.CompatibilityForScripts) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_MISCOBJECT; }
            if (recordType == TES4RecordType.NPC_) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.TREE) { return TES5BasicType.T_FORM; }//Skyrim doesn't have a Tree type.
            if (recordType == TES4RecordType.PACK) { return TES5BasicType.T_PACKAGE; }
            if (recordType == TES4RecordType.QUST) { return TES5BasicType.T_QUEST; }
            if (recordType == TES4RecordType.RACE) { return TES5BasicType.T_RACE; }
            if (recordType == TES4RecordType.REFR) { return TES5BasicType.T_OBJECTREFERENCE; }
            if (recordType == TES4RecordType.SCPT) { return TES5BasicType.T_FORM; }//WTM:  Note:  Apparently only used by TrapHungerStatue05SCRIPT.timer
            if (recordType == TES4RecordType.SLGM) { return TES5BasicType.T_SOULGEM; }
            if (recordType == TES4RecordType.SOUN) { if (mode == TypeMapperMode.CompatibilityForPropertyFactory || mode == TypeMapperMode.CompatibilityForReferenceFactory) { returnedFormForCompatibility = true; return TES5BasicType.T_FORM; } return TES5BasicType.T_SOUND; }
            if (recordType == TES4RecordType.SPEL) { return TES5BasicType.T_SPELL; }
            if (recordType == TES4RecordType.STAT) { if (mode == TypeMapperMode.Strict) { return TES5BasicType.T_STATIC; } returnedFormForCompatibility = true; return TES5BasicType.T_FORM; }
            if (recordType == TES4RecordType.WTHR) { return TES5BasicType.T_WEATHER; }
            if (recordType == TES4RecordType.WEAP) { if (mode == TypeMapperMode.Strict) { return TES5BasicType.T_WEAPON; } returnedFormForCompatibility = true; return TES5BasicType.T_FORM; }
            if (recordType == TES4RecordType.WRLD) { return TES5BasicType.T_WORLDSPACE; }
            throw new ArgumentException(nameof(recordType) + " " + recordType.Name + " could not be converted to a " + nameof(TES5BasicType) + ".", nameof(recordType));
        }
#else
        //WTM:  Change:  Added
        public static TES5BasicType GetTES5BasicType(TES4RecordType recordType, out bool mayRevertToForm)
        {
            //For information about record types:  https://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
            //Activators, Containers, Doors, Statics, and Weapons seem to have the most problems with calling methods on them.  I'm leaving them as Forms.
            //Some of these records get converted to ObjectReferences if they are a specific variable that has problems with the type inferencer.  Please see ESMAnalyzer.forcedObjectReferenceExceptionsEDIDs.
            mayRevertToForm = false;
            if (recordType == TES4RecordType.ACHR) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.ACRE) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.ACTI) { mayRevertToForm = true; return TES5BasicType.T_ACTIVATOR; }
            if (recordType == TES4RecordType.ALCH) { mayRevertToForm = true; return TES5BasicType.T_POTION; }
            if (recordType == TES4RecordType.APPA) { mayRevertToForm = true; return TES5BasicType.T_APPARATUS; }
            if (recordType == TES4RecordType.AMMO) { return TES5BasicType.T_AMMO; }
            if (recordType == TES4RecordType.ARMO) { mayRevertToForm = true; return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.BOOK) { mayRevertToForm = true; return TES5BasicType.T_BOOK; }
            if (recordType == TES4RecordType.CELL) { return TES5BasicType.T_CELL; }
            if (recordType == TES4RecordType.CLAS) { return TES5BasicType.T_CLASS; }
            if (recordType == TES4RecordType.CLOT) { mayRevertToForm = true; return TES5BasicType.T_ARMOR; }
            if (recordType == TES4RecordType.CREA) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.CONT) { mayRevertToForm = true; return TES5BasicType.T_CONTAINER; }
            if (recordType == TES4RecordType.CSTY) { return TES5BasicType.T_COMBATSTYLE; }
            if (recordType == TES4RecordType.DIAL) { return TES5BasicType.T_TOPIC; }
            if (recordType == TES4RecordType.DOOR) { mayRevertToForm = true; return TES5BasicType.T_DOOR; }
            if (recordType == TES4RecordType.EFSH) { return TES5BasicType.T_EFFECTSHADER; }
            if (recordType == TES4RecordType.FACT) { return TES5BasicType.T_FACTION; }
            if (recordType == TES4RecordType.FLOR) { mayRevertToForm = true; return TES5BasicType.T_FLORA; }
            if (recordType == TES4RecordType.FURN) { mayRevertToForm = true; return TES5BasicType.T_FURNITURE; }
            if (recordType == TES4RecordType.INGR) { mayRevertToForm = true; return TES5BasicType.T_INGREDIENT; }
            if (recordType == TES4RecordType.KEYM) { mayRevertToForm = true; return TES5BasicType.T_KEY; }
            if (recordType == TES4RecordType.LIGH) { mayRevertToForm = true; return TES5BasicType.T_LIGHT; }
            if (recordType == TES4RecordType.LVLC) { mayRevertToForm = true; return TES5BasicType.T_LEVELEDACTOR; }
            if (recordType == TES4RecordType.LVLI) { return TES5BasicType.T_LEVELEDITEM; }
            if (recordType == TES4RecordType.MGEF) { return TES5BasicType.T_MAGICEFFECT; }
            if (recordType == TES4RecordType.MISC) { mayRevertToForm = true; return TES5BasicType.T_MISCOBJECT; }
            if (recordType == TES4RecordType.NPC_) { return TES5BasicType.T_ACTOR; }
            if (recordType == TES4RecordType.TREE) { return TES5BasicType.T_FORM; }//Skyrim doesn't have a Tree type.
            if (recordType == TES4RecordType.PACK) { return TES5BasicType.T_PACKAGE; }
            if (recordType == TES4RecordType.QUST) { return TES5BasicType.T_QUEST; }
            if (recordType == TES4RecordType.RACE) { return TES5BasicType.T_RACE; }
            if (recordType == TES4RecordType.REFR) { return TES5BasicType.T_OBJECTREFERENCE; }
            if (recordType == TES4RecordType.SCPT) { return TES5BasicType.T_FORM; }//WTM:  Note:  Apparently only used by TrapHungerStatue05SCRIPT.timer
            if (recordType == TES4RecordType.SLGM) { return TES5BasicType.T_SOULGEM; }
            if (recordType == TES4RecordType.SOUN) { mayRevertToForm = true; return TES5BasicType.T_SOUND; }
            if (recordType == TES4RecordType.SPEL) { return TES5BasicType.T_SPELL; }
            if (recordType == TES4RecordType.STAT) { mayRevertToForm = true; return TES5BasicType.T_STATIC; }
            if (recordType == TES4RecordType.WTHR) { return TES5BasicType.T_WEATHER; }
            if (recordType == TES4RecordType.WEAP) { mayRevertToForm = true; return TES5BasicType.T_WEAPON; }
            if (recordType == TES4RecordType.WRLD) { return TES5BasicType.T_WORLDSPACE; }
            throw new ArgumentException(nameof(recordType) + " " + recordType.Name + " could not be converted to a " + nameof(TES5BasicType) + ".", nameof(recordType));
        }
#endif
    }
}