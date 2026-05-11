using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    static class TES5InheritanceGraphAnalyzer
    {
        private static readonly Dictionary<TES5BasicType, TES5BasicType> inheritanceCache = new Dictionary<TES5BasicType, TES5BasicType>();
        //WTM:  Note:  This includes some SKSE functions:  http://skse.silverlock.org/vanilla_commands.html
        private static readonly TES5InheritanceItemCollection inheritance = new TES5InheritanceItemCollection()
        {
            { TES5BasicType.T_ALIAS,
                new TES5InheritanceItemCollection()
                {
                    new TES5InheritanceItem(TES5BasicType.T_REFERENCEALIAS), new TES5InheritanceItem(TES5BasicType.T_LOCATIONALIAS)
                }
            },
            { TES5BasicType.T_UTILITY, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_ACTIVEMAGICEFFECT, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_DEBUG, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_GAME, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_MAIN, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_MATH, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_FORM, new TES5InheritanceItemCollection()
                {
                    { TES5BasicType.T_ACTION },
                    { TES5BasicType.T_MAGICEFFECT },
                    { TES5BasicType.T_ACTIVATOR, new TES5InheritanceItemCollection(){TES5BasicType.T_FURNITURE,TES5BasicType.T_FLORA,TES5BasicType.T_TALKINGACTIVATOR } },
                    { TES5BasicType.T_MESSAGE },
                    { TES5BasicType.T_ACTORBASE },
                    { TES5BasicType.T_MISCOBJECT, new TES5InheritanceItemCollection(){
                        TES5BasicType.T_APPARATUS,
                        TES5BasicType.T_CONSTRUCTIBLEOBJECT,
                        TES5BasicType.T_KEY,
                        TES5BasicType.T_SOULGEM
                    } },
                    { TES5BasicType.T_AMMO },
                    { TES5BasicType.T_ARMOR },
                    { TES5BasicType.T_ARMORADDON },
                    { TES5BasicType.T_ASSOCIATIONTYPE },
                    { TES5BasicType.T_MUSICTYPE },
                    { TES5BasicType.T_BOOK },
                    { TES5BasicType.T_OBJECTREFERENCE, new TES5InheritanceItemCollection(){
                        TES5BasicType.T_ACTOR
                    } },
                    { TES5BasicType.T_CELL },
                    { TES5BasicType.T_CLASS },
                    { TES5BasicType.T_OUTFIT },
                    { TES5BasicType.T_COLORFORM },
                    { TES5BasicType.T_PACKAGE },
                    { TES5BasicType.T_COMBATSTYLE },
                    { TES5BasicType.T_CONTAINER },
                    { TES5BasicType.T_PERK },
                    { TES5BasicType.T_DOOR },
                    { TES5BasicType.T_POTION },
                    { TES5BasicType.T_EFFECTSHADER },
                    { TES5BasicType.T_PROJECTILE },
                    { TES5BasicType.T_ENCHANTMENT },
                    { TES5BasicType.T_QUEST, new TES5InheritanceItemCollection(){
                        TES5BasicType.T_SKYBTIMERHELPER,
                        TES5BasicType.T_SKYBCONTAINER
                    } },
                    { TES5BasicType.T_ENCOUNTERZONE },
                    { TES5BasicType.T_RACE },
                    { TES5BasicType.T_EQUIPSLOT },
                    { TES5BasicType.T_SCENE },
                    { TES5BasicType.T_EXPLOSION },
                    { TES5BasicType.T_FACTION },
                    { TES5BasicType.T_FORMLIST },
                    { TES5BasicType.T_SCROLL },
                    { TES5BasicType.T_GLOBALVARIABLE },
                    { TES5BasicType.T_SHOUT },
                    { TES5BasicType.T_HAZARD },
                    { TES5BasicType.T_SOUND },
                    { TES5BasicType.T_HEADPART },
                    { TES5BasicType.T_SOUNDCATEGORY },
                    { TES5BasicType.T_IDLE },
                    { TES5BasicType.T_SPELL },
                    { TES5BasicType.T_IMAGESPACEMODIFIER },
                    { TES5BasicType.T_STATIC },
                    { TES5BasicType.T_IMPACTDATASET },
                    { TES5BasicType.T_TEXTURESET },
                    { TES5BasicType.T_INGREDIENT },
                    { TES5BasicType.T_TOPIC },
                    { TES5BasicType.T_KEYWORD, new TES5InheritanceItemCollection(){
                        TES5BasicType.T_LOCATIONREFTYPE
                    } },
                    { TES5BasicType.T_TOPICINFO },
                    { TES5BasicType.T_LEVELEDACTOR },
                    { TES5BasicType.T_VISUALEFFECT },
                    { TES5BasicType.T_LEVELEDITEM },
                    { TES5BasicType.T_VOICETYPE },
                    { TES5BasicType.T_LEVELEDSPELL },
                    { TES5BasicType.T_WEAPON },
                    { TES5BasicType.T_LIGHT },
                    { TES5BasicType.T_WEATHER },
                    { TES5BasicType.T_LOCATION },
                    { TES5BasicType.T_WORDOFPOWER },
                    { TES5BasicType.T_WORLDSPACE },
                    { TES5BasicType.T_ART },//WTM:  Change:  Added
                    { TES5BasicType.T_SOUNDDESCRIPTOR }//WTM:  Change:  Added
                }
            },
            { TES5BasicType.T_INPUT, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_SKSE, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_STRINGUTIL, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_UI, new TES5InheritanceItemCollection() },
            { TES5BasicType.T_SKYBGameUtility, new TES5InheritanceItemCollection() }
        };
        private static readonly TES5InheritanceItem inheritanceAsItem = new TES5InheritanceItem(null, inheritance);

        //Regular Expression used to build tree from PHP:  ("[^"]+") =>[\r\n\s]+new string\[\] \{\r\n\s+"args" =>[\r\n\s]+(new string\[\] \{[^\}]*\}),[\r\n\s]+"returnType" => ("[^"]+"),?[\r\n\s]*}
        private static readonly Dictionary<TES5BasicType, TES5InheritanceFunctionSignature[]> callReturns = new Dictionary<TES5BasicType, TES5InheritanceFunctionSignature[]>()
        {
            { TES5BasicType.T_ACTIVEMAGICEFFECT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Dispel", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBaseObject", [], TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCasterActor", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetTargetActor", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDuration", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTimeElapsed", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("RegisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("OnUpdate", [], TES5VoidType.Instance),//WTM:  Change:  Added
                new TES5InheritanceFunctionSignature("GotoState", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance)//WTM:  Change:  Added
                }
            },
        { TES5BasicType.T_ACTOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ModFavorPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModFavorPointsWithGlobal", new TES5BasicType[] {
                                TES5BasicType.T_GLOBALVARIABLE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakePlayerFriend", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddPerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddShout", new TES5BasicType[] {
                                TES5BasicType.T_SHOUT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("AddSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("AllowBleedoutDialogue", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AllowPCDialogue", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AttachAshPile", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanFlyHere", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ClearArrested", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearExpressionOverride", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearExtraArrows", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearForcedLandingMarker", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearKeepOffsetFromActor", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearLookAt", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Dismount", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("DispelAllSpells", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DispelSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("DoCombatSpellApply", new TES5BasicType[] {
                                TES5BasicType.T_SPELL,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EnableAI", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EndDeferredKill", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EquipItem", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EquipShout", new TES5BasicType[] {
                                TES5BasicType.T_SHOUT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EquipSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EvaluatePackage", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorBase", [], TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetActorValuePercentage", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAVPercentage", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetBaseActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetBaseAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetBribeAmount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeFaction", [], TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("GetCombatState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCombatTarget", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetCurrentPackage", [], TES5BasicType.T_PACKAGE),
                new TES5InheritanceFunctionSignature("GetDialogueTarget", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetEquippedItemType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetEquippedShout", [], TES5BasicType.T_SHOUT),
                new TES5InheritanceFunctionSignature("GetEquippedWeapon", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_WEAPON),
                new TES5InheritanceFunctionSignature("GetEquippedShield", [], TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("GetEquippedSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetFactionRank", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetFactionReaction", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetFlyingState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetForcedLandingMarker", [], TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetGoldAmount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetHighestRelationshipRank", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetKiller", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetLevel", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetLightLevel", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetLowestRelationshipRank", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetLeveledActorBase", [], TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetNoBleedoutRecovery", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetPlayerControls", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetRace", [], TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetRelationshipRank", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSitState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSleepState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVoiceRecoveryTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("HasAssociation", new TES5BasicType[] {
                                TES5BasicType.T_ASSOCIATIONTYPE,
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasFamilyRelationship", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasLOS", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_MAGICEFFECT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasMagicEffectWithKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasParentRelationship", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasPerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasSpell", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsAlarmed", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsAlerted", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsAllowedToFly", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsArrested", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsArrestingTarget", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBeingRidden", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBleedingOut", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBribed", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsChild", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCommandedActor", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDead", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDetectedBy", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDoingFavor", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEquipped", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEssential", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFlying", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGuard", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGhost", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHostileToActor", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInCombat", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInKillMove", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsIntimidated", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsOnMount", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayersLastRiddenHorse", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerTeammate", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsRunning", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSneaking", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSprinting", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsTrespassing", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsUnconscious", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWeaponDrawn", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("KeepOffsetFromActor", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Kill", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("KillEssential", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("KillSilent", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModFactionRank", new TES5BasicType[] {
                                TES5BasicType.T_FACTION,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MoveToPackageLocation", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("OpenInventory", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PathToReference", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayIdle", new TES5BasicType[] {
                                TES5BasicType.T_IDLE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayIdleWithTarget", new TES5BasicType[] {
                                TES5BasicType.T_IDLE,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlaySubGraphAnimation", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveFromFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveFromAllFactions", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemovePerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveShout", new TES5BasicType[] {
                                TES5BasicType.T_SHOUT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RemoveSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ResetHealthAndLimbs", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RestoreActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Resurrect", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RestoreAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendTrespassAlarm", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAlert", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAllowFlying", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAllowFlyingEx", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAlpha", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAttackActorOnSight", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetBribed", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCrimeFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCriticalStage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetDoingFavor", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetDontMove", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetExpressionOverride", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetEyeTexture", new TES5BasicType[] {
                                TES5BasicType.T_TEXTURESET
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFactionRank", new TES5BasicType[] {
                                TES5BasicType.T_FACTION,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetForcedLandingMarker", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGhost", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddToFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetHeadTracking", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetIntimidated", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetLookAt", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNoBleedoutRecovery", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNotShowOnStealthMeter", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetOutfit", new TES5BasicType[] {
                                TES5BasicType.T_OUTFIT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerControls", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerResistingArrest", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerTeammate", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetRace", new TES5BasicType[] {
                                TES5BasicType.T_RACE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetRelationshipRank", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetRestrained", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetSubGraphFloatVariable", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetUnconscious", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetVehicle", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetVoiceRecoveryTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowBarterMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowGiftMenu", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("StartCannibal", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartCombat", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartDeferredKill", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartVampireFeed", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopCombat", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopCombatAlarm", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TrapSoul", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("UnequipAll", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnequipItem", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnequipItemSlot", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnequipShout", new TES5BasicType[] {
                                TES5BasicType.T_SHOUT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnequipSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnLockOwnedDoorsInCell", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("WillIntimidateSucceed", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("WornHasKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("StartSneaking", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DrawWeapon", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementDirection", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementSpeed", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementRotationSpeed", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementDirectionRamp", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementSpeedRamp", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceMovementRotationSpeedRamp", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceTargetDirection", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceTargetSpeed", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceTargetAngle", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearForcedMovement", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWornForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetWornItemId", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetEquippedObject", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetEquippedItemId", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSpellCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("QueueNiNodeUpdate", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegenerateHead", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EquipItemEx", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EquipItemById", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnequipItemEx", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ChangeHeadPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateWeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsAIEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSwimming", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SheatheWeapon", [], TES5VoidType.Instance),
                /*new TES5InheritanceFunctionSignature("ForceFlee", new TES5BasicType[] {
                                TES5BasicType.T_CELL,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),//WTM:  Note:  I think Aerisarn was going to add this, possibly utilizing SKSE's code.  But since it isn't ready, I'm commenting this for now.
                new TES5InheritanceFunctionSignature("OBStartConversation", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_TOPIC
                            }, TES5VoidType.Instance),//WTM:  Note:  I think Aerisarn was going to add this.  But since it isn't ready, I'm commenting this for now.
                new TES5InheritanceFunctionSignature("GetRestrained", [], TES5BasicType.T_BOOL),//WTM:  Note:  This function was added to our Actor.psc file as "native," but the SKSE plugin does not implement it.
                new TES5InheritanceFunctionSignature("IsTorchOut" ,[], TES5BasicType.T_BOOL),//WTM:  Note:  This function was added to our Actor.psc file as "native," but the SKSE plugin does not implement it.*/
            }
        },
        { TES5BasicType.T_ACTORBASE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetClass", [], TES5BasicType.T_CLASS),
                new TES5InheritanceFunctionSignature("GetDeadCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGiftFilter", [], TES5BasicType.T_FORMLIST),
                new TES5InheritanceFunctionSignature("GetRace", [], TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetSex", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsEssential", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInvulnerable", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsProtected", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsUnique", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetEssential", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetInvulnerable", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetProtected", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetOutfit", new TES5BasicType[] {
                                TES5BasicType.T_OUTFIT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCombatStyle", [], TES5BasicType.T_COMBATSTYLE),
                new TES5InheritanceFunctionSignature("SetCombatStyle", new TES5BasicType[] {
                                TES5BasicType.T_COMBATSTYLE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetOutfit", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_OUTFIT),
                new TES5InheritanceFunctionSignature("SetClass", new TES5BasicType[] {
                                TES5BasicType.T_CLASS
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetHeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetWeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumHeadParts", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthHeadPart", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_HEADPART),
                new TES5InheritanceFunctionSignature("SetNthHeadPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIndexOfHeadPartByType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetFaceMorph", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetFaceMorph", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFacePreset", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetFacePreset", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHairColor", [], TES5BasicType.T_COLORFORM),
                new TES5InheritanceFunctionSignature("SetHairColor", new TES5BasicType[] {
                                TES5BasicType.T_COLORFORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSpellCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetFaceTextureSet", [], TES5BasicType.T_TEXTURESET),
                new TES5InheritanceFunctionSignature("SetFaceTextureSet", new TES5BasicType[] {
                                TES5BasicType.T_TEXTURESET
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetVoiceType", [], TES5BasicType.T_VOICETYPE),
                new TES5InheritanceFunctionSignature("SetVoiceType", new TES5BasicType[] {
                                TES5BasicType.T_VOICETYPE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkin", [], TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkin", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkinFar", [], TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkinFar", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetTemplate", [], TES5BasicType.T_ACTORBASE)
            }
        },
        { TES5BasicType.T_ALIAS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", [], TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetName", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetID", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RegisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_APPARATUS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetQuality", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetQuality", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_ARMOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetArmorRating", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetAR", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetArmorRating", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAR", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModArmorRating", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModAR", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIconPath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMessageIconPath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetMessageIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeightClass", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetWeightClass", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantment", [], TES5BasicType.T_ENCHANTMENT),
                new TES5InheritanceFunctionSignature("SetEnchantment", new TES5BasicType[] {
                                TES5BasicType.T_ENCHANTMENT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsLightArmor", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHeavyArmor", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothing", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBoots", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCuirass", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGauntlets", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHelmet", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsShield", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsJewelry", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingHead", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingBody", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingFeet", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingHands", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingRing", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingRich", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingPoor", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetSlotMask", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSlotMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddSlotToMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RemoveSlotFromMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMaskForSlot", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNumArmorAddons", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthArmorAddon", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_ARMORADDON)
            }
        },
        { TES5BasicType.T_ARMORADDON,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModelNumTextureSets", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetModelNthTextureSet", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_TEXTURESET),
                new TES5InheritanceFunctionSignature("SetModelNthTextureSet", new TES5BasicType[] {
                                TES5BasicType.T_TEXTURESET,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumAdditionalRaces", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthAdditionalRace", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetSlotMask", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSlotMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddSlotToMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RemoveSlotFromMask", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMaskForSlot", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_BOOK,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSpell", [], TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetSkill", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsRead", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsTakeable", [], TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_CELL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetActorOwner", [], TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetFactionOwner", [], TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("IsAttached", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInterior", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Reset", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActorOwner", new TES5BasicType[] {
                                TES5BasicType.T_ACTORBASE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFactionOwner", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFogPlanes", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFogPower", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPublic", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumRefs", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthRef", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_OBJECTREFERENCE)
            }
        },
        /* Removed due to GetValue() conflict, i will think how to readd this . todo,
        { TES5BasicType.T_COLORCOMPONENT, 
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetAlpha", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetRed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGreen", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetBlue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetHue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSaturation", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetValue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetAlpha", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetRed", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetGreen", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetBlue", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetHue", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSaturation", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetValue", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_COLORFORM, 
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetColor", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetColor", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetRed", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGreen", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetBlue", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetHue", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSaturation", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetValue", [], TES5BasicType.T_FLOAT)
            },*/
        { TES5BasicType.T_COMBATSTYLE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOffensiveMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetDefensiveMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetGroupOffensiveMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAvoidThreatChance", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetRangedMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMagicMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetShoutMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetStaffMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetUnarmedMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetOffensiveMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetDefensiveMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGroupOffensiveMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAvoidThreatChance", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetRangedMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMagicMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetShoutMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetStaffMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetUnarmedMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMeleeAttackStaggeredMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackStaggeredMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackBlockingMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashRecoiledMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashAttackMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashPowerAttackMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeSpecialAttackMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAllowDualWielding", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetMeleeAttackStaggeredMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleePowerAttackStaggeredMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleePowerAttackBlockingMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeBashMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeBashRecoiledMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeBashAttackMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeBashPowerAttackMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMeleeSpecialAttackMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAllowDualWielding", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingCircleMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingFallbackMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingFlankDistance", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingStalkTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetCloseRangeDuelingCircleMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCloseRangeDuelingFallbackMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCloseRangeFlankingFlankDistance", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCloseRangeFlankingStalkTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetLongRangeStrafeMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetLongRangeStrafeMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFlightHoverChance", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetFlightDiveBombChance", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetFlightFlyingAttackChance", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetFlightHoverChance", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFlightDiveBombChance", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFlightFlyingAttackChance", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_CONSTRUCTIBLEOBJECT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetResult", [], TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("SetResult", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResultQuantity", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetResultQuantity", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumIngredients", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthIngredient", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("SetNthIngredient", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNthIngredientQuantity", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthIngredientQuantity", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWorkbenchKeyword", [], TES5BasicType.T_KEYWORD),
                new TES5InheritanceFunctionSignature("SetWorkbenchKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5VoidType.Instance)
            }
        },
        /*{ "DwarvenMechScript",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetFormIndex", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_INT)
            }
        },*///WTM:  Change:  Commented
        { TES5BasicType.T_ENCHANTMENT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNumEffects", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", [], TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_EQUIPSLOT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParents", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthParent", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_EQUIPSLOT)
            }
        },
        { TES5BasicType.T_FLORA,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHarvestSound", [], TES5BasicType.T_SOUNDDESCRIPTOR),
                new TES5InheritanceFunctionSignature("SetHarvestSound", new TES5BasicType[] {
                               TES5BasicType.T_SOUNDDESCRIPTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIngredient", [], TES5BasicType.T_INGREDIENT),
                new TES5InheritanceFunctionSignature("SetIngredient", new TES5BasicType[] {
                                TES5BasicType.T_INGREDIENT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_FORM,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetFormID", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGoldValue", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("HasKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayerKnows", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetType", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetName", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetName", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetWeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGoldValue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumKeywords", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthKeyword", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_KEYWORD),
                new TES5InheritanceFunctionSignature("HasKeywordString", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RegisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TempClone", [], TES5BasicType.T_FORM)
            }
        },
        /*{ "FormType", [] },*///WTM:  Change:  Commented
        { TES5BasicType.T_GAME,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddAchievement", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddPerkPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AdvanceSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddHavokBallAndSocketConstraint", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RemoveHavokConstraints", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("CalculateFavorCost", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("ClearPrison", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearTempEffects", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DisablePlayerControls", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EnableFastTravel", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FadeOutGame", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FastTravel", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfType", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfType", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfAnyTypeInList", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfAnyTypeInList", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfTypeFromRef", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfTypeFromRef", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfAnyTypeInListFromRef", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfAnyTypeInListFromRef", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("FindClosestActor", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("FindRandomActor", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("FindClosestActorFromRef", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("FindRandomActorFromRef", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("ForceThirdPerson", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceFirstPerson", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowFirstPersonGeometry", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetFormFromFile", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetGameSettingFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetGameSettingInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGameSettingString", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetPlayer", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetPlayerGrabbedRef", [], TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetPlayersLastRiddenHorse", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetSunPositionX", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunPositionY", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunPositionZ", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetRealHoursPassed", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("IncrementSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IncrementSkillBy", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IncrementStat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsActivateControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCamSwitchControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFastTravelControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFastTravelEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFightingControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsJournalControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLookingControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMenuControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMovementControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerSungazing", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSneakingControlsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWordUnlocked", new TES5BasicType[] {
                                TES5BasicType.T_WORDOFPOWER
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PrecacheCharGen", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PrecacheCharGenClear", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("QueryStat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("QuitToMainMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestAutoSave", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestModel", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestSave", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ServeTime", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendWereWolfTransformation", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetBeastForm", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCameraTarget", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetHudCartMode", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetInChargen", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerAIDriven", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerReportCrime", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetSittingRotation", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShakeCamera", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShakeController", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowRaceMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowLimitedRaceMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowTitleSequenceMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("HideTitleSequenceMenu", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartTitleSequence", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAllowFlyingMountLandingRequests", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetSunGazeImageSpaceModifier", new TES5BasicType[] {
                                TES5BasicType.T_IMAGESPACEMODIFIER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowTrainingMenu", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TeachWord", new TES5BasicType[] {
                                TES5BasicType.T_WORDOFPOWER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TriggerScreenBlood", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnlockWord", new TES5BasicType[] {
                                TES5BasicType.T_WORDOFPOWER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UsingGamepad", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetPerkPoints", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetPerkPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModPerkPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetModByName", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetModName", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetModAuthor", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetModDescription", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetModDependencyCount", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthModDependency", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetGameSettingFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGameSettingInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGameSettingBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGameSettingString", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SaveGame", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("LoadGame", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumTintMasks", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthTintMaskColor", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthTintMaskType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthTintMaskColor", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNthTintMaskTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetNthTintMaskTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumTintsByType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetTintMaskColor", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetTintMaskColor", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetTintMaskTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetTintMaskTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateTintMaskColors", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateHairColor", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCameraState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetMiscStat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayersLastRiddenHorse", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkillLegendaryLevel", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSkillLegendaryLevel", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetPlayerMovementMode", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("UpdateThirdPerson", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnbindObjectHotkey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHotkeyBoundObject", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("IsObjectFavorited", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayBink", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("EnablePlayerControls", new TES5BasicType[] {//WTM:  Change:  Added
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("LegacyGetAmountSoldStolen", [], TES5BasicType.T_INT),//WTM:  Change:  Added via Skyblivion.dll
                new TES5InheritanceFunctionSignature("LegacyModAmountSoldStolen", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),//WTM:  Change:  Added via Skyblivion.dll
                /*new TES5InheritanceFunctionSignature("IsPCAMurderer", [], TES5BasicType.T_INT),*/
                new TES5InheritanceFunctionSignature("LegacyIsPCAMurderer", [], TES5BasicType.T_INT)//WTM:  Change:  Added via Skyblivion.dll (experimenting)
            }
        },
        { TES5BasicType.T_HEADPART,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHeadPart", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_HEADPART),
                new TES5InheritanceFunctionSignature("GetType", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNumExtraParts", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_HEADPART),
                new TES5InheritanceFunctionSignature("HasExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetIndexOfExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetValidRaces", [], TES5BasicType.T_FORMLIST),
                new TES5InheritanceFunctionSignature("SetValidRaces", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_INGREDIENT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("LearnEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("LearnNextEffect", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LearnAllEffects", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumEffects", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_INPUT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsKeyPressed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TapKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("HoldKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ReleaseKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumKeysPressed", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthKeyPressed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMappedKey", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMappedControl", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING)
            }
        },
        { TES5BasicType.T_KEYWORD,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetKeyword", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_KEYWORD),
                new TES5InheritanceFunctionSignature("GetString", [], TES5BasicType.T_STRING)
            }
        },
        { TES5BasicType.T_MATH,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("abs", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("acos", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("asin", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("atan", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("Ceiling", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("cos", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("DegreesToRadians", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("Floor", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("pow", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("RadiansToDegrees", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("sin", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("sqrt", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("tan", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("LeftShift", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RightShift", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LogicalAnd", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LogicalOr", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LogicalXor", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LogicalNot", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_OBJECTREFERENCE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("rampRumble", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsNearPlayer", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInInterior", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MoveToIfUnloaded", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MoveToWhenUnloaded", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                /*new TES5InheritanceFunctionSignature("finishes", new TES5BasicType[] {
                                "and",
                                "the",
                                "1"
                            }, "this"),*///WTM:  Change:  Commented
                new TES5InheritanceFunctionSignature("DeleteWhenAble", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddKeyIfNeeded", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("get", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("Activate", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("AddDependentAnimatedObjectReference", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddItem", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddToMap", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ApplyHavokImpulse", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("BlockActivation", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CalculateEncounterLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("CanFastTravelToMarker", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ClearDestruction", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CreateDetectionEvent", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageObject", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Delete", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Disable", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DisableNoWait", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DropObject", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("Enable", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EnableFastTravel", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EnableNoWait", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceAddRagdollToWorld", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceRemoveRagdollFromWorld", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorOwner", [], TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetAngleX", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAngleY", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAngleZ", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAnimationVariableBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetAnimationVariableInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetAnimationVariableFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetBaseObject", [], TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetCurrentDestructionStage", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCurrentLocation", [], TES5BasicType.T_LOCATION),
                new TES5InheritanceFunctionSignature("GetCurrentScene", [], TES5BasicType.T_SCENE),
                new TES5InheritanceFunctionSignature("GetDistance", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetEditorLocation", [], TES5BasicType.T_LOCATION),
                new TES5InheritanceFunctionSignature("GetEnableParent", [], TES5BasicType.T_OBJECTREFERENCE),//WTM:  Note:  SKSE
                new TES5InheritanceFunctionSignature("GetFactionOwner", [], TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("GetHeadingAngle", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetHeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetItemCount", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetItemHealthPercent", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetKey", [], TES5BasicType.T_KEY),
                new TES5InheritanceFunctionSignature("GetLength", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetLinkedRef", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetLockLevel", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("countLinkedRefChain", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthLinkedRef", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_OBJECTREFERENCE),
                /*new TES5InheritanceFunctionSignature("GetStartingAngle", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),//WTM:  Note:  I think Aerisarn was going to add this, possibly utilizing SKSE's code.  But since it isn't ready, I'm commenting this for now.
                new TES5InheritanceFunctionSignature("GetStartingPos", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),//WTM:  Note:  I think Aerisarn was going to add this, possibly utilizing SKSE's code.  But since it isn't ready, I'm commenting this for now.*/
                new TES5InheritanceFunctionSignature("EnableLinkChain", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DisableLinkChain", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMass", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetOpenState", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetParentCell", [], TES5BasicType.T_CELL),
                new TES5InheritanceFunctionSignature("GetPositionX", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPositionY", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPositionZ", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetScale", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTriggerObjectCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVoiceType", [], TES5BasicType.T_VOICETYPE),
                new TES5InheritanceFunctionSignature("GetWidth", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWorldSpace", [], TES5BasicType.T_WORLDSPACE),
                new TES5InheritanceFunctionSignature("GetSelfAsActor", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("HasEffectKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasNode", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasRefType", new TES5BasicType[] {
                                TES5BasicType.T_LOCATIONREFTYPE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IgnoreFriendlyHits", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InterruptCast", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsActivateChild", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsActivationBlocked", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Is3DLoaded", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDeleted", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDisabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEnabled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFurnitureInUse", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFurnitureMarkerInUse", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsIgnoringFriendlyHits", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInDialogueWithPlayer", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLockBroken", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLocked", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMapMarkerVisible", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("KnockAreaEffect", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Lock", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MoveTo", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MoveToInteractionLocation", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MoveToMyEditorLocation", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MoveToNode", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PlaceAtMe", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("PlaceActorAtMe", new TES5BasicType[] {
                                TES5BasicType.T_ACTORBASE,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_ENCOUNTERZONE
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("PlayAnimation", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayAnimationAndWait", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayGamebryoAnimation", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayImpactEffect", new TES5BasicType[] {
                                TES5BasicType.T_IMPACTDATASET,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlaySyncedAnimationSS", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlaySyncedAnimationAndWaitSS", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayTerrainEffect", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ProcessTrapHit", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PushActorAway", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveAllItems", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveItem", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveDependentAnimatedObjectReference", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Reset", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Say", new TES5BasicType[] {
                                TES5BasicType.T_TOPIC,
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendStealAlarm", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActorCause", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActorOwner", new TES5BasicType[] {
                                TES5BasicType.T_ACTORBASE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAngle", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAnimationVariableBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAnimationVariableInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetAnimationVariableFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetDestroyed", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFactionOwner", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetLockLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetMotionType", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNoFavorAllowed", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetOpen", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPosition", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetScale", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TranslateTo", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SplineTranslateTo", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SplineTranslateToRefNode", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopTranslation", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TranslateToRef", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SplineTranslateToRef", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TetherToHorse", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("WaitForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInLocation", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNumItems", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetTotalItemWeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTotalArmorWeight", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("IsHarvested", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetHarvested", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetItemHealthPercent", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetItemMaxCharge", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetItemCharge", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetItemCharge", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ResetInventory", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsOffLimits", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("OnUpdate", [], TES5VoidType.Instance),//WTM:  Change:  Added
                new TES5InheritanceFunctionSignature("GotoState", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),//WTM:  Change:  Added
                new TES5InheritanceFunctionSignature("SetDisplayName", new TES5BasicType[]
                            {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance)//WTM:  Change:  Added.  SKSE.
            }
        },
        { TES5BasicType.T_OUTFIT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParts", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthPart", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM)
            }
        },
        { TES5BasicType.T_PERK,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumEntries", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEntryRank", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEntryRank", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryPriority", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEntryPriority", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryQuest", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("SetNthEntryQuest", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_QUEST
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryStage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEntryStage", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntrySpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("SetNthEntrySpell", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_SPELL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryLeveledList", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_LEVELEDITEM),
                new TES5InheritanceFunctionSignature("SetNthEntryLeveledList", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_LEVELEDITEM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryText", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetNthEntryText", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNthEntryValue", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetNthEntryValue", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_POTION,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFood", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNumEffects", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_QUEST,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ModObjectiveGlobal", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_GLOBALVARIABLE,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("CompleteAllObjectives", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CompleteQuest", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FailAllObjectives", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetAlias", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_ALIAS),
                new TES5InheritanceFunctionSignature("GetCurrentStageID", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStage", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStageDone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsActive", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCompleted", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveCompleted", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveDisplayed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveFailed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsRunning", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStageDone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStarting", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStopping", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStopped", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PrepareForReinitializing", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Reset", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActive", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCurrentStageID", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetObjectiveCompleted", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetObjectiveDisplayed", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetObjectiveFailed", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetStage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Start", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Stop", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateCurrentInstanceGlobal", new TES5BasicType[] {
                                TES5BasicType.T_GLOBALVARIABLE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetQuest", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("GetID", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetPriority", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNumAliases", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthAlias", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_ALIAS),
                new TES5InheritanceFunctionSignature("GetAliasByName", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_ALIAS)
            }
        },
        { TES5BasicType.T_RACE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSpellCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("IsRaceFlagSet", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetRaceFlag", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearRaceFlag", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDefaultVoiceType", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_VOICETYPE),
                new TES5InheritanceFunctionSignature("SetDefaultVoiceType", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_VOICETYPE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkin", [], TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkin", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumPlayableRaces", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthPlayableRace", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetRace", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("IsPlayable", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakePlayable", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeUnplayable", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsChildRace", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeChildRace", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonChildRace", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanFly", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanFly", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonFlying", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanSwim", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanSwim", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonSwimming", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanWalk", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanWalk", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonWalking", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsImmobile", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeImmobile", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeMobile", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsNotPushable", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeNotPushable", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakePushable", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoKnockdowns", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeNoKnockdowns", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoKNockdowns", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoCombatInWater", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetNoCombatInWater", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoCombatInWater", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AvoidsRoads", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAvoidsRoads", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAvoidsRoads", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AllowPickpocket", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAllowPickpocket", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAllowPickpocket", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AllowPCDialogue", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAllowPCDialogue", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAllowPCDialogue", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CantOpenDoors", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetCantOpenDoors", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearCantOpenDoors", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoShadow", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetNoShadow", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoShadow", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_SCROLL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Cast", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPerk", [], TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("GetNumEffects", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipType", [], TES5BasicType.T_EQUIPSLOT),
                new TES5InheritanceFunctionSignature("SetEquipType", new TES5BasicType[] {
                                TES5BasicType.T_EQUIPSLOT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_SHOUT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNthWordOfPower", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_WORDOFPOWER),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetNthRecoveryTime", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetNthWordOfPower", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_WORDOFPOWER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_SPELL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthRecoveryTime", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_SKSE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetVersion", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionMinor", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionBeta", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionRelease", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetScriptVersionRelease", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetPluginVersion", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_SOULGEM,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSoulSize", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGemSize", [], TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_SOUND,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Play", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("PlayAndWait", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("StopInstance", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetInstanceVolume", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDescriptor", [], TES5BasicType.T_SOUNDDESCRIPTOR)
            }
        },
        { TES5BasicType.T_SOUNDDESCRIPTOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDecibelAttenuation", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetDecibelAttenuation", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDecibelVariance", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetDecibelVariance", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFrequencyVariance", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetFrequencyVariance", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFrequencyShift", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetFrequencyShift", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_SPELL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Cast", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoteCast", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsHostile", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Preload", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Unload", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPerk", [], TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("GetNumEffects", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMagickaCost", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetEffectiveMagickaCost", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipType", [], TES5BasicType.T_EQUIPSLOT),
                new TES5InheritanceFunctionSignature("SetEquipType", new TES5BasicType[] {
                                TES5BasicType.T_EQUIPSLOT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_STRINGUTIL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetLength", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthChar", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("IsLetter", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDigit", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPunctuation", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPrintable", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Find", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("Substring", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("AsOrd", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("AsChar", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING)
            }
        },
        { TES5BasicType.T_TEXTURESET,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumTexturePaths", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetNthTexturePath", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_UI,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsMenuOpen", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetString", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetNumber", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetString", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumber", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Invoke", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeString", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeNumber", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeBoolA", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5ArrayType.ArrayBool
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeIntA", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5ArrayType.ArrayInt
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeFloatA", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5ArrayType.ArrayFloat
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeStringA", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5ArrayType.ArrayString
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeNumberA", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5ArrayType.ArrayFloat
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("InvokeForm", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsTextInputEnabled", [], TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_UTILITY,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GameTimeToString", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetCurrentGameTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCurrentRealTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("IsInMenuMode", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RandomInt", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RandomFloat", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetINIFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetINIInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetINIBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetINIString", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Wait", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("WaitGameTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("WaitMenuMode", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CaptureFrameRate", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("StartFrameRateCapture", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EndFrameRateCapture", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetAverageFrameRate", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMinFrameRate", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMaxFrameRate", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCurrentMemory", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetBudgetCount", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCurrentBudget", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("OverBudget", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetBudgetName", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetINIFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetINIInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetINIBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetINIString", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_STRING)
            }
        },
        { TES5BasicType.T_WEAPON,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Fire", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_AMMO
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBaseDamage", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetBaseDamage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritDamage", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetCritDamage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetReach", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetReach", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMinRange", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetMinRange", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMaxRange", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetMaxRange", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSpeed", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetSpeed", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetStagger", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetStagger", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeaponType", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetWeaponType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModelPath", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIconPath", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMessageIconPath", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetMessageIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantment", [], TES5BasicType.T_ENCHANTMENT),
                new TES5InheritanceFunctionSignature("SetEnchantment", new TES5BasicType[] {
                                TES5BasicType.T_ENCHANTMENT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantmentValue", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetEnchantmentValue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquippedModel", [], TES5BasicType.T_STATIC),
                new TES5InheritanceFunctionSignature("SetEquippedModel", new TES5BasicType[] {
                                TES5BasicType.T_STATIC
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipType", [], TES5BasicType.T_EQUIPSLOT),
                new TES5InheritanceFunctionSignature("SetEquipType", new TES5BasicType[] {
                                TES5BasicType.T_EQUIPSLOT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkill", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResist", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetResist", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritEffect", [], TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("SetCritEffect", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritEffectOnDeath", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetCritEffectOnDeath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritMultiplier", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetCritMultiplier", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsBattleaxe", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBow", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDagger", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGreatsword", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMace", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStaff", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSword", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWarhammer", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWarAxe", [], TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_WEATHER,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ReleaseOverride", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceActive", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetActive", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FindWeather", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_WEATHER),
                new TES5InheritanceFunctionSignature("GetClassification", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCurrentWeather", [], TES5BasicType.T_WEATHER),
                new TES5InheritanceFunctionSignature("GetOutgoingWeather", [], TES5BasicType.T_WEATHER),
                new TES5InheritanceFunctionSignature("GetCurrentWeatherTransition", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSkyMode", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSunGlare", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunDamage", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWindDirection", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWindDirectionRange", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetFogDistance", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FLOAT)
            }
        },
        { TES5BasicType.T_REFERENCEALIAS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Clear", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceRefIfEmpty", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ForceRefTo", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorRef", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetActorReference", [], TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetRef", [], TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetReference", [], TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TryToAddToFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToClear", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToDisable", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToDisableNoWait", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEnable", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEnableNoWait", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEvaluatePackage", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToKill", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToMoveTo", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToRemoveFromFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToReset", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToStopCombat", [], TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_LOCATIONALIAS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Clear", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetLocation", [], TES5BasicType.T_LOCATION),
                new TES5InheritanceFunctionSignature("ForceLocationTo", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_DEBUG,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("CenterOnCell", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CenterOnCellAndWait", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("PlayerMoveToAndWait", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("CloseUserLog", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DumpAliasData", new TES5BasicType[] {
                                TES5BasicType.T_QUEST
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetConfigName", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetPlatformName", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetVersionNumber", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("MessageBox", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Notification", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("OpenUserLog", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("QuitGame", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFootIK", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGodMode", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartScriptProfiling", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartStackProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopScriptProfiling", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopStackProfiling", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleAI", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleCollisions", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleMenus", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Trace", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TraceAndBox", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TraceConditional", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TraceStack", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TraceUser", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_ACTION, [] },
        { TES5BasicType.T_MAGICEFFECT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetAssociatedSkill", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetAssociatedSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResistance", [], TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetResistance", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsEffectFlagSet", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetEffectFlag", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearEffectFlag", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastTime", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetCastTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkillLevel", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSkillLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetArea", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkillUsageMult", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetSkillUsageMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBaseCost", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetBaseCost", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetLight", [], TES5BasicType.T_LIGHT),
                new TES5InheritanceFunctionSignature("SetLight", new TES5BasicType[] {
                                TES5BasicType.T_LIGHT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHitShader", [], TES5BasicType.T_EFFECTSHADER),
                new TES5InheritanceFunctionSignature("SetHitShader", new TES5BasicType[] {
                                TES5BasicType.T_EFFECTSHADER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantShader", [], TES5BasicType.T_EFFECTSHADER),
                new TES5InheritanceFunctionSignature("SetEnchantShader", new TES5BasicType[] {
                                TES5BasicType.T_EFFECTSHADER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetProjectile", [], TES5BasicType.T_PROJECTILE),
                new TES5InheritanceFunctionSignature("SetProjectile", new TES5BasicType[] {
                                TES5BasicType.T_PROJECTILE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetExplosion", [], TES5BasicType.T_EXPLOSION),
                new TES5InheritanceFunctionSignature("SetExplosion", new TES5BasicType[] {
                                TES5BasicType.T_EXPLOSION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastingArt", [], TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetCastingArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHitEffectArt", [], TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetHitEffectArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantArt", [], TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetEnchantArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetImpactDataSet", [], TES5BasicType.T_IMPACTDATASET),
                new TES5InheritanceFunctionSignature("SetImpactDataSet", new TES5BasicType[] {
                                TES5BasicType.T_IMPACTDATASET
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipAbility", [], TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("SetEquipAbility", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetImageSpaceMod", [], TES5BasicType.T_IMAGESPACEMODIFIER),
                new TES5InheritanceFunctionSignature("SetImageSpaceMod", new TES5BasicType[] {
                                TES5BasicType.T_IMAGESPACEMODIFIER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetPerk", [], TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("SetPerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_FURNITURE, [] },
        { TES5BasicType.T_TALKINGACTIVATOR, [] },
        { TES5BasicType.T_ACTIVATOR, [] },
        { TES5BasicType.T_MESSAGE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ResetHelpMessage", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Show", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("ShowAsHelpMessage", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_KEY, [] },
        { TES5BasicType.T_MISCOBJECT, [] },
        { TES5BasicType.T_AMMO, [] },
        { TES5BasicType.T_ASSOCIATIONTYPE, [] },
        { TES5BasicType.T_MUSICTYPE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Remove", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_CLASS, [] },
        { TES5BasicType.T_PACKAGE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", [], TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("GetTemplate", [], TES5BasicType.T_PACKAGE)
            }
        },
        { TES5BasicType.T_CONTAINER, [] },
        { TES5BasicType.T_DOOR, [] },
        { TES5BasicType.T_EFFECTSHADER, [] },
        { TES5BasicType.T_PROJECTILE, [] },
        { TES5BasicType.T_ENCOUNTERZONE, [] },
        { TES5BasicType.T_SCENE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ForceStart", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetOwningQuest", [], TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("IsActionComplete", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlaying", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Start", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Stop", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_EXPLOSION, [] },
        { TES5BasicType.T_FACTION,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("CanPayCrimeGold", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetCrimeGold", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeGoldNonViolent", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeGoldViolent", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamy", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamyNonViolent", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamyViolent", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetReaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStolenItemValueCrime", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStolenItemValueNoCrime", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsFactionInCrimeGroup", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerExpelled", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ModCrimeGold", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModReaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PlayerPayCrimeGold", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendPlayerToJail", new TES5BasicType[] {
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCrimeGold", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetCrimeGoldViolent", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetEnemy", new TES5BasicType[] {
                                TES5BasicType.T_FACTION,
                                TES5BasicType.T_BOOL,
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerEnemy", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetPlayerExpelled", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetReaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_FORMLIST,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Find", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetAt", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetSize", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("HasForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RemoveAddedForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Revert", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_GLOBALVARIABLE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetValue", [], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetValueInt", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("Mod", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetValue", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetValueInt", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_HAZARD, [] },
        { TES5BasicType.T_SOUNDCATEGORY,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Mute", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Pause", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFrequency", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetVolume", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnMute", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnPause", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_IDLE, [] },
        { TES5BasicType.T_IMAGESPACEMODIFIER,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("RemoveCrossFade", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Apply", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ApplyCrossFade", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PopTo", new TES5BasicType[] {
                                TES5BasicType.T_IMAGESPACEMODIFIER,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Remove", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_STATIC, [] },
        { TES5BasicType.T_IMPACTDATASET, [] },
        { TES5BasicType.T_TOPIC,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", [], TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_LOCATIONREFTYPE, [] },
        { TES5BasicType.T_TOPICINFO,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", [], TES5BasicType.T_QUEST)
            }
        },
        { TES5BasicType.T_LEVELEDACTOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Revert", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNthCount", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthCount", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_VISUALEFFECT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Play", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_FLOAT,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Stop", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_LEVELEDITEM,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Revert", [], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceNone", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetChanceNone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceGlobal", [], TES5BasicType.T_GLOBALVARIABLE),
                new TES5InheritanceFunctionSignature("SetChanceGlobal", new TES5BasicType[] {
                                TES5BasicType.T_GLOBALVARIABLE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNthCount", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthCount", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_VOICETYPE, [] },
        { TES5BasicType.T_LEVELEDSPELL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceNone", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetChanceNone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetNthLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_LIGHT, [] },
        { TES5BasicType.T_LOCATION,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetKeywordData", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetRefTypeAliveCount", new TES5BasicType[] {
                                TES5BasicType.T_LOCATIONREFTYPE
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetRefTypeDeadCount", new TES5BasicType[] {
                                TES5BasicType.T_LOCATIONREFTYPE
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("HasCommonParent", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION,
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("HasRefType", new TES5BasicType[] {
                                TES5BasicType.T_LOCATIONREFTYPE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCleared", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsChild", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLoaded", [], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSameLocation", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION,
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetCleared", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetKeywordData", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_WORDOFPOWER, [] },
        { TES5BasicType.T_WORLDSPACE, [] },
        //SKSE plugin:
        { TES5BasicType.T_SKYBTIMERHELPER,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDayOfWeek", Array.Empty<TES5BasicType>(), TES5BasicType.T_INT)
                /*new TES5InheritanceFunctionSignature("GetSecondsPassed", new TES5BasicType[] {
                        TES5BasicType.T_FLOAT
                    }, TES5BasicType.T_FLOAT),*///WTM:  Change:  Deleted
            }
        },
        { TES5BasicType.T_SKYBGameUtility,
            [
                new TES5InheritanceFunctionSignature("LegacyGetAmountSoldStolen", [], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LegacyIsPCAMurderer", [], TES5BasicType.T_BOOL)
            ]
        },
        { TES5BasicType.T_SKYBObjectReferenceUtility,
            [
                new TES5InheritanceFunctionSignature("ContainsItem", [TES5BasicType.T_OBJECTREFERENCE, TES5BasicType.T_FORM], TES5BasicType.T_BOOL),
                //new TES5InheritanceFunctionSignature("LegacyRotate", [TES5BasicType.T_OBJECTREFERENCE, TES5BasicType.T_FLOAT, TES5BasicType.T_FLOAT, TES5BasicType.T_FLOAT], TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsAnimPlaying", [TES5BasicType.T_OBJECTREFERENCE], TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("LegacyGetDestroyed", [TES5BasicType.T_OBJECTREFERENCE], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LegacySay", [TES5BasicType.T_OBJECTREFERENCE, TES5BasicType.T_TOPIC, TES5BasicType.T_BOOL], TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("LegacySayTo", [TES5BasicType.T_OBJECTREFERENCE, TES5BasicType.T_ACTOR, TES5BasicType.T_TOPIC, TES5BasicType.T_BOOL], TES5BasicType.T_FLOAT)
            ]
        },
        { TES5BasicType.T_SKYBDISPOSITIONUTIL,
            [
                new TES5InheritanceFunctionSignature("GetDisposition", [TES5BasicType.T_ACTOR], TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("ModDisposition", [TES5BasicType.T_ACTOR, TES5BasicType.T_INT], TES5BasicType.T_INT)
            ]
        }
        };



        private static TES5InheritanceItemCollection? FindSubtreeFor(TES5BasicType type)
        {
            return FindInternalSubtreeFor(type, inheritance);
        }

        private static TES5InheritanceItemCollection? FindInternalSubtreeFor(TES5BasicType type, TES5InheritanceItemCollection inputTree)
        {
            foreach (var item in inputTree)
            {
                var treeType = item.ParentType;
                var itemsWithoutSubItems = item.ItemsWithoutSubItems;
                foreach (var itemWithoutSubItems in itemsWithoutSubItems)
                {//value only
                    if (itemWithoutSubItems.ParentType == type)
                    {
                        return new TES5InheritanceItemCollection(); //Value matches.
                    }
                }
                if (treeType == type)
                {
                    return item.Items;
                }
                else
                {
                    TES5InheritanceItemCollection? data = FindInternalSubtreeFor(type, item.Items);
                    if (data != null)
                    {
                        return data;
                    }
                }
            }
            return null;//Not found.
        }

        private static bool TreeContains(TES5BasicType type, TES5InheritanceItemCollection inputTree)
        {
            return FindInternalSubtreeFor(type, inputTree) != null;
        }

        public static bool IsExtending(ITES5Type extendingType, ITES5Type baseType)
        {
            if (!extendingType.IsNativePapyrusType && baseType.IsNativePapyrusType)
            {
                return IsTypeOrExtendsType(extendingType.NativeType, baseType);
            }
            else if (extendingType.IsNativePapyrusType && !baseType.IsNativePapyrusType)
            {
                return false;
            }
            TES5InheritanceItemCollection? subTree = FindSubtreeFor(baseType.NativeType);
            if (subTree == null)
            {
                return false;
            }
            return TreeContains(extendingType.NativeType, subTree);
        }

        public static bool IsTypeOrExtendsType(ITES5Type extendingType, ITES5Type baseType)
        {
            return extendingType.Equals(baseType) || IsExtending(extendingType, baseType);
        }
        public static bool IsTypeOrExtendsType(ITES5Type extendingType, IEnumerable<ITES5Type> baseTypes)
        {
            foreach (ITES5Type baseType in baseTypes)
            {
                if (IsTypeOrExtendsType(extendingType, baseType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTypeOrExtendsTypeOrIsNumberType(ITES5Type extendingType, ITES5Type baseType, bool commuteNativeTypes)
        {
            return IsTypeOrExtendsType(extendingType, baseType) ||
                IsNumberType(extendingType, baseType) ||
                IsTypeOrExtendsType(extendingType.NativeType, baseType.NativeType) ||
                commuteNativeTypes && IsTypeOrExtendsType(baseType.NativeType, extendingType.NativeType);
        }

        public static bool IsNumberTypeOrBoolAndInt(ITES5Type type1, ITES5Type type2)
        {
            return (IsNumberType(type1) && IsNumberType(type2)) ||
                (type1 == TES5BasicType.T_INT && type2 == TES5BasicType.T_BOOL) ||
                (type1 == TES5BasicType.T_BOOL && type2 == TES5BasicType.T_INT);
        }

        private static bool IsNumberType(ITES5Type type)
        {
            return type == TES5BasicType.T_INT || type == TES5BasicType.T_FLOAT;
        }

        public static bool IsNumberType(ITES5Type type1, ITES5Type type2)
        {
            return IsNumberType(type1) && IsNumberType(type2);
        }

        public static bool IsTypeOrIsNumberTypeOrBoolAndInt(ITES5Type type1, ITES5Type type2)
        {
            return type1.Equals(type2) || IsNumberTypeOrBoolAndInt(type1, type2);
        }

        private static TES5BasicType? GetCommonBaseType(TES5BasicType type1, TES5BasicType type2)
        {
            if (type1 == type2) { return type1; }
            TES5BasicType[] baseTypes1 = GetSelfAndBaseClassesWithoutCache(type1).Reverse().ToArray();
            TES5BasicType[] baseTypes2 = GetSelfAndBaseClassesWithoutCache(type2).Reverse().ToArray();
            TES5BasicType? commonBaseType = null;
            for (int i = 0; i < baseTypes1.Length && i < baseTypes2.Length; i++)
            {
                if (baseTypes1[i] == baseTypes2[i])
                {
                    commonBaseType = baseTypes1[i];
                }
                else
                {
                    break;
                }
            }
            return commonBaseType;
        }

        public static TES5BasicType GetCommonBaseType(IEnumerable<TES5BasicType> types)
        {
            TES5BasicType[] distinctTypes = types.Distinct().ToArray();
            TES5BasicType baseType = distinctTypes[0];
            for (int i = 1; i < distinctTypes.Length; i++)
            {
                TES5BasicType nextType = distinctTypes[i];
                TES5BasicType? newBaseType = GetCommonBaseType(nextType, baseType);
                if (newBaseType == null) { throw new ConversionException("A common base type could not be found."); }
                baseType = newBaseType;
            }
            return baseType;
        }

        private static TES5BasicType? TargetRootBaseClassNullable(TES5BasicType type, TES5InheritanceItem baseClass, bool throwIfNotFound)
        {
            TES5BasicType? baseTypeForNode = baseClass.ParentType;
            TES5InheritanceItemCollection baseClassExtenders = baseClass.Items;
            if (baseClassExtenders.Any())
            {
                foreach (var item in baseClassExtenders)
                {
                    if (item.ParentType == type)
                    {
                        if (baseTypeForNode == null && throwIfNotFound)
                        {
                            throw new ConversionException("Type " + type.Value + " is a top-level type in the inheritance graph, so it has no base class.");
                        }
                        return baseTypeForNode;
                    }
                }

                foreach (var item in baseClassExtenders)
                {
                    TES5BasicType? recursiveReturn = TargetRootBaseClassNullable(type, item);
                    if (recursiveReturn != null)
                    {
                        return recursiveReturn;
                    }
                }

                //not found in node.
                if (baseTypeForNode == null || throwIfNotFound)
                {
                    throw new ConversionException("Type " + type.Value + " not found in inheritance graph.");
                }
                return null;
            }
            else if (baseTypeForNode == type)
            {
                return baseTypeForNode;
            }
            if (throwIfNotFound)
            {
                throw new ConversionException("Type " + type.Value + " not found in inheritance graph.");
            }
            return null;
        }
        private static TES5BasicType? TargetRootBaseClassNullable(TES5BasicType type, TES5InheritanceItem baseClass)
        {
            return TargetRootBaseClassNullable(type, baseClass, false);
        }
        private static TES5BasicType TargetRootBaseClass(TES5BasicType type, TES5InheritanceItem baseClass)
        {
            return TargetRootBaseClassNullable(type, baseClass, true)!;
        }

        public static TES5BasicType GetBaseClassWithCache(TES5BasicType type)
        {
            return inheritanceCache.GetOrAdd(type, () =>
            {
                return TargetRootBaseClass(type, inheritanceAsItem);
            });
        }

        private static TES5BasicType? GetBaseClassWithoutCache(TES5BasicType type)
        {
            return TargetRootBaseClassNullable(type, inheritanceAsItem);
        }

        public static IEnumerable<TES5BasicType> GetBaseClassesWithoutCache(TES5BasicType type)
        {
            TES5BasicType baseType = type;
            while (true)
            {
                TES5BasicType? newBaseType = GetBaseClassWithoutCache(baseType);
                if (newBaseType == null) { yield break; }
                baseType = newBaseType;
                yield return baseType;
            }
        }

        public static IEnumerable<TES5BasicType> GetSelfAndBaseClassesWithoutCache(TES5BasicType type)
        {
            yield return type;
            foreach (TES5BasicType baseType in GetBaseClassesWithoutCache(type))
            {
                yield return baseType;
            }
        }

        [Obsolete("Unused")]
        public static TES5BasicType FindTypeByMethodParameter(TES5BasicType calledOnType, string methodName, int parameterIndex)
        {
            TES5InheritanceFunctionSignature[]? callReturnsOfCalledOnType;
            if (!callReturns.TryGetValue(calledOnType, out callReturnsOfCalledOnType))
            {
                throw new ConversionException("Inference type exception - no methods found for " + calledOnType.Value + "!");
            }

            foreach (var callReturn in callReturnsOfCalledOnType)
            {
                if (callReturn.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    TES5BasicType[] arguments = callReturn.Arguments;
                    TES5BasicType argument;
                    try
                    {
                        argument = arguments[parameterIndex];
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        throw new ConversionException("Cannot find argument index " + parameterIndex + " in method " + methodName + " in type " + calledOnType.Value, ex);
                    }
                    return argument;
                }
            }

            TES5BasicType calledOnTypeBaseClass;
            try
            {
                calledOnTypeBaseClass = GetBaseClassWithCache(calledOnType);
            }
            catch (ConversionException ex)
            {
                throw new ConversionException("Method " + methodName + " not found in type " + calledOnType.Value, ex);
            }
            return FindTypeByMethodParameter(calledOnTypeBaseClass, methodName, parameterIndex);
        }

        public static ITES5Type FindReturnTypeForObjectCall(TES5BasicType calledOnType, string methodName)
        {
            TES5InheritanceFunctionSignature[]? callReturnsOfCalledOnType;
            if (!callReturns.TryGetValue(calledOnType, out callReturnsOfCalledOnType))
            {
                //Type not present in inheritance graph, check if its a basic type ( which means its basically an exception )
                throw new ConversionException("Inference type exception - no call returns for " + calledOnType.Value + "!");
            }

            foreach (var method in callReturnsOfCalledOnType)
            {
                if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    return method.ReturnType;
                }
            }

            return FindReturnTypeForObjectCall(GetBaseClassWithCache(calledOnType), methodName);
        }

        public static TES5BasicType FindTypeByMethodOld(TES5ObjectCall objectCall, ESMAnalyzer esmAnalyzer)
        {
            string methodName = objectCall.FunctionName;
            List<TES5BasicType> possibleMatches = new List<TES5BasicType>();
            foreach (var callReturn in callReturns)
            {
                TES5BasicType type = callReturn.Key;
                var methods = callReturn.Value;
                foreach (var method in methods)
                {
                    if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                    {
                        possibleMatches.Add(type);
                    }
                }
            }

            ITES5VariableOrProperty? calledOn = objectCall.AccessedObject.ReferencesTo;
            if (calledOn == null) { throw new InvalidOperationException(nameof(calledOn) + " was null."); }
            ITES5Type calledOnType = calledOn.TES5Type;
            TES5BasicType actualType = calledOnType.NativeType;
            List<TES5BasicType> extendingMatches = new List<TES5BasicType>();
            foreach (TES5BasicType possibleMatch in possibleMatches)
            {
                if (possibleMatch == actualType)
                {
                    return possibleMatch; //if the possible match matches the actual basic type, it means that it surely IS one of those.
                }

                //Ok, so are those matches somehow connected at all?
                if (IsExtending(possibleMatch, actualType) || IsExtending(actualType, possibleMatch))
                {
                    extendingMatches.Add(possibleMatch);
                }
            }

            switch (extendingMatches.Count)
            {
                case 0:
                    {
                        string possibleMatchesString = string.Join(", ", possibleMatches.Select(m => m.Value));
                        throw new ConversionTypeMismatchException("Cannot find any possible type for method " + methodName + ", trying to extend " + actualType.Value + " with following types: " + possibleMatchesString);
                    }

                case 1:
                    {
                        return extendingMatches[0];
                    }

                default:
                    {
                        if (calledOn.ReferenceEDID == null) { throw new InvalidOperationException(nameof(calledOn.ReferenceEDID) + " was null."); }
                        //We analyze the property name and check inside the ESM analyzer.
                        TES5BasicType formType = esmAnalyzer.GetTypeByEDID(calledOn.ReferenceEDID);
                        IEnumerable<TES5BasicType> formTypeAndBaseTypes = GetSelfAndBaseClassesWithoutCache(formType);
                        if (!formTypeAndBaseTypes.Any(bt => extendingMatches.Contains(bt)))
                        {
                            string extendingMatchesString = string.Join(", ", extendingMatches.Select(em => em.Value));
                            throw new ConversionException("ESM <-> Inheritance Graph conflict.  ESM returned " + formType.Value + ", which is not present in possible matches from inheritance graph:  " + extendingMatchesString);
                        }
                        return formType;
                    }
            }
        }

        private static IEnumerable<TES5InheritanceFunctionWithCalledOn> GetFunctions(string functionName)
        {
            foreach (var group in callReturns)
            {
                TES5BasicType calledOn = group.Key;
                var functions = group.Value;
                foreach (var function in functions)
                {
                    if (function.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new TES5InheritanceFunctionWithCalledOn(function, calledOn);
                    }
                }
            }
        }

        private static (TES5InheritanceFunctionWithCalledOn? ExactMatch, List<TES5InheritanceFunctionWithCalledOn>? MatchesByExtensionOfCalledOnType) GetFunctions(ITES5Type calledOnType, IEnumerable<TES5InheritanceFunctionWithCalledOn> functions)
        {
            TES5BasicType nativeCalledOnType = calledOnType.NativeType;
            List<TES5InheritanceFunctionWithCalledOn> extendingMatches = new List<TES5InheritanceFunctionWithCalledOn>();
            foreach (var possibleMatch in functions)
            {
                if (possibleMatch.CalledOn == nativeCalledOnType)
                {
                    return (possibleMatch, null); //if the possible match matches the actual basic type, it means that it surely IS one of those.
                }

                //Ok, so are those matches somehow connected at all?
                if (IsExtending(possibleMatch.CalledOn, nativeCalledOnType) || IsExtending(nativeCalledOnType, possibleMatch.CalledOn))
                {
                    extendingMatches.Add(possibleMatch);
                }
            }
            return (null, extendingMatches);
        }

        private static TES5InheritanceFunctionWithCalledOn ReviseCalledOnType(TES5ObjectCall objectCall, IReadOnlyCollection<TES5InheritanceFunctionWithCalledOn> functions, ESMAnalyzer esmAnalyzer)
        {
            ITES5VariableOrProperty? calledOn = objectCall.AccessedObject.ReferencesTo;
            if (calledOn == null)
            {
                return functions.Where(f => IsTypeOrExtendsType(objectCall.AccessedObject.TES5Type, f.CalledOn)).Single();
            }
            ITES5Type calledOnType = calledOn.TES5Type;
            var (exactMatch, extendingMatches) = GetFunctions(calledOnType, functions);
            if (exactMatch != null) { return exactMatch; }
            if (extendingMatches == null) { throw new NullableException(nameof(extendingMatches)); }
            TES5BasicType nativeCalledOnType = calledOnType.NativeType;
            switch (extendingMatches.Count)
            {
                case 0:
                    {
                        string possibleMatchesString = string.Join(", ", functions.Select(m => m.CalledOn.Value));
                        throw new ConversionTypeMismatchException("Cannot find any possible type for method " + objectCall.FunctionName + ", trying to extend " + nativeCalledOnType.Value + " with following types: " + possibleMatchesString);
                    }

                case 1:
                    {
                        return extendingMatches[0];
                    }

                default:
                    {
                        if (calledOn.ReferenceEDID == null) { throw new InvalidOperationException(nameof(calledOn.ReferenceEDID) + " was null."); }
                        //We analyze the property name and check inside the ESM analyzer.
                        TES5BasicType formType = esmAnalyzer.GetTypeByEDID(calledOn.ReferenceEDID);
                        IEnumerable<TES5BasicType> formTypeAndBaseTypes = GetSelfAndBaseClassesWithoutCache(formType);
                        var extendingMatchesTypes = extendingMatches.Select(f => f.CalledOn).ToArray();
                        if (!formTypeAndBaseTypes.Any(bt => extendingMatchesTypes.Contains(bt)))
                        {
                            string extendingMatchesString = string.Join(", ", extendingMatchesTypes.Select(em => em.Value));
                            throw new ConversionException("ESM <-> Inheritance Graph conflict.  ESM returned " + formType.Value + ", which is not present in possible matches from inheritance graph:  " + extendingMatchesString);
                        }
                        var exactMatch2 = GetFunctions(formType, functions).ExactMatch;
                        if (exactMatch2 == null) { throw new NullableException(nameof(exactMatch2)); }
                        return exactMatch2;
                    }
            }
        }

        public static TES5InheritanceFunctionWithCalledOn FindFunction(TES5ObjectCall objectCall, ESMAnalyzer esmAnalyzer)
        {
            TES5InheritanceFunctionWithCalledOn[] functions = GetFunctions(objectCall.FunctionName).ToArray();
            if (functions.Length == 0)
            {
                throw new ConversionException("No functions were found with the name " + objectCall.FunctionName);
            }
            return ReviseCalledOnType(objectCall, functions, esmAnalyzer);
        }
    }
}
