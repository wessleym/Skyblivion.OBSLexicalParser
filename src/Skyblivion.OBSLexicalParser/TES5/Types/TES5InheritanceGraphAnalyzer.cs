using Dissect.Extensions.IDictionaryExtensions;
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
                        TES5BasicType.T_TES4TIMERHELPER,
                        TES5BasicType.T_TES4CONTAINER
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
            { TES5BasicType.T_UI, new TES5InheritanceItemCollection() }
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
                new TES5InheritanceFunctionSignature("Dispel", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBaseObject", new TES5BasicType[] {
                            }, TES5BasicType.T_MAGICEFFECT),
                new TES5InheritanceFunctionSignature("GetCasterActor", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetTargetActor", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
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
                new TES5InheritanceFunctionSignature("RegisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new TES5BasicType[] {
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
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDuration", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTimeElapsed", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("RegisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("OnUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),//WTM:  Change:  Added
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
                new TES5InheritanceFunctionSignature("MakePlayerFriend", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("CanFlyHere", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ClearArrested", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearExpressionOverride", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearExtraArrows", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearForcedLandingMarker", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearKeepOffsetFromActor", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearLookAt", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Dismount", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("DispelAllSpells", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("EndDeferredKill", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("EvaluatePackage", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorBase", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTORBASE),
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
                new TES5InheritanceFunctionSignature("GetBribeAmount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeFaction", new TES5BasicType[] {
                            }, TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("GetCombatState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCombatTarget", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetCurrentPackage", new TES5BasicType[] {
                            }, TES5BasicType.T_PACKAGE),
                new TES5InheritanceFunctionSignature("GetDialogueTarget", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetEquippedItemType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetEquippedShout", new TES5BasicType[] {
                            }, TES5BasicType.T_SHOUT),
                new TES5InheritanceFunctionSignature("GetEquippedWeapon", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_WEAPON),
                new TES5InheritanceFunctionSignature("GetEquippedShield", new TES5BasicType[] {
                            }, TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("GetEquippedSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetFactionRank", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetFactionReaction", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetFlyingState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetForcedLandingMarker", new TES5BasicType[] {
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetGoldAmount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetHighestRelationshipRank", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetKiller", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetLevel", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetLightLevel", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetLowestRelationshipRank", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetLeveledActorBase", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetNoBleedoutRecovery", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetPlayerControls", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetRace", new TES5BasicType[] {
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetRelationshipRank", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetRestrained", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetSitState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSleepState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVoiceRecoveryTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("IsAlarmed", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsAlerted", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsAllowedToFly", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsArrested", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsArrestingTarget", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBeingRidden", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBleedingOut", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBribed", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsChild", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCommandedActor", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDead", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDetectedBy", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDoingFavor", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEquipped", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEssential", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFlying", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGuard", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGhost", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHostileToActor", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInCombat", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInKillMove", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsIntimidated", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsOnMount", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayersLastRiddenHorse", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerTeammate", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsRunning", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSneaking", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSprinting", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsTorchOut" ,new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsTrespassing", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsUnconscious", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWeaponDrawn", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("MoveToPackageLocation", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("RemoveFromAllFactions", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemovePerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveShout", new TES5BasicType[] {
                                TES5BasicType.T_SHOUT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RemoveSpell", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ResetHealthAndLimbs", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RestoreActorValue", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Resurrect", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RestoreAV", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("SetPlayerResistingArrest", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("ShowBarterMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("StartDeferredKill", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartVampireFeed", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopCombat", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopCombatAlarm", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TrapSoul", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("UnequipAll", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("UnLockOwnedDoorsInCell", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("WillIntimidateSucceed", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("WornHasKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("StartSneaking", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DrawWeapon", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("ClearForcedMovement", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetSpellCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("QueueNiNodeUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegenerateHead", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("IsAIEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSwimming", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SheatheWeapon", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                /*new TES5InheritanceFunctionSignature("ForceFlee", new TES5BasicType[] {
                                TES5BasicType.T_CELL,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),//WTM:  Note:  I think Aerisarn was going to add this, possibly utilizing SKSE's code.  But since it isn't ready, I'm commenting this for now.
                new TES5InheritanceFunctionSignature("OBStartConversation", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_TOPIC
                            }, TES5VoidType.Instance)//WTM:  Note:  I think Aerisarn was going to add this.  But since it isn't ready, I'm commenting this for now.*/
            }
        },
        { TES5BasicType.T_ACTORBASE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetClass", new TES5BasicType[] {
                            }, TES5BasicType.T_CLASS),
                new TES5InheritanceFunctionSignature("GetDeadCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGiftFilter", new TES5BasicType[] {
                            }, TES5BasicType.T_FORMLIST),
                new TES5InheritanceFunctionSignature("GetRace", new TES5BasicType[] {
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetSex", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsEssential", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInvulnerable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsProtected", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsUnique", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("GetCombatStyle", new TES5BasicType[] {
                            }, TES5BasicType.T_COMBATSTYLE),
                new TES5InheritanceFunctionSignature("SetCombatStyle", new TES5BasicType[] {
                                TES5BasicType.T_COMBATSTYLE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetOutfit", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_OUTFIT),
                new TES5InheritanceFunctionSignature("SetClass", new TES5BasicType[] {
                                TES5BasicType.T_CLASS
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetHeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetWeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumHeadParts", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetHairColor", new TES5BasicType[] {
                            }, TES5BasicType.T_COLORFORM),
                new TES5InheritanceFunctionSignature("SetHairColor", new TES5BasicType[] {
                                TES5BasicType.T_COLORFORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSpellCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthSpell", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetFaceTextureSet", new TES5BasicType[] {
                            }, TES5BasicType.T_TEXTURESET),
                new TES5InheritanceFunctionSignature("SetFaceTextureSet", new TES5BasicType[] {
                                TES5BasicType.T_TEXTURESET
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetVoiceType", new TES5BasicType[] {
                            }, TES5BasicType.T_VOICETYPE),
                new TES5InheritanceFunctionSignature("SetVoiceType", new TES5BasicType[] {
                                TES5BasicType.T_VOICETYPE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkin", new TES5BasicType[] {
                            }, TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkin", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkinFar", new TES5BasicType[] {
                            }, TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkinFar", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetTemplate", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTORBASE)
            }
        },
        { TES5BasicType.T_ALIAS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new TES5BasicType[] {
                            }, TES5BasicType.T_QUEST),
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
                new TES5InheritanceFunctionSignature("RegisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetName", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetID", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("RegisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetQuality", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetQuality", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_ARMOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetArmorRating", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetAR", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetWeightClass", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetWeightClass", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantment", new TES5BasicType[] {
                            }, TES5BasicType.T_ENCHANTMENT),
                new TES5InheritanceFunctionSignature("SetEnchantment", new TES5BasicType[] {
                                TES5BasicType.T_ENCHANTMENT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsLightArmor", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHeavyArmor", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothing", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBoots", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCuirass", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGauntlets", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsHelmet", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsShield", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsJewelry", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingHead", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingBody", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingFeet", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingHands", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingRing", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingRich", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsClothingPoor", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetSlotMask", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetNumArmorAddons", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetNumAdditionalRaces", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthAdditionalRace", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetSlotMask", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetSpell", new TES5BasicType[] {
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("GetSkill", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsRead", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsTakeable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_CELL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetActorOwner", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetFactionOwner", new TES5BasicType[] {
                            }, TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("IsAttached", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInterior", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Reset", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetColor", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetColor", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetRed", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGreen", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetBlue", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetHue", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSaturation", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetValue", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT)
            },*/
        { TES5BasicType.T_COMBATSTYLE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOffensiveMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetDefensiveMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetGroupOffensiveMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAvoidThreatChance", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetRangedMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMagicMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetShoutMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetStaffMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetUnarmedMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("GetMeleeAttackStaggeredMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackStaggeredMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackBlockingMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashRecoiledMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashAttackMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeBashPowerAttackMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMeleeSpecialAttackMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAllowDualWielding", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingCircleMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingFallbackMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingFlankDistance", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingStalkTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("GetLongRangeStrafeMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetLongRangeStrafeMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFlightHoverChance", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetFlightDiveBombChance", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetFlightFlyingAttackChance", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("GetResult", new TES5BasicType[] {
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("SetResult", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResultQuantity", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetResultQuantity", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumIngredients", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetWorkbenchKeyword", new TES5BasicType[] {
                            }, TES5BasicType.T_KEYWORD),
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
                new TES5InheritanceFunctionSignature("IsHostile", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNumEffects", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new TES5BasicType[] {
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_EQUIPSLOT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParents", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthParent", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_EQUIPSLOT)
            }
        },
        { TES5BasicType.T_FLORA,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHarvestSound", new TES5BasicType[] {
                            }, TES5BasicType.T_SOUNDDESCRIPTOR),
                new TES5InheritanceFunctionSignature("SetHarvestSound", new TES5BasicType[] {
                               TES5BasicType.T_SOUNDDESCRIPTOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIngredient", new TES5BasicType[] {
                            }, TES5BasicType.T_INGREDIENT),
                new TES5InheritanceFunctionSignature("SetIngredient", new TES5BasicType[] {
                                TES5BasicType.T_INGREDIENT
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_FORM,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetFormID", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGoldValue", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("HasKeyword", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PlayerKnows", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("RegisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new TES5BasicType[] {
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
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetType", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetName", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetName", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetWeight", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetGoldValue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumKeywords", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendModEvent", new TES5BasicType[] {
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_STRING,
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TempClone", new TES5BasicType[] {
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetSecondsPassed", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT)//WTM:  Change:  Added
            }
        },
        /*{ "FormType",
            new TES5InheritanceFunctionSignature[] {
            }
        },*///WTM:  Change:  Commented
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
                new TES5InheritanceFunctionSignature("ClearPrison", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearTempEffects", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("ForceThirdPerson", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceFirstPerson", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetPlayer", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetPlayerGrabbedRef", new TES5BasicType[] {
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetPlayersLastRiddenHorse", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetSunPositionX", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunPositionY", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunPositionZ", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetRealHoursPassed", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("IsActivateControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCamSwitchControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFastTravelControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFastTravelEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFightingControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsJournalControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLookingControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMenuControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMovementControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerSungazing", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSneakingControlsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWordUnlocked", new TES5BasicType[] {
                                TES5BasicType.T_WORDOFPOWER
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PrecacheCharGen", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("PrecacheCharGenClear", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("QueryStat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("QuitToMainMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestAutoSave", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestModel", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RequestSave", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ServeTime", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SendWereWolfTransformation", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("ShowRaceMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowLimitedRaceMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ShowTitleSequenceMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("HideTitleSequenceMenu", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("UsingGamepad", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetPerkPoints", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetPerkPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ModPerkPoints", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetNumTintMasks", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("UpdateTintMaskColors", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateHairColor", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCameraState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetPlayerMovementMode", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("UpdateThirdPerson", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                //WTM:  Note:  These methods actually seem to exist natively in Skyrim, but in my tests, they don't work reliably.
                //I seemed to only be able to call ModAmountSoldStolen once.  After that, I couldn't get it to have any affect on the stored value.
                /*new TES5InheritanceFunctionSignature("GetAmountSoldStolen", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("ModAmountSoldStolen", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),*/
                new TES5InheritanceFunctionSignature("LegacyGetAmountSoldStolen", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),//WTM:  Change:  Added via Skyblivion.dll
                new TES5InheritanceFunctionSignature("LegacyModAmountSoldStolen", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),//WTM:  Change:  Added via Skyblivion.dll
                /*new TES5InheritanceFunctionSignature("IsPCAMurderer", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),*/
                new TES5InheritanceFunctionSignature("LegacyIsPCAMurderer", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),//WTM:  Change:  Added via Skyblivion.dll (experimenting)
                new TES5InheritanceFunctionSignature("EssentialDeathReload", new TES5BasicType[] {//WTM:  Change:  Added via Skyblivion.dll (experimenting)
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_HEADPART,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHeadPart", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_HEADPART),
                new TES5InheritanceFunctionSignature("GetType", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNumExtraParts", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_HEADPART),
                new TES5InheritanceFunctionSignature("HasExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetIndexOfExtraPart", new TES5BasicType[] {
                                TES5BasicType.T_HEADPART
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetValidRaces", new TES5BasicType[] {
                            }, TES5BasicType.T_FORMLIST),
                new TES5InheritanceFunctionSignature("SetValidRaces", new TES5BasicType[] {
                                TES5BasicType.T_FORMLIST
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_INGREDIENT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("LearnEffect", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("LearnNextEffect", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("LearnAllEffects", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumEffects", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new TES5BasicType[] {
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
                new TES5InheritanceFunctionSignature("GetNumKeysPressed", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetString", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING)
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
                new TES5InheritanceFunctionSignature("IsNearPlayer", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInInterior", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("DeleteWhenAble", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AddKeyIfNeeded", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("get", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("CanFastTravelToMarker", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ClearDestruction", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CreateDetectionEvent", new TES5BasicType[] {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("DamageObject", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Delete", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("ForceAddRagdollToWorld", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceRemoveRagdollFromWorld", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorOwner", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTORBASE),
                new TES5InheritanceFunctionSignature("GetAngleX", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAngleY", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAngleZ", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetAnimationVariableBool", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetAnimationVariableInt", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetAnimationVariableFloat", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetBaseObject", new TES5BasicType[] {
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetCurrentDestructionStage", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCurrentLocation", new TES5BasicType[] {
                            }, TES5BasicType.T_LOCATION),
                new TES5InheritanceFunctionSignature("GetCurrentScene", new TES5BasicType[] {
                            }, TES5BasicType.T_SCENE),
                new TES5InheritanceFunctionSignature("GetDestroyed", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetDistance", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetEditorLocation", new TES5BasicType[] {
                            }, TES5BasicType.T_LOCATION),
                new TES5InheritanceFunctionSignature("GetEnableParent", new TES5BasicType[] {
                            }, TES5BasicType.T_OBJECTREFERENCE),//WTM:  Note:  SKSE
                new TES5InheritanceFunctionSignature("GetFactionOwner", new TES5BasicType[] {
                            }, TES5BasicType.T_FACTION),
                new TES5InheritanceFunctionSignature("GetHeadingAngle", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetHeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetItemCount", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetItemHealthPercent", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetKey", new TES5BasicType[] {
                            }, TES5BasicType.T_KEY),
                new TES5InheritanceFunctionSignature("GetLength", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetLinkedRef", new TES5BasicType[] {
                                TES5BasicType.T_KEYWORD
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetLockLevel", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetMass", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetOpenState", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetParentCell", new TES5BasicType[] {
                            }, TES5BasicType.T_CELL),
                new TES5InheritanceFunctionSignature("GetPositionX", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPositionY", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPositionZ", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetScale", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTriggerObjectCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVoiceType", new TES5BasicType[] {
                            }, TES5BasicType.T_VOICETYPE),
                new TES5InheritanceFunctionSignature("GetWidth", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWorldSpace", new TES5BasicType[] {
                            }, TES5BasicType.T_WORLDSPACE),
                new TES5InheritanceFunctionSignature("GetSelfAsActor", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
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
                new TES5InheritanceFunctionSignature("InterruptCast", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsActivateChild", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsActivationBlocked", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Is3DLoaded", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDeleted", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDisabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFurnitureInUse", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFurnitureMarkerInUse", new TES5BasicType[] {
                                TES5BasicType.T_INT,
                                TES5BasicType.T_BOOL
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsIgnoringFriendlyHits", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsInDialogueWithPlayer", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLockBroken", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLocked", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMapMarkerVisible", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("MoveToMyEditorLocation", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("StopTranslation", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetNumItems", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthForm", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM),
                new TES5InheritanceFunctionSignature("GetTotalItemWeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetTotalArmorWeight", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("IsHarvested", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetHarvested", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetItemHealthPercent", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetItemMaxCharge", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetItemCharge", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetItemCharge", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ResetInventory", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsOffLimits", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("isAnimPlaying", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("OnUpdate", new TES5BasicType[] {
                            }, TES5VoidType.Instance),//WTM:  Change:  Added
                new TES5InheritanceFunctionSignature("GotoState", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),//WTM:  Change:  Added
                new TES5InheritanceFunctionSignature("SetDisplayName", new TES5BasicType[]
                            {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),//WTM:  Change:  Added.  SKSE.
                new TES5InheritanceFunctionSignature("ContainsItem", new TES5BasicType[]
                            {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),//WTM:  Change:  Added.  See modified ObjectReference.psc.
                new TES5InheritanceFunctionSignature("LegacyStartConversation", new TES5BasicType[]
                            {
                                TES5BasicType.T_ACTOR,
                                TES5BasicType.T_TOPIC
                            }, TES5VoidType.Instance),//WTM:  Change:  Added.  See modified ObjectReference.psc.
                new TES5InheritanceFunctionSignature("LegacySay", new TES5BasicType[] {
                        TES5BasicType.T_TOPIC,
                        TES5BasicType.T_BOOL
                    }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("LegacySayTo", new TES5BasicType[] {
                        TES5BasicType.T_ACTOR,
                        TES5BasicType.T_TOPIC,
                        TES5BasicType.T_BOOL
                    }, TES5BasicType.T_FLOAT)
            }
        },
        { TES5BasicType.T_OUTFIT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParts", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthPart", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_FORM)
            }
        },
        { TES5BasicType.T_PERK,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumEntries", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("IsHostile", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsFood", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetNumEffects", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new TES5BasicType[] {
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
                new TES5InheritanceFunctionSignature("CompleteAllObjectives", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CompleteQuest", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("FailAllObjectives", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetAlias", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_ALIAS),
                new TES5InheritanceFunctionSignature("GetCurrentStageID", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStage", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStageDone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsActive", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsCompleted", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveCompleted", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveDisplayed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsObjectiveFailed", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsRunning", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStageDone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStarting", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStopping", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStopped", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("PrepareForReinitializing", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Reset", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("Start", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Stop", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UpdateCurrentInstanceGlobal", new TES5BasicType[] {
                                TES5BasicType.T_GLOBALVARIABLE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetQuest", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("GetID", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetPriority", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNumAliases", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetSpellCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetSkin", new TES5BasicType[] {
                            }, TES5BasicType.T_ARMOR),
                new TES5InheritanceFunctionSignature("SetSkin", new TES5BasicType[] {
                                TES5BasicType.T_ARMOR
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumPlayableRaces", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetNthPlayableRace", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("GetRace", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_RACE),
                new TES5InheritanceFunctionSignature("IsPlayable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakePlayable", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeUnplayable", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsChildRace", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeChildRace", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonChildRace", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanFly", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanFly", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonFlying", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanSwim", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanSwim", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonSwimming", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CanWalk", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeCanWalk", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeNonWalking", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsImmobile", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeImmobile", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakeMobile", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsNotPushable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeNotPushable", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("MakePushable", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoKnockdowns", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("MakeNoKnockdowns", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoKNockdowns", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoCombatInWater", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetNoCombatInWater", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoCombatInWater", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AvoidsRoads", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAvoidsRoads", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAvoidsRoads", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AllowPickpocket", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAllowPickpocket", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAllowPickpocket", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("AllowPCDialogue", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetAllowPCDialogue", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearAllowPCDialogue", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("CantOpenDoors", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetCantOpenDoors", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearCantOpenDoors", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("NoShadow", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetNoShadow", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ClearNoShadow", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_SCROLL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Cast", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE,
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPerk", new TES5BasicType[] {
                            }, TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("GetNumEffects", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new TES5BasicType[] {
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
                new TES5InheritanceFunctionSignature("GetEquipType", new TES5BasicType[] {
                            }, TES5BasicType.T_EQUIPSLOT),
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
                new TES5InheritanceFunctionSignature("GetVersion", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionMinor", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionBeta", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetVersionRelease", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetScriptVersionRelease", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetPluginVersion", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_INT)
            }
        },
        { TES5BasicType.T_SOULGEM,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSoulSize", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetGemSize", new TES5BasicType[] {
                            }, TES5BasicType.T_INT)
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
                new TES5InheritanceFunctionSignature("GetDescriptor", new TES5BasicType[] {
                            }, TES5BasicType.T_SOUNDDESCRIPTOR)
            }
        },
        { TES5BasicType.T_SOUNDDESCRIPTOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDecibelAttenuation", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetDecibelAttenuation", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetDecibelVariance", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetDecibelVariance", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFrequencyVariance", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetFrequencyVariance", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetFrequencyShift", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("IsHostile", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Preload", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Unload", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetPerk", new TES5BasicType[] {
                            }, TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("GetNumEffects", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetMagickaCost", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetEquipType", new TES5BasicType[] {
                            }, TES5BasicType.T_EQUIPSLOT),
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
                new TES5InheritanceFunctionSignature("GetNumTexturePaths", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("IsTextInputEnabled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_UTILITY,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GameTimeToString", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetCurrentGameTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCurrentRealTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("IsInMenuMode", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("StartFrameRateCapture", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("EndFrameRateCapture", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetAverageFrameRate", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMinFrameRate", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetMaxFrameRate", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetCurrentMemory", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetBudgetCount", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("GetBaseDamage", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetBaseDamage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritDamage", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetCritDamage", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetReach", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetReach", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMinRange", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetMinRange", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMaxRange", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetMaxRange", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSpeed", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetSpeed", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetStagger", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetStagger", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetWeaponType", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetWeaponType", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetModelPath", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetModelPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetIconPath", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetMessageIconPath", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetMessageIconPath", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantment", new TES5BasicType[] {
                            }, TES5BasicType.T_ENCHANTMENT),
                new TES5InheritanceFunctionSignature("SetEnchantment", new TES5BasicType[] {
                                TES5BasicType.T_ENCHANTMENT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantmentValue", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetEnchantmentValue", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquippedModel", new TES5BasicType[] {
                            }, TES5BasicType.T_STATIC),
                new TES5InheritanceFunctionSignature("SetEquippedModel", new TES5BasicType[] {
                                TES5BasicType.T_STATIC
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipType", new TES5BasicType[] {
                            }, TES5BasicType.T_EQUIPSLOT),
                new TES5InheritanceFunctionSignature("SetEquipType", new TES5BasicType[] {
                                TES5BasicType.T_EQUIPSLOT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkill", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResist", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetResist", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritEffect", new TES5BasicType[] {
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("SetCritEffect", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritEffectOnDeath", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("SetCritEffectOnDeath", new TES5BasicType[] {
                                TES5BasicType.T_BOOL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCritMultiplier", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetCritMultiplier", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("IsBattleaxe", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsBow", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsDagger", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsGreatsword", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsMace", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsStaff", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsSword", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWarhammer", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsWarAxe", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_WEATHER,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ReleaseOverride", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetClassification", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCurrentWeather", new TES5BasicType[] {
                            }, TES5BasicType.T_WEATHER),
                new TES5InheritanceFunctionSignature("GetOutgoingWeather", new TES5BasicType[] {
                            }, TES5BasicType.T_WEATHER),
                new TES5InheritanceFunctionSignature("GetCurrentWeatherTransition", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSkyMode", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetSunGlare", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetSunDamage", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWindDirection", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetWindDirectionRange", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
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
                new TES5InheritanceFunctionSignature("Clear", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ForceRefIfEmpty", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("ForceRefTo", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetActorRef", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetActorReference", new TES5BasicType[] {
                            }, TES5BasicType.T_ACTOR),
                new TES5InheritanceFunctionSignature("GetRef", new TES5BasicType[] {
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("GetReference", new TES5BasicType[] {
                            }, TES5BasicType.T_OBJECTREFERENCE),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("TryToAddToFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToClear", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToDisable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToDisableNoWait", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEnable", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEnableNoWait", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToEvaluatePackage", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToKill", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToMoveTo", new TES5BasicType[] {
                                TES5BasicType.T_OBJECTREFERENCE
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToRemoveFromFaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToReset", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("TryToStopCombat", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL)
            }
        },
        { TES5BasicType.T_LOCATIONALIAS,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Clear", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetLocation", new TES5BasicType[] {
                            }, TES5BasicType.T_LOCATION),
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
                new TES5InheritanceFunctionSignature("GetConfigName", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetPlatformName", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("GetVersionNumber", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("MessageBox", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Notification", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("OpenUserLog", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("QuitGame", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("StartStackProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopScriptProfiling", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("StopStackProfiling", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleAI", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleCollisions", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("ToggleMenus", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
        { TES5BasicType.T_ACTION,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_MAGICEFFECT,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetAssociatedSkill", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
                new TES5InheritanceFunctionSignature("SetAssociatedSkill", new TES5BasicType[] {
                                TES5BasicType.T_STRING
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetResistance", new TES5BasicType[] {
                            }, TES5BasicType.T_STRING),
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
                new TES5InheritanceFunctionSignature("GetCastTime", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetCastTime", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkillLevel", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetSkillLevel", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetArea", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetArea", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetSkillUsageMult", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetSkillUsageMult", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetBaseCost", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("SetBaseCost", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetLight", new TES5BasicType[] {
                            }, TES5BasicType.T_LIGHT),
                new TES5InheritanceFunctionSignature("SetLight", new TES5BasicType[] {
                                TES5BasicType.T_LIGHT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHitShader", new TES5BasicType[] {
                            }, TES5BasicType.T_EFFECTSHADER),
                new TES5InheritanceFunctionSignature("SetHitShader", new TES5BasicType[] {
                                TES5BasicType.T_EFFECTSHADER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantShader", new TES5BasicType[] {
                            }, TES5BasicType.T_EFFECTSHADER),
                new TES5InheritanceFunctionSignature("SetEnchantShader", new TES5BasicType[] {
                                TES5BasicType.T_EFFECTSHADER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetProjectile", new TES5BasicType[] {
                            }, TES5BasicType.T_PROJECTILE),
                new TES5InheritanceFunctionSignature("SetProjectile", new TES5BasicType[] {
                                TES5BasicType.T_PROJECTILE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetExplosion", new TES5BasicType[] {
                            }, TES5BasicType.T_EXPLOSION),
                new TES5InheritanceFunctionSignature("SetExplosion", new TES5BasicType[] {
                                TES5BasicType.T_EXPLOSION
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetCastingArt", new TES5BasicType[] {
                            }, TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetCastingArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetHitEffectArt", new TES5BasicType[] {
                            }, TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetHitEffectArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEnchantArt", new TES5BasicType[] {
                            }, TES5BasicType.T_ART),
                new TES5InheritanceFunctionSignature("SetEnchantArt", new TES5BasicType[] {
                                TES5BasicType.T_ART
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetImpactDataSet", new TES5BasicType[] {
                            }, TES5BasicType.T_IMPACTDATASET),
                new TES5InheritanceFunctionSignature("SetImpactDataSet", new TES5BasicType[] {
                                TES5BasicType.T_IMPACTDATASET
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetEquipAbility", new TES5BasicType[] {
                            }, TES5BasicType.T_SPELL),
                new TES5InheritanceFunctionSignature("SetEquipAbility", new TES5BasicType[] {
                                TES5BasicType.T_SPELL
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetImageSpaceMod", new TES5BasicType[] {
                            }, TES5BasicType.T_IMAGESPACEMODIFIER),
                new TES5InheritanceFunctionSignature("SetImageSpaceMod", new TES5BasicType[] {
                                TES5BasicType.T_IMAGESPACEMODIFIER
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetPerk", new TES5BasicType[] {
                            }, TES5BasicType.T_PERK),
                new TES5InheritanceFunctionSignature("SetPerk", new TES5BasicType[] {
                                TES5BasicType.T_PERK
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_FURNITURE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_TALKINGACTIVATOR,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_ACTIVATOR,
            new TES5InheritanceFunctionSignature[] {
            }
        },
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
        { TES5BasicType.T_KEY,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_MISCOBJECT,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_AMMO,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_ASSOCIATIONTYPE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_MUSICTYPE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Remove", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_CLASS,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_PACKAGE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new TES5BasicType[] {
                            }, TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("GetTemplate", new TES5BasicType[] {
                            }, TES5BasicType.T_PACKAGE)
            }
        },
        { TES5BasicType.T_CONTAINER,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_DOOR,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_EFFECTSHADER,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_PROJECTILE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_ENCOUNTERZONE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_SCENE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ForceStart", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetOwningQuest", new TES5BasicType[] {
                            }, TES5BasicType.T_QUEST),
                new TES5InheritanceFunctionSignature("IsActionComplete", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlaying", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("Start", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Stop", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_EXPLOSION,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_FACTION,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("CanPayCrimeGold", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("GetCrimeGold", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeGoldNonViolent", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetCrimeGoldViolent", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamy", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamyNonViolent", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetInfamyViolent", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetReaction", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStolenItemValueCrime", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("GetStolenItemValueNoCrime", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("IsFactionInCrimeGroup", new TES5BasicType[] {
                                TES5BasicType.T_FACTION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsPlayerExpelled", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
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
                new TES5InheritanceFunctionSignature("GetSize", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("HasForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("RemoveAddedForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Revert", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_GLOBALVARIABLE,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetValue", new TES5BasicType[] {
                            }, TES5BasicType.T_FLOAT),
                new TES5InheritanceFunctionSignature("GetValueInt", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
        { TES5BasicType.T_HAZARD,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_SOUNDCATEGORY,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Mute", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Pause", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetFrequency", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("SetVolume", new TES5BasicType[] {
                                TES5BasicType.T_FLOAT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnMute", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("UnPause", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_IDLE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
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
                new TES5InheritanceFunctionSignature("Remove", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_STATIC,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_IMPACTDATASET,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_TOPIC,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", new TES5BasicType[] {
                            }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_LOCATIONREFTYPE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_TOPICINFO,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new TES5BasicType[] {
                            }, TES5BasicType.T_QUEST)
            }
        },
        { TES5BasicType.T_LEVELEDACTOR,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("Revert", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
                new TES5InheritanceFunctionSignature("Revert", new TES5BasicType[] {
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceNone", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetChanceNone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceGlobal", new TES5BasicType[] {
                            }, TES5BasicType.T_GLOBALVARIABLE),
                new TES5InheritanceFunctionSignature("SetChanceGlobal", new TES5BasicType[] {
                                TES5BasicType.T_GLOBALVARIABLE
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
        { TES5BasicType.T_VOICETYPE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_LEVELEDSPELL,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new TES5BasicType[] {
                                TES5BasicType.T_FORM,
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetChanceNone", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
                new TES5InheritanceFunctionSignature("SetChanceNone", new TES5BasicType[] {
                                TES5BasicType.T_INT
                            }, TES5VoidType.Instance),
                new TES5InheritanceFunctionSignature("GetNumForms", new TES5BasicType[] {
                            }, TES5BasicType.T_INT),
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
        { TES5BasicType.T_LIGHT,
            new TES5InheritanceFunctionSignature[] {
            }
        },
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
                new TES5InheritanceFunctionSignature("IsCleared", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsChild", new TES5BasicType[] {
                                TES5BasicType.T_LOCATION
                            }, TES5BasicType.T_BOOL),
                new TES5InheritanceFunctionSignature("IsLoaded", new TES5BasicType[] {
                            }, TES5BasicType.T_BOOL),
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
        { TES5BasicType.T_WORDOFPOWER,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { TES5BasicType.T_WORLDSPACE,
            new TES5InheritanceFunctionSignature[] {
            }
        },
        //Conversion hooks,
        { TES5BasicType.T_TES4TIMERHELPER,
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDayOfWeek", new TES5BasicType[] { }, TES5BasicType.T_INT),
                /*new TES5InheritanceFunctionSignature("GetSecondsPassed", new TES5BasicType[] {
                        TES5BasicType.T_FLOAT
                    }, TES5BasicType.T_FLOAT),*///WTM:  Change:  Deleted
                new TES5InheritanceFunctionSignature("Rotate", new TES5BasicType[] {
                        TES5BasicType.T_OBJECTREFERENCE,
                        TES5BasicType.T_FLOAT,
                        TES5BasicType.T_FLOAT,
                        TES5BasicType.T_FLOAT
                    }, TES5VoidType.Instance)
            }
        },
        { TES5BasicType.T_TES4CONTAINER,
            new TES5InheritanceFunctionSignature[] {

            }
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
            for(int i=1;i<distinctTypes.Length;i++)
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

        public static TES5BasicType FindTypeByMethodParameter(TES5BasicType calledOnType, string methodName, int parameterIndex)
        {
            TES5InheritanceFunctionSignature[] callReturnsOfCalledOnType;
            if (!callReturns.TryGetValue(calledOnType, out callReturnsOfCalledOnType))
            {
                throw new ConversionException("Inference type exception - no methods found for " + calledOnType.Value+ "!");
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
            TES5InheritanceFunctionSignature[] callReturnsOfCalledOnType;
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
        public static TES5BasicType FindTypeByMethod(TES5ObjectCall objectCall, ESMAnalyzer esmAnalyzer)
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
                            throw new ConversionException("ESM <-> Inheritance Graph conflict.  ESM returned " + formType.Value+ ", which is not present in possible matches from inheritance graph:  " + extendingMatchesString);
                        }
                        return formType;
                    }
            }
        }
    }
}
