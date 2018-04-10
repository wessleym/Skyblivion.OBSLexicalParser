using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceGraphAnalyzer
    {
        private static Dictionary<ITES5Type, ITES5Type> inheritanceCache = new Dictionary<ITES5Type, ITES5Type>();
        private static TES5InheritanceItemCollection inheritance = new TES5InheritanceItemCollection()
        {
            { "Alias",
                new TES5InheritanceItemCollection()
                {
                    new TES5InheritanceItem("ReferenceAlias"), new TES5InheritanceItem("LocationAlias")
                }
            },
            { "Utility", new TES5InheritanceItemCollection() },
            { "ActiveMagicEffect", new TES5InheritanceItemCollection() },
            { "Debug", new TES5InheritanceItemCollection() },
            { "Game", new TES5InheritanceItemCollection() },
            { "Main", new TES5InheritanceItemCollection() },
            { "Math", new TES5InheritanceItemCollection() },
            { "Form", new TES5InheritanceItemCollection()
                {
                    { "Action" },
                    { "MagicEffect" },
                    { "Activator", new TES5InheritanceItemCollection(){"Furniture","Flora","TalkingActivator" } },
                    { "Message" },
                    { "ActorBase" },
                    { "MiscObject", new TES5InheritanceItemCollection(){
                        "Apparatus",
                        "ConstructibleObject",
                        "Key",
                        "SoulGem"
                    } },
                    { "Ammo" },
                    { "Armor" },
                    { "ArmorAddon" },
                    { "AssociationType" },
                    { "MusicType" },
                    { "Book" },
                    { "ObjectReference", new TES5InheritanceItemCollection(){
                        "Actor"
                    } },
                    { "Cell" },
                    { "Class" },
                    { "Outfit" },
                    { "ColorForm" },
                    { "Package" },
                    { "CombatStyle" },
                    { "Container" },
                    { "Perk" },
                    { "Door" },
                    { "Potion" },
                    { "EffectShader" },
                    { "Projectile" },
                    { "Enchantment" },
                    { "Quest", new TES5InheritanceItemCollection(){
                        "TES4TimerHelper",
                        "TES4Container"
                    } },
                    { "EncounterZone" },
                    { "Race" },
                    { "EquipSlot" },
                    { "Scene" },
                    { "Explosion" },
                    { "Faction" },
                    { "FormList" },
                    { "Scroll" },
                    { "GlobalVariable" },
                    { "Shout" },
                    { "Hazard" },
                    { "Sound" },
                    { "HeadPart" },
                    { "SoundCategory" },
                    { "Idle" },
                    { "Spell" },
                    { "ImageSpaceModifier" },
                    { "Static" },
                    { "ImpactDataSet" },
                    { "TextureSet" },
                    { "Ingredient" },
                    { "Topic" },
                    { "Keyword", new TES5InheritanceItemCollection(){
                        "LocationRefType"
                    } },
                    { "TopicInfo" },
                    { "LeveledActor" },
                    { "VisualEffect" },
                    { "LeveledItem" },
                    { "VoiceType" },
                    { "LeveledSpell" },
                    { "Weapon" },
                    { "Light" },
                    { "Weather" },
                    { "Location" },
                    { "WordOfPower" },
                    { "WorldSpace" }
                }
            },
            { "Input", new TES5InheritanceItemCollection() },
            { "SKSE", new TES5InheritanceItemCollection() },
            { "StringUtil", new TES5InheritanceItemCollection() },
            { "UI", new TES5InheritanceItemCollection() }
        };
        private static TES5InheritanceItem inheritanceAsItem = new TES5InheritanceItem(null, inheritance);

        //Regular Expression used to build tree from PHP:  ("[^"]+") =>[\r\n\s]+new string\[\] \{\r\n\s+"args" =>[\r\n\s]+(new string\[\] \{[^\}]*\}),[\r\n\s]+"returnType" => ("[^"]+"),?[\r\n\s]*}
        private static Dictionary<string, TES5InheritanceFunctionSignature[]> callReturns = new Dictionary<string, TES5InheritanceFunctionSignature[]>()
        {
            { "ActiveMagicEffect",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Dispel", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetBaseObject", new string[] {
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCasterActor", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetTargetActor", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetDuration", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetTimeElapsed", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("RegisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendModEvent", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new string[] {
                                "int"
                            }, "void")
                }
            },
        { "Actor",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ModFavorPoints", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModFavorPointsWithGlobal", new string[] {
                                "GlobalVariable"
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakePlayerFriend", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddPerk", new string[] {
                                "Perk"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddShout", new string[] {
                                "Shout"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("AddSpell", new string[] {
                                "Spell",
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("AllowBleedoutDialogue", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AllowPCDialogue", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AttachAshPile", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("CanFlyHere", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("ClearArrested", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearExpressionOverride", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearExtraArrows", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearForcedLandingMarker", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearKeepOffsetFromActor", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearLookAt", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("DamageActorValue", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DamageAV", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Dismount", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("DispelAllSpells", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("DispelSpell", new string[] {
                                "Spell"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("DoCombatSpellApply", new string[] {
                                "Spell",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EnableAI", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EndDeferredKill", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("EquipItem", new string[] {
                                "Form",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EquipShout", new string[] {
                                "Shout"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EquipSpell", new string[] {
                                "Spell",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EvaluatePackage", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceActorValue", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceAV", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetActorBase", new string[] {
                            }, "ActorBase"),
                new TES5InheritanceFunctionSignature("GetActorValue", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetActorValuePercentage", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAV", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAVPercentage", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetBaseActorValue", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetBaseAV", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetBribeAmount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetCrimeFaction", new string[] {
                            }, "Faction"),
                new TES5InheritanceFunctionSignature("GetCombatState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetCombatTarget", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetCurrentPackage", new string[] {
                            }, "Package"),
                new TES5InheritanceFunctionSignature("GetDialogueTarget", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetEquippedItemType", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetEquippedShout", new string[] {
                            }, "Shout"),
                new TES5InheritanceFunctionSignature("GetEquippedWeapon", new string[] {
                                "bool"
                            }, "Weapon"),
                new TES5InheritanceFunctionSignature("GetEquippedShield", new string[] {
                            }, "Armor"),
                new TES5InheritanceFunctionSignature("GetEquippedSpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("GetFactionRank", new string[] {
                                "Faction"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetFactionReaction", new string[] {
                                "Actor"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetFlyingState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetForcedLandingMarker", new string[] {
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("GetGoldAmount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetHighestRelationshipRank", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetKiller", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetLevel", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetLightLevel", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetLowestRelationshipRank", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetLeveledActorBase", new string[] {
                            }, "ActorBase"),
                new TES5InheritanceFunctionSignature("GetNoBleedoutRecovery", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetPlayerControls", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetRace", new string[] {
                            }, "Race"),
                new TES5InheritanceFunctionSignature("GetRelationshipRank", new string[] {
                                "Actor"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetRestrained", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetSitState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetSleepState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetVoiceRecoveryTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("HasAssociation", new string[] {
                                "AssociationType",
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasFamilyRelationship", new string[] {
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasLOS", new string[] {
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasMagicEffect", new string[] {
                                "MagicEffect"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasMagicEffectWithKeyword", new string[] {
                                "Keyword"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasParentRelationship", new string[] {
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasPerk", new string[] {
                                "Perk"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasSpell", new string[] {
                                "Form"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsAlarmed", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsAlerted", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsAllowedToFly", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsArrested", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsArrestingTarget", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsBeingRidden", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsBleedingOut", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsBribed", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsChild", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsCommandedActor", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDead", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDetectedBy", new string[] {
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDoingFavor", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsEquipped", new string[] {
                                "Form"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsEssential", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFlying", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsGuard", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsGhost", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsHostileToActor", new string[] {
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInCombat", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInFaction", new string[] {
                                "Faction"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInKillMove", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsIntimidated", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsOnMount", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsPlayersLastRiddenHorse", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsPlayerTeammate", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsRunning", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsSneaking", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsSprinting", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsTorchOut" ,new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsTrespassing", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsUnconscious", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsWeaponDrawn", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("KeepOffsetFromActor", new string[] {
                                "Actor",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Kill", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("KillEssential", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("KillSilent", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModActorValue", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModAV", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModFactionRank", new string[] {
                                "Faction",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("MoveToPackageLocation", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("OpenInventory", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("PathToReference", new string[] {
                                "ObjectReference",
                                "float"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayIdle", new string[] {
                                "Idle"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayIdleWithTarget", new string[] {
                                "Idle",
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlaySubGraphAnimation", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveFromFaction", new string[] {
                                "Faction"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveFromAllFactions", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemovePerk", new string[] {
                                "Perk"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveShout", new string[] {
                                "Shout"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RemoveSpell", new string[] {
                                "Spell"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("ResetHealthAndLimbs", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RestoreActorValue", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Resurrect", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RestoreAV", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendTrespassAlarm", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActorValue", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAlert", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAllowFlying", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAllowFlyingEx", new string[] {
                                "bool",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAlpha", new string[] {
                                "float",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAttackActorOnSight", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAV", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetBribed", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCrimeFaction", new string[] {
                                "Faction"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCriticalStage", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetDoingFavor", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetDontMove", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetExpressionOverride", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetEyeTexture", new string[] {
                                "TextureSet"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFactionRank", new string[] {
                                "Faction",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetForcedLandingMarker", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGhost", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddToFaction", new string[] {
                                "Faction"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetHeadTracking", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetIntimidated", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetLookAt", new string[] {
                                "ObjectReference",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNoBleedoutRecovery", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNotShowOnStealthMeter", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetOutfit", new string[] {
                                "Outfit",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerControls", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerResistingArrest", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerTeammate", new string[] {
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetRace", new string[] {
                                "Race"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetRelationshipRank", new string[] {
                                "Actor",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetRestrained", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetSubGraphFloatVariable", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetUnconscious", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetVehicle", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetVoiceRecoveryTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowBarterMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowGiftMenu", new string[] {
                                "bool",
                                "FormList",
                                "bool",
                                "bool"
                            }, "int"),
                new TES5InheritanceFunctionSignature("StartCannibal", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartCombat", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartDeferredKill", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartVampireFeed", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopCombat", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopCombatAlarm", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("TrapSoul", new string[] {
                                "Actor"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("UnequipAll", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnequipItem", new string[] {
                                "Form",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnequipItemSlot", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnequipShout", new string[] {
                                "Shout"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnequipSpell", new string[] {
                                "Spell",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnLockOwnedDoorsInCell", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("WillIntimidateSucceed", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("WornHasKeyword", new string[] {
                                "Keyword"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("StartSneaking", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("DrawWeapon", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementDirection", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementSpeed", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementRotationSpeed", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementDirectionRamp", new string[] {
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementSpeedRamp", new string[] {
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceMovementRotationSpeedRamp", new string[] {
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceTargetDirection", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceTargetSpeed", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceTargetAngle", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearForcedMovement", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWornForm", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetWornItemId", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetEquippedObject", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetEquippedItemId", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetSpellCount", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetNthSpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("QueueNiNodeUpdate", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegenerateHead", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("EquipItemEx", new string[] {
                                "Form",
                                "int",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EquipItemById", new string[] {
                                "Form",
                                "int",
                                "int",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnequipItemEx", new string[] {
                                "Form",
                                "int",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ChangeHeadPart", new string[] {
                                "HeadPart"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UpdateWeight", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsAIEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsSwimming", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SheatheWeapon", new string[] {
                            }, "void")
            }
        },
        { "ActorBase",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetClass", new string[] {
                            }, "Class"),
                new TES5InheritanceFunctionSignature("GetDeadCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetGiftFilter", new string[] {
                            }, "FormList"),
                new TES5InheritanceFunctionSignature("GetRace", new string[] {
                            }, "Race"),
                new TES5InheritanceFunctionSignature("GetSex", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("IsEssential", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInvulnerable", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsProtected", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsUnique", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetEssential", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetInvulnerable", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetProtected", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetOutfit", new string[] {
                                "Outfit",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCombatStyle", new string[] {
                            }, "CombatStyle"),
                new TES5InheritanceFunctionSignature("SetCombatStyle", new string[] {
                                "CombatStyle"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetOutfit", new string[] {
                                "bool"
                            }, "Outfit"),
                new TES5InheritanceFunctionSignature("SetClass", new string[] {
                                "Class"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetHeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetHeight", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetWeight", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumHeadParts", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthHeadPart", new string[] {
                                "int"
                            }, "HeadPart"),
                new TES5InheritanceFunctionSignature("SetNthHeadPart", new string[] {
                                "HeadPart",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetIndexOfHeadPartByType", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetFaceMorph", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetFaceMorph", new string[] {
                                "float",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetFacePreset", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetFacePreset", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetHairColor", new string[] {
                            }, "ColorForm"),
                new TES5InheritanceFunctionSignature("SetHairColor", new string[] {
                                "ColorForm"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSpellCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthSpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("GetFaceTextureSet", new string[] {
                            }, "TextureSet"),
                new TES5InheritanceFunctionSignature("SetFaceTextureSet", new string[] {
                                "TextureSet"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetVoiceType", new string[] {
                            }, "VoiceType"),
                new TES5InheritanceFunctionSignature("SetVoiceType", new string[] {
                                "VoiceType"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkin", new string[] {
                            }, "Armor"),
                new TES5InheritanceFunctionSignature("SetSkin", new string[] {
                                "Armor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkinFar", new string[] {
                            }, "Armor"),
                new TES5InheritanceFunctionSignature("SetSkinFar", new string[] {
                                "Armor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetTemplate", new string[] {
                            }, "ActorBase")
            }
        },
        { "Alias",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new string[] {
                            }, "Quest"),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetName", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetID", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("RegisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendModEvent", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new string[] {
                                "int"
                            }, "void")
            }
        },
        { "Apparatus",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetQuality", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetQuality", new string[] {
                                "int"
                            }, "void")
            }
        },
        { "Armor",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetArmorRating", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetAR", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetArmorRating", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAR", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModArmorRating", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModAR", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetModelPath", new string[] {
                                "bool"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetModelPath", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetIconPath", new string[] {
                                "bool"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetIconPath", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMessageIconPath", new string[] {
                                "bool"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetMessageIconPath", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWeightClass", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetWeightClass", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEnchantment", new string[] {
                            }, "Enchantment"),
                new TES5InheritanceFunctionSignature("SetEnchantment", new string[] {
                                "Enchantment"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsLightArmor", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsHeavyArmor", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothing", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsBoots", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsCuirass", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsGauntlets", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsHelmet", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsShield", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsJewelry", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingHead", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingBody", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingFeet", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingHands", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingRing", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingRich", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsClothingPoor", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetSlotMask", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetSlotMask", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddSlotToMask", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("RemoveSlotFromMask", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetMaskForSlot", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNumArmorAddons", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthArmorAddon", new string[] {
                                "int"
                            }, "ArmorAddon")
            }
        },
        { "ArmorAddon",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetModelPath", new string[] {
                                "bool",
                                "bool"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetModelPath", new string[] {
                                "string",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetModelNumTextureSets", new string[] {
                                "bool",
                                "bool"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetModelNthTextureSet", new string[] {
                                "int",
                                "bool",
                                "bool"
                            }, "TextureSet"),
                new TES5InheritanceFunctionSignature("SetModelNthTextureSet", new string[] {
                                "TextureSet",
                                "int",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumAdditionalRaces", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthAdditionalRace", new string[] {
                                "int"
                            }, "Race"),
                new TES5InheritanceFunctionSignature("GetSlotMask", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetSlotMask", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddSlotToMask", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("RemoveSlotFromMask", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetMaskForSlot", new string[] {
                                "int"
                            }, "int")
            }
        },
        { "Book",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSpell", new string[] {
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("GetSkill", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("IsRead", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsTakeable", new string[] {
                            }, "bool")
            }
        },
        { "Cell",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetActorOwner", new string[] {
                            }, "ActorBase"),
                new TES5InheritanceFunctionSignature("GetFactionOwner", new string[] {
                            }, "Faction"),
                new TES5InheritanceFunctionSignature("IsAttached", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInterior", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Reset", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActorOwner", new string[] {
                                "ActorBase"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFactionOwner", new string[] {
                                "Faction"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFogPlanes", new string[] {
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFogPower", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPublic", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumRefs", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthRef", new string[] {
                                "int",
                                "int"
                            }, "ObjectReference")
            }
        },
        /* Removed due to GetValue() conflict, i will think how to readd this . todo,
        { "ColorComponent", 
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetAlpha", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetRed", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetGreen", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetBlue", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetHue", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSaturation", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetValue", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetAlpha", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetRed", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetGreen", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetBlue", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetHue", new string[] {
                                "int",
                                "float"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetSaturation", new string[] {
                                "int",
                                "float"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetValue", new string[] {
                                "int",
                                "float"
                            }, "int")
            }
        },
        { "ColorForm", 
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetColor", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetColor", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetRed", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetGreen", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetBlue", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetHue", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSaturation", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetValue", new string[] {
                            }, "float")
            },*/
        { "CombatStyle",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOffensiveMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetDefensiveMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetGroupOffensiveMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAvoidThreatChance", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetRangedMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMagicMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetShoutMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetStaffMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetUnarmedMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetOffensiveMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetDefensiveMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGroupOffensiveMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAvoidThreatChance", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetRangedMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMagicMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetShoutMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetStaffMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetUnarmedMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMeleeAttackStaggeredMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackStaggeredMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleePowerAttackBlockingMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeBashMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeBashRecoiledMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeBashAttackMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeBashPowerAttackMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMeleeSpecialAttackMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAllowDualWielding", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetMeleeAttackStaggeredMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleePowerAttackStaggeredMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleePowerAttackBlockingMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeBashMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeBashRecoiledMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeBashAttackMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeBashPowerAttackMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMeleeSpecialAttackMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAllowDualWielding", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingCircleMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetCloseRangeDuelingFallbackMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingFlankDistance", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetCloseRangeFlankingStalkTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetCloseRangeDuelingCircleMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCloseRangeDuelingFallbackMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCloseRangeFlankingFlankDistance", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCloseRangeFlankingStalkTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetLongRangeStrafeMult", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetLongRangeStrafeMult", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetFlightHoverChance", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetFlightDiveBombChance", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetFlightFlyingAttackChance", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetFlightHoverChance", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFlightDiveBombChance", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFlightFlyingAttackChance", new string[] {
                                "float"
                            }, "void")
            }
        },
        { "ConstructibleObject",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetResult", new string[] {
                            }, "Form"),
                new TES5InheritanceFunctionSignature("SetResult", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetResultQuantity", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetResultQuantity", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumIngredients", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthIngredient", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("SetNthIngredient", new string[] {
                                "Form",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNthIngredientQuantity", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthIngredientQuantity", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWorkbenchKeyword", new string[] {
                            }, "Keyword"),
                new TES5InheritanceFunctionSignature("SetWorkbenchKeyword", new string[] {
                                "Keyword"
                            }, "void")
            }
        },
        { "DwarvenMechScript",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetFormIndex", new string[] {
                                "FormList",
                                "Form"
                            }, "int")
            }
        },
        { "Enchantment",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNumEffects", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new string[] {
                                "int"
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new string[] {
                            }, "int")
            }
        },
        { "EquipSlot",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParents", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthParent", new string[] {
                                "int"
                            }, "EquipSlot")
            }
        },
        { "Flora",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHarvestSound", new string[] {
                            }, "SoundDescriptor"),
                new TES5InheritanceFunctionSignature("SetHarvestSound", new string[] {
                                "SoundDescriptor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetIngredient", new string[] {
                            }, "Ingredient"),
                new TES5InheritanceFunctionSignature("SetIngredient", new string[] {
                                "Ingredient"
                            }, "void")
            }
        },
        { "Form",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetFormID", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetGoldValue", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("HasKeyword", new string[] {
                                "Keyword"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayerKnows", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RegisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RegisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSGain", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleLOSLost", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdate", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForSingleUpdateGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopObjectProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAnimationEvent", new string[] {
                                "ObjectReference",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForLOS", new string[] {
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForSleep", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForTrackedStatsEvent", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdate", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForUpdateGameTime", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetType", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetName", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetName", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetWeight", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGoldValue", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumKeywords", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthKeyword", new string[] {
                                "int"
                            }, "Keyword"),
                new TES5InheritanceFunctionSignature("HasKeywordString", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RegisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForKey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllKeys", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForControl", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllControls", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForMenu", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllMenus", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForModEvent", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForModEvent", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForAllModEvents", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendModEvent", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCameraState", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForCrosshairRef", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RegisterForActorAction", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnregisterForActorAction", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TempClone", new string[] {
                            }, "Form")
            }
        },
        { "FormType",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Game",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddAchievement", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddPerkPoints", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AdvanceSkill", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddHavokBallAndSocketConstraint", new string[] {
                                "ObjectReference",
                                "string",
                                "ObjectReference",
                                "string",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RemoveHavokConstraints", new string[] {
                                "ObjectReference",
                                "string",
                                "ObjectReference",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("CalculateFavorCost", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("ClearPrison", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearTempEffects", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("DisablePlayerControls", new string[] {
                                "bool",
                                "bool",
                                "bool",
                                "bool",
                                "bool",
                                "bool",
                                "bool",
                                "bool",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EnableFastTravel", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("FadeOutGame", new string[] {
                                "bool",
                                "bool",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("FastTravel", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfType", new string[] {
                                "Form",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfType", new string[] {
                                "Form",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfAnyTypeInList", new string[] {
                                "FormList",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfAnyTypeInList", new string[] {
                                "FormList",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfTypeFromRef", new string[] {
                                "Form",
                                "ObjectReference",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfTypeFromRef", new string[] {
                                "Form",
                                "ObjectReference",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindClosestReferenceOfAnyTypeInListFromRef", new string[] {
                                "FormList",
                                "ObjectReference",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindRandomReferenceOfAnyTypeInListFromRef", new string[] {
                                "FormList",
                                "ObjectReference",
                                "float"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("FindClosestActor", new string[] {
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("FindRandomActor", new string[] {
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("FindClosestActorFromRef", new string[] {
                                "ObjectReference",
                                "float"
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("FindRandomActorFromRef", new string[] {
                                "ObjectReference",
                                "float"
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("ForceThirdPerson", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceFirstPerson", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowFirstPersonGeometry", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetAmountSoldStolen", new string[] {}, "int"),
                new TES5InheritanceFunctionSignature("GetForm", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetFormFromFile", new string[] {
                                "int",
                                "string"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetGameSettingFloat", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetGameSettingInt", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetGameSettingString", new string[] {
                                "string"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetPlayer", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetPlayerGrabbedRef", new string[] {
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("GetPlayersLastRiddenHorse", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetSunPositionX", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSunPositionY", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSunPositionZ", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetRealHoursPassed", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("IncrementSkill", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IncrementSkillBy", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IncrementStat", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsActivateControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsCamSwitchControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFastTravelControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFastTravelEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFightingControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsJournalControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsLookingControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsMenuControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsMovementControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsPCAMurderer", new string[] {}, "int"),
                new TES5InheritanceFunctionSignature("IsPlayerSungazing", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsSneakingControlsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsWordUnlocked", new string[] {
                                "WordOfPower"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PrecacheCharGen", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("PrecacheCharGenClear", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("QueryStat", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("QuitToMainMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RequestAutoSave", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RequestModel", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RequestSave", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ServeTime", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendWereWolfTransformation", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetBeastForm", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCameraTarget", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetHudCartMode", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetInChargen", new string[] {
                                "bool",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerAIDriven", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerReportCrime", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetSittingRotation", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShakeCamera", new string[] {
                                "ObjectReference",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShakeController", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowRaceMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowLimitedRaceMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowTitleSequenceMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("HideTitleSequenceMenu", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartTitleSequence", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAllowFlyingMountLandingRequests", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetSunGazeImageSpaceModifier", new string[] {
                                "ImageSpaceModifier"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ShowTrainingMenu", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TeachWord", new string[] {
                                "WordOfPower"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TriggerScreenBlood", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnlockWord", new string[] {
                                "WordOfPower"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UsingGamepad", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetPerkPoints", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetPerkPoints", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModPerkPoints", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetModCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetModByName", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetModName", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetModAuthor", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetModDescription", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetModDependencyCount", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthModDependency", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetGameSettingFloat", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGameSettingInt", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGameSettingBool", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGameSettingString", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SaveGame", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("LoadGame", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumTintMasks", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthTintMaskColor", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthTintMaskType", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthTintMaskColor", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNthTintMaskTexturePath", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetNthTintMaskTexturePath", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumTintsByType", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetTintMaskColor", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetTintMaskColor", new string[] {
                                "int",
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetTintMaskTexturePath", new string[] {
                                "int",
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetTintMaskTexturePath", new string[] {
                                "string",
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UpdateTintMaskColors", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UpdateHairColor", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCameraState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetMiscStat", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayersLastRiddenHorse", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkillLegendaryLevel", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetSkillLegendaryLevel", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetPlayerMovementMode", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("UpdateThirdPerson", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnbindObjectHotkey", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetHotkeyBoundObject", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("IsObjectFavorited", new string[] {
                                "Form"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayBink", new string[] {
                                "string",
                                "bool",
                                "bool",
                                "bool",
                                "bool"
                            }, "bool")
            }
        },
        { "HeadPart",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetHeadPart", new string[] {
                                "string"
                            }, "HeadPart"),
                new TES5InheritanceFunctionSignature("GetType", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNumExtraParts", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthExtraPart", new string[] {
                                "int"
                            }, "HeadPart"),
                new TES5InheritanceFunctionSignature("HasExtraPart", new string[] {
                                "HeadPart"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetIndexOfExtraPart", new string[] {
                                "HeadPart"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetValidRaces", new string[] {
                            }, "FormList"),
                new TES5InheritanceFunctionSignature("SetValidRaces", new string[] {
                                "FormList"
                            }, "void")
            }
        },
        { "Ingredient",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("LearnEffect", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("LearnNextEffect", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("LearnAllEffects", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumEffects", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new string[] {
                                "int"
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new string[] {
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new string[] {
                                "int",
                                "int"
                            }, "void")
            }
        },
        { "Input",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsKeyPressed", new string[] {
                                "Int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("TapKey", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("HoldKey", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ReleaseKey", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumKeysPressed", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthKeyPressed", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetMappedKey", new string[] {
                                "string",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetMappedControl", new string[] {
                                "int"
                            }, "string")
            }
        },
        { "Keyword",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetKeyword", new string[] {
                                "string"
                            }, "Keyword"),
                new TES5InheritanceFunctionSignature("GetString", new string[] {
                            }, "string")
            }
        },
        { "Math",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("abs", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("acos", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("asin", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("atan", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("Ceiling", new string[] {
                                "float"
                            }, "int"),
                new TES5InheritanceFunctionSignature("cos", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("DegreesToRadians", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("Floor", new string[] {
                                "float"
                            }, "int"),
                new TES5InheritanceFunctionSignature("pow", new string[] {
                                "float",
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("RadiansToDegrees", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("sin", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("sqrt", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("tan", new string[] {
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("LeftShift", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("RightShift", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("LogicalAnd", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("LogicalOr", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("LogicalXor", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("LogicalNot", new string[] {
                                "int"
                            }, "int")
            }
        },
        { "ObjectReference",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("rampRumble", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsNearPlayer", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInInterior", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MoveToIfUnloaded", new string[] {
                                "ObjectReference",
                                "float",
                                "float",
                                "float"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MoveToWhenUnloaded", new string[] {
                                "ObjectReference",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("finishes", new string[] {
                                "and",
                                "the",
                                "1"
                            }, "this"),
                new TES5InheritanceFunctionSignature("DeleteWhenAble", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddKeyIfNeeded", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("get", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("Activate", new string[] {
                                "ObjectReference",
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("AddDependentAnimatedObjectReference", new string[] {
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddItem", new string[] {
                                "Form",
                                "int",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("AddToMap", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ApplyHavokImpulse", new string[] {
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("BlockActivation", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("CalculateEncounterLevel", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("CanFastTravelToMarker", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("ClearDestruction", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CreateDetectionEvent", new string[] {
                                "Actor",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DamageObject", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Delete", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Disable", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DisableNoWait", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DropObject", new string[] {
                                "Form",
                                "int"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("Enable", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EnableFastTravel", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("EnableNoWait", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceAddRagdollToWorld", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceRemoveRagdollFromWorld", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetActorOwner", new string[] {
                            }, "ActorBase"),
                new TES5InheritanceFunctionSignature("GetAngleX", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAngleY", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAngleZ", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetAnimationVariableBool", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetAnimationVariableInt", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetAnimationVariableFloat", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetBaseObject", new string[] {
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetCurrentDestructionStage", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetCurrentLocation", new string[] {
                            }, "Location"),
                new TES5InheritanceFunctionSignature("GetCurrentScene", new string[] {
                            }, "Scene"),
                new TES5InheritanceFunctionSignature("GetDestroyed", new string[] {}, "int"),
                new TES5InheritanceFunctionSignature("GetDistance", new string[] {
                                "ObjectReference"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetEditorLocation", new string[] {
                            }, "Location"),
                new TES5InheritanceFunctionSignature("GetEnableParent", new string[]{ }, "ObjectReference"),//Only SKSE function atm used
                new TES5InheritanceFunctionSignature("GetFactionOwner", new string[] {
                            }, "Faction"),
                new TES5InheritanceFunctionSignature("GetHeadingAngle", new string[] {
                                "ObjectReference"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetHeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetItemCount", new string[] {
                                "Form"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetItemHealthPercent", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetKey", new string[] {
                            }, "Key"),
                new TES5InheritanceFunctionSignature("GetLength", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetLinkedRef", new string[] {
                                "Keyword"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("GetLockLevel", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("countLinkedRefChain", new string[] {
                                "keyword",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthLinkedRef", new string[] {
                                "int"
                            }, "ObjectReference"),
                //WTM:  Change:  These methods don't seem to exist for ObjectReference:
                /*new TES5InheritanceFunctionSignature("GetStartingAngle", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetStartingPos", new string[] {
                                "string"
                            }, "float"),*/
                new TES5InheritanceFunctionSignature("EnableLinkChain", new string[] {
                                "Keyword"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DisableLinkChain", new string[] {
                                "Keyword",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMass", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetOpenState", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetParentCell", new string[] {
                            }, "Cell"),
                new TES5InheritanceFunctionSignature("GetPositionX", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetPositionY", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetPositionZ", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetScale", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetTriggerObjectCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetVoiceType", new string[] {
                            }, "VoiceType"),
                new TES5InheritanceFunctionSignature("GetWidth", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetWorldSpace", new string[] {
                            }, "WorldSpace"),
                new TES5InheritanceFunctionSignature("GetSelfAsActor", new string[] {
                            }, "actor"),
                new TES5InheritanceFunctionSignature("HasEffectKeyword", new string[] {
                                "Keyword"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasNode", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("HasRefType", new string[] {
                                "LocationRefType"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IgnoreFriendlyHits", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InterruptCast", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsActivateChild", new string[] {
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsActivationBlocked", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Is3DLoaded", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDeleted", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDisabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsEnabled", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFurnitureInUse", new string[] {
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFurnitureMarkerInUse", new string[] {
                                "int",
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsIgnoringFriendlyHits", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInDialogueWithPlayer", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsLockBroken", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsLocked", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsMapMarkerVisible", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("KnockAreaEffect", new string[] {
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Lock", new string[] {
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("MoveTo", new string[] {
                                "ObjectReference",
                                "float",
                                "float",
                                "float",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("MoveToInteractionLocation", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("MoveToMyEditorLocation", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MoveToNode", new string[] {
                                "ObjectReference",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("PlaceAtMe", new string[] {
                                "Form",
                                "int",
                                "bool",
                                "bool"
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("PlaceActorAtMe", new string[] {
                                "ActorBase",
                                "int",
                                "EncounterZone"
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("PlayAnimation", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayAnimationAndWait", new string[] {
                                "string",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayGamebryoAnimation", new string[] {
                                "string",
                                "bool",
                                "float"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayImpactEffect", new string[] {
                                "ImpactDataSet",
                                "string",
                                "float",
                                "float",
                                "float",
                                "float",
                                "bool",
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlaySyncedAnimationSS", new string[] {
                                "string",
                                "ObjectReference",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlaySyncedAnimationAndWaitSS", new string[] {
                                "string",
                                "string",
                                "ObjectReference",
                                "string",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PlayTerrainEffect", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ProcessTrapHit", new string[] {
                                "ObjectReference",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("PushActorAway", new string[] {
                                "Actor",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveAllItems", new string[] {
                                "ObjectReference",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveItem", new string[] {
                                "Form",
                                "int",
                                "bool",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveDependentAnimatedObjectReference", new string[] {
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Reset", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Say", new string[] {
                                "Topic",
                                "Actor",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendStealAlarm", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActorCause", new string[] {
                                "Actor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActorOwner", new string[] {
                                "ActorBase"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAngle", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAnimationVariableBool", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAnimationVariableInt", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetAnimationVariableFloat", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetDestroyed", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFactionOwner", new string[] {
                                "Faction"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetLockLevel", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetMotionType", new string[] {
                                "int",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNoFavorAllowed", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetOpen", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPosition", new string[] {
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetScale", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TranslateTo", new string[] {
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SplineTranslateTo", new string[] {
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SplineTranslateToRefNode", new string[] {
                                "ObjectReference",
                                "string",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopTranslation", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("TranslateToRef", new string[] {
                                "ObjectReference",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SplineTranslateToRef", new string[] {
                                "ObjectReference",
                                "float",
                                "float",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TetherToHorse", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("WaitForAnimationEvent", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsInLocation", new string[] {
                                "Location"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNumItems", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthForm", new string[] {
                                "int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetTotalItemWeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetTotalArmorWeight", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("IsHarvested", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetHarvested", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetItemHealthPercent", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetItemMaxCharge", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetItemCharge", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetItemCharge", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ResetInventory", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsOffLimits", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("isAnimPlaying", new string[] {
                            }, "int")
            }
        },
        { "Outfit",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumParts", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthPart", new string[] {
                                "int"
                            }, "Form")
            }
        },
        { "Perk",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumEntries", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEntryRank", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEntryRank", new string[] {
                                "int",
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryPriority", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEntryPriority", new string[] {
                                "int",
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryQuest", new string[] {
                                "int"
                            }, "Quest"),
                new TES5InheritanceFunctionSignature("SetNthEntryQuest", new string[] {
                                "int",
                                "Quest"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryStage", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEntryStage", new string[] {
                                "int",
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntrySpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("SetNthEntrySpell", new string[] {
                                "int",
                                "Spell"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryLeveledList", new string[] {
                                "int"
                            }, "LeveledItem"),
                new TES5InheritanceFunctionSignature("SetNthEntryLeveledList", new string[] {
                                "int",
                                "LeveledItem"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryText", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetNthEntryText", new string[] {
                                "int",
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNthEntryValue", new string[] {
                                "int",
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetNthEntryValue", new string[] {
                                "int",
                                "int",
                                "float"
                            }, "bool")
            }
        },
        { "Potion",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsHostile", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsFood", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetNumEffects", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new string[] {
                                "int"
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new string[] {
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new string[] {
                                "int",
                                "int"
                            }, "void")
            }
        },
        { "Quest",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ModObjectiveGlobal", new string[] {
                                "float",
                                "GlobalVariable",
                                "int",
                                "float",
                                "bool",
                                "bool",
                                "bool"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("CompleteAllObjectives", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CompleteQuest", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("FailAllObjectives", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetAlias", new string[] {
                                "int"
                            }, "Alias"),
                new TES5InheritanceFunctionSignature("GetCurrentStageID", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetStage", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetStageDone", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsActive", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsCompleted", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsObjectiveCompleted", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsObjectiveDisplayed", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsObjectiveFailed", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsRunning", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsStageDone", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsStarting", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsStopping", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsStopped", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("PrepareForReinitializing", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Reset", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActive", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCurrentStageID", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetObjectiveCompleted", new string[] {
                                "int",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetObjectiveDisplayed", new string[] {
                                "int",
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetObjectiveFailed", new string[] {
                                "int",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetStage", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Start", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Stop", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UpdateCurrentInstanceGlobal", new string[] {
                                "GlobalVariable"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetQuest", new string[] {
                                "string"
                            }, "Quest"),
                new TES5InheritanceFunctionSignature("GetID", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetPriority", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNumAliases", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthAlias", new string[] {
                                "int"
                            }, "Alias"),
                new TES5InheritanceFunctionSignature("GetAliasByName", new string[] {
                                "string"
                            }, "Alias")
            }
        },
        { "Race",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSpellCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthSpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("IsRaceFlagSet", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetRaceFlag", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearRaceFlag", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetDefaultVoiceType", new string[] {
                                "bool"
                            }, "VoiceType"),
                new TES5InheritanceFunctionSignature("SetDefaultVoiceType", new string[] {
                                "bool",
                                "VoiceType"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkin", new string[] {
                            }, "Armor"),
                new TES5InheritanceFunctionSignature("SetSkin", new string[] {
                                "Armor"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumPlayableRaces", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthPlayableRace", new string[] {
                                "int"
                            }, "Race"),
                new TES5InheritanceFunctionSignature("GetRace", new string[] {
                                "string"
                            }, "Race"),
                new TES5InheritanceFunctionSignature("IsPlayable", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakePlayable", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeUnplayable", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsChildRace", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeChildRace", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeNonChildRace", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CanFly", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeCanFly", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeNonFlying", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CanSwim", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeCanSwim", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeNonSwimming", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CanWalk", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeCanWalk", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeNonWalking", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsImmobile", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeImmobile", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakeMobile", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsNotPushable", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeNotPushable", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("MakePushable", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("NoKnockdowns", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("MakeNoKnockdowns", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearNoKNockdowns", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("NoCombatInWater", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetNoCombatInWater", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearNoCombatInWater", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("AvoidsRoads", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetAvoidsRoads", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearAvoidsRoads", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("AllowPickpocket", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetAllowPickpocket", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearAllowPickpocket", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("AllowPCDialogue", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetAllowPCDialogue", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearAllowPCDialogue", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("CantOpenDoors", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetCantOpenDoors", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearCantOpenDoors", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("NoShadow", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetNoShadow", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearNoShadow", new string[] {
                            }, "void")
            }
        },
        { "Scroll",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Cast", new string[] {
                                "ObjectReference",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCastTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetPerk", new string[] {
                            }, "Perk"),
                new TES5InheritanceFunctionSignature("GetNumEffects", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new string[] {
                                "int"
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new string[] {
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEquipType", new string[] {
                            }, "EquipSlot"),
                new TES5InheritanceFunctionSignature("SetEquipType", new string[] {
                                "EquipSlot"
                            }, "void")
            }
        },
        { "Shout",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNthWordOfPower", new string[] {
                                "int"
                            }, "WordOfPower"),
                new TES5InheritanceFunctionSignature("GetNthSpell", new string[] {
                                "int"
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("GetNthRecoveryTime", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetNthWordOfPower", new string[] {
                                "int",
                                "WordOfPower"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthSpell", new string[] {
                                "int",
                                "Spell"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthRecoveryTime", new string[] {
                                "int",
                                "float"
                            }, "void")
            }
        },
        { "SKSE",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetVersion", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetVersionMinor", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetVersionBeta", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetVersionRelease", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetScriptVersionRelease", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetPluginVersion", new string[] {
                                "string"
                            }, "int")
            }
        },
        { "SoulGem",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetSoulSize", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetGemSize", new string[] {
                            }, "int")
            }
        },
        { "Sound",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Play", new string[] {
                                "ObjectReference"
                            }, "int"),
                new TES5InheritanceFunctionSignature("PlayAndWait", new string[] {
                                "ObjectReference"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("StopInstance", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetInstanceVolume", new string[] {
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetDescriptor", new string[] {
                            }, "SoundDescriptor")
            }
        },
        { "SoundDescriptor",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDecibelAttenuation", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetDecibelAttenuation", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetDecibelVariance", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetDecibelVariance", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetFrequencyVariance", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetFrequencyVariance", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetFrequencyShift", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetFrequencyShift", new string[] {
                                "int"
                            }, "void")
            }
        },
        { "Spell",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Cast", new string[] {
                                "ObjectReference",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoteCast", new string[] {
                                "ObjectReference",
                                "Actor",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsHostile", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Preload", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Unload", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCastTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetPerk", new string[] {
                            }, "Perk"),
                new TES5InheritanceFunctionSignature("GetNumEffects", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagnitude", new string[] {
                                "int"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetNthEffectArea", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectDuration", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthEffectMagicEffect", new string[] {
                                "int"
                            }, "MagicEffect"),
                new TES5InheritanceFunctionSignature("GetCostliestEffectIndex", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetMagickaCost", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetEffectiveMagickaCost", new string[] {
                                "Actor"
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetNthEffectMagnitude", new string[] {
                                "int",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectArea", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNthEffectDuration", new string[] {
                                "int",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEquipType", new string[] {
                            }, "EquipSlot"),
                new TES5InheritanceFunctionSignature("SetEquipType", new string[] {
                                "EquipSlot"
                            }, "void")
            }
        },
        { "StringUtil",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetLength", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthChar", new string[] {
                                "string",
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("IsLetter", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDigit", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsPunctuation", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsPrintable", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("Find", new string[] {
                                "string",
                                "string",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("Substring", new string[] {
                                "string",
                                "int",
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("AsOrd", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("AsChar", new string[] {
                                "int"
                            }, "string")
            }
        },
        { "TextureSet",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetNumTexturePaths", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetNthTexturePath", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetNthTexturePath", new string[] {
                                "int",
                                "string"
                            }, "void")
            }
        },
        { "UI",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("IsMenuOpen", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetBool", new string[] {
                                "string",
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetInt", new string[] {
                                "string",
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFloat", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetString", new string[] {
                                "string",
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetNumber", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetBool", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetInt", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetFloat", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetString", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumber", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Invoke", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeBool", new string[] {
                                "string",
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeInt", new string[] {
                                "string",
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeFloat", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeString", new string[] {
                                "string",
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeNumber", new string[] {
                                "string",
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeBoolA", new string[] {
                                "string",
                                "string",
                                "bool[]"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeIntA", new string[] {
                                "string",
                                "string",
                                "int[]"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeFloatA", new string[] {
                                "string",
                                "string",
                                "float[]"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeStringA", new string[] {
                                "string",
                                "string",
                                "string[]"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeNumberA", new string[] {
                                "string",
                                "string",
                                "float[]"
                            }, "void"),
                new TES5InheritanceFunctionSignature("InvokeForm", new string[] {
                                "string",
                                "string",
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsTextInputEnabled", new string[] {
                            }, "bool")
            }
        },
        { "Utility",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GameTimeToString", new string[] {
                                "float"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetCurrentGameTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetCurrentRealTime", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("IsInMenuMode", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("RandomInt", new string[] {
                                "int",
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("RandomFloat", new string[] {
                                "float",
                                "float"
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetINIFloat", new string[] {
                                "string",
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetINIInt", new string[] {
                                "string",
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetINIBool", new string[] {
                                "string",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetINIString", new string[] {
                                "string",
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Wait", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("WaitGameTime", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("WaitMenuMode", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("CaptureFrameRate", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("StartFrameRateCapture", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("EndFrameRateCapture", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetAverageFrameRate", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMinFrameRate", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetMaxFrameRate", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetCurrentMemory", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetBudgetCount", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetCurrentBudget", new string[] {
                                "int"
                            }, "int"),
                new TES5InheritanceFunctionSignature("OverBudget", new string[] {
                                "int"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetBudgetName", new string[] {
                                "int"
                            }, "string"),
                new TES5InheritanceFunctionSignature("GetINIFloat", new string[] {
                                "string"
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetINIInt", new string[] {
                                "string"
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetINIBool", new string[] {
                                "string"
                            }, "bool"),
                new TES5InheritanceFunctionSignature("GetINIString", new string[] {
                                "string"
                            }, "string")
            }
        },
        { "Weapon",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Fire", new string[] {
                                "ObjectReference",
                                "Ammo"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetBaseDamage", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetBaseDamage", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCritDamage", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetCritDamage", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetReach", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetReach", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMinRange", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetMinRange", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMaxRange", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetMaxRange", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSpeed", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetSpeed", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetStagger", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetStagger", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetWeaponType", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetWeaponType", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetModelPath", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetModelPath", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetIconPath", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetIconPath", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetMessageIconPath", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetMessageIconPath", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEnchantment", new string[] {
                            }, "Enchantment"),
                new TES5InheritanceFunctionSignature("SetEnchantment", new string[] {
                                "Enchantment"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEnchantmentValue", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("SetEnchantmentValue", new string[] {
                                "int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEquippedModel", new string[] {
                            }, "Static"),
                new TES5InheritanceFunctionSignature("SetEquippedModel", new string[] {
                                "Static"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEquipType", new string[] {
                            }, "EquipSlot"),
                new TES5InheritanceFunctionSignature("SetEquipType", new string[] {
                                "EquipSlot"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkill", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetSkill", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetResist", new string[] {
                            }, "string"),
                new TES5InheritanceFunctionSignature("SetResist", new string[] {
                                "string"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCritEffect", new string[] {
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("SetCritEffect", new string[] {
                                "Spell"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCritEffectOnDeath", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("SetCritEffectOnDeath", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCritMultiplier", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("SetCritMultiplier", new string[] {
                                "float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsBattleaxe", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsBow", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsDagger", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsGreatsword", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsMace", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsStaff", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsSword", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsWarhammer", new string[] {
                            }, "bool"),
                new TES5InheritanceFunctionSignature("IsWarAxe", new string[] {
                            }, "bool")
            }
        },
        { "Weather",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ReleaseOverride", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceActive", new string[] {
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetActive", new string[] {
                                "bool",
                                "bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("FindWeather", new string[] {
                                "int"
                            }, "Weather"),
                new TES5InheritanceFunctionSignature("GetClassification", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetCurrentWeather", new string[] {
                            }, "Weather"),
                new TES5InheritanceFunctionSignature("GetOutgoingWeather", new string[] {
                            }, "Weather"),
                new TES5InheritanceFunctionSignature("GetCurrentWeatherTransition", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSkyMode", new string[] {
                            }, "int"),
                new TES5InheritanceFunctionSignature("GetSunGlare", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetSunDamage", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetWindDirection", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetWindDirectionRange", new string[] {
                            }, "float"),
                new TES5InheritanceFunctionSignature("GetFogDistance", new string[] {
                                "bool",
                                "int"
                            }, "float")
            }
        },
        { "ReferenceAlias",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Clear", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ForceRefIfEmpty", new string[] {
                                "ObjectReference"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("ForceRefTo", new string[] {
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetActorRef", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetActorReference", new string[] {
                            }, "Actor"),
                new TES5InheritanceFunctionSignature("GetRef", new string[] {
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("GetReference", new string[] {
                            }, "ObjectReference"),
                new TES5InheritanceFunctionSignature("RemoveAllInventoryEventFilters", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("RemoveInventoryEventFilter", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TryToAddToFaction", new string[] {
                                "Faction"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToClear", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToDisable", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToDisableNoWait", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToEnable", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToEnableNoWait", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToEvaluatePackage", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToKill", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToMoveTo", new string[] {
                                "ObjectReference"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToRemoveFromFaction", new string[] {
                                "Faction"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToReset", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("TryToStopCombat", new string[] {
                            }, "Bool")
            }
        },
        { "LocationAlias",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Clear", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetLocation", new string[] {
                            }, "Location"),
                new TES5InheritanceFunctionSignature("ForceLocationTo", new string[] {
                                "Location"
                            }, "void")
            }
        },
        { "Debug",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("CenterOnCell", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("CenterOnCellAndWait", new string[] {
                                "String"
                            }, "Float"),
                new TES5InheritanceFunctionSignature("PlayerMoveToAndWait", new string[] {
                                "String"
                            }, "Float"),
                new TES5InheritanceFunctionSignature("CloseUserLog", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("DumpAliasData", new string[] {
                                "Quest"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetConfigName", new string[] {
                            }, "String"),
                new TES5InheritanceFunctionSignature("GetPlatformName", new string[] {
                            }, "String"),
                new TES5InheritanceFunctionSignature("GetVersionNumber", new string[] {
                            }, "String"),
                new TES5InheritanceFunctionSignature("MessageBox", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Notification", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("OpenUserLog", new string[] {
                                "String"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("QuitGame", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFootIK", new string[] {
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetGodMode", new string[] {
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendAnimationEvent", new string[] {
                                "ObjectReference",
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartScriptProfiling", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StartStackProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopScriptProfiling", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("StopStackProfiling", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ToggleAI", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ToggleCollisions", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("ToggleMenus", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Trace", new string[] {
                                "String",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TraceAndBox", new string[] {
                                "String",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TraceConditional", new string[] {
                                "String",
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TraceStack", new string[] {
                                "String",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("TraceUser", new string[] {
                                "String",
                                "String",
                                "Int"
                            }, "Bool")
            }
        },
        { "Action",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "MagicEffect",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetAssociatedSkill", new string[] {
                            }, "String"),
                new TES5InheritanceFunctionSignature("SetAssociatedSkill", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetResistance", new string[] {
                            }, "String"),
                new TES5InheritanceFunctionSignature("SetResistance", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("IsEffectFlagSet", new string[] {
                                "Int"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("SetEffectFlag", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ClearEffectFlag", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCastTime", new string[] {
                            }, "Float"),
                new TES5InheritanceFunctionSignature("SetCastTime", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkillLevel", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetSkillLevel", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetArea", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetArea", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetSkillUsageMult", new string[] {
                            }, "Float"),
                new TES5InheritanceFunctionSignature("SetSkillUsageMult", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetBaseCost", new string[] {
                            }, "Float"),
                new TES5InheritanceFunctionSignature("SetBaseCost", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetLight", new string[] {
                            }, "Light"),
                new TES5InheritanceFunctionSignature("SetLight", new string[] {
                                "Light"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetHitShader", new string[] {
                            }, "EffectShader"),
                new TES5InheritanceFunctionSignature("SetHitShader", new string[] {
                                "EffectShader"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEnchantShader", new string[] {
                            }, "EffectShader"),
                new TES5InheritanceFunctionSignature("SetEnchantShader", new string[] {
                                "EffectShader"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetProjectile", new string[] {
                            }, "Projectile"),
                new TES5InheritanceFunctionSignature("SetProjectile", new string[] {
                                "Projectile"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetExplosion", new string[] {
                            }, "Explosion"),
                new TES5InheritanceFunctionSignature("SetExplosion", new string[] {
                                "Explosion"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetCastingArt", new string[] {
                            }, "Art"),
                new TES5InheritanceFunctionSignature("SetCastingArt", new string[] {
                                "Art"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetHitEffectArt", new string[] {
                            }, "Art"),
                new TES5InheritanceFunctionSignature("SetHitEffectArt", new string[] {
                                "Art"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEnchantArt", new string[] {
                            }, "Art"),
                new TES5InheritanceFunctionSignature("SetEnchantArt", new string[] {
                                "Art"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetImpactDataSet", new string[] {
                            }, "ImpactDataSet"),
                new TES5InheritanceFunctionSignature("SetImpactDataSet", new string[] {
                                "ImpactDataSet"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetEquipAbility", new string[] {
                            }, "Spell"),
                new TES5InheritanceFunctionSignature("SetEquipAbility", new string[] {
                                "Spell"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetImageSpaceMod", new string[] {
                            }, "ImageSpaceModifier"),
                new TES5InheritanceFunctionSignature("SetImageSpaceMod", new string[] {
                                "ImageSpaceModifier"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetPerk", new string[] {
                            }, "Perk"),
                new TES5InheritanceFunctionSignature("SetPerk", new string[] {
                                "Perk"
                            }, "void")
            }
        },
        { "Furniture",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "TalkingActivator",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Activator",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Message",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ResetHelpMessage", new string[] {
                                "String"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Show", new string[] {
                                "Float",
                                "Float",
                                "Float",
                                "Float",
                                "Float",
                                "Float",
                                "Float",
                                "Float",
                                "Float"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("ShowAsHelpMessage", new string[] {
                                "String",
                                "Float",
                                "Float",
                                "Int"
                            }, "void")
            }
        },
        { "Key",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "MiscObject",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Ammo",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "AssociationType",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "MusicType",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Remove", new string[] {
                            }, "void")
            }
        },
        { "Class",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Package",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new string[] {
                            }, "Quest"),
                new TES5InheritanceFunctionSignature("GetTemplate", new string[] {
                            }, "Package")
            }
        },
        { "Container",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Door",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "EffectShader",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Projectile",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "EncounterZone",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Scene",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("ForceStart", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetOwningQuest", new string[] {
                            }, "Quest"),
                new TES5InheritanceFunctionSignature("IsActionComplete", new string[] {
                                "Int"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsPlaying", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("Start", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Stop", new string[] {
                            }, "void")
            }
        },
        { "Explosion",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Faction",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("CanPayCrimeGold", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("GetCrimeGold", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetCrimeGoldNonViolent", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetCrimeGoldViolent", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetInfamy", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetInfamyNonViolent", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetInfamyViolent", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetReaction", new string[] {
                                "Faction"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetStolenItemValueCrime", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetStolenItemValueNoCrime", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("IsFactionInCrimeGroup", new string[] {
                                "Faction"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsPlayerExpelled", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("ModCrimeGold", new string[] {
                                "Int",
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ModReaction", new string[] {
                                "Faction",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("PlayerPayCrimeGold", new string[] {
                                "Bool",
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendAssaultAlarm", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SendPlayerToJail", new string[] {
                                "Bool",
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCrimeGold", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetCrimeGoldViolent", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetEnemy", new string[] {
                                "Faction",
                                "Bool",
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerEnemy", new string[] {
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetPlayerExpelled", new string[] {
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetReaction", new string[] {
                                "Faction",
                                "Int"
                            }, "void")
            }
        },
        { "FormList",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Find", new string[] {
                                "Form"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetAt", new string[] {
                                "Int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetSize", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("HasForm", new string[] {
                                "Form"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("RemoveAddedForm", new string[] {
                                "Form"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Revert", new string[] {
                            }, "void")
            }
        },
        { "GlobalVariable",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetValue", new string[] {
                            }, "Float"),
                new TES5InheritanceFunctionSignature("GetValueInt", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("Mod", new string[] {
                                "Float"
                            }, "Float"),
                new TES5InheritanceFunctionSignature("SetValue", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetValueInt", new string[] {
                                "Int"
                            }, "void")
            }
        },
        { "Hazard",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "SoundCategory",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Mute", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("Pause", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetFrequency", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetVolume", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnMute", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("UnPause", new string[] {
                            }, "void")
            }
        },
        { "Idle",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "ImageSpaceModifier",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("RemoveCrossFade", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Apply", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("ApplyCrossFade", new string[] {
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("PopTo", new string[] {
                                "ImageSpaceModifier",
                                "Float"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Remove", new string[] {
                            }, "void")
            }
        },
        { "Static",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "ImpactDataSet",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Topic",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Add", new string[] {
                            }, "void")
            }
        },
        { "LocationRefType",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "TopicInfo",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetOwningQuest", new string[] {
                            }, "Quest")
            }
        },
        { "LeveledActor",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new string[] {
                                "Form",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Revert", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumForms", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetNthForm", new string[] {
                                "Int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetNthLevel", new string[] {
                                "Int"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetNthLevel", new string[] {
                                "Int",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNthCount", new string[] {
                                "Int"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetNthCount", new string[] {
                                "Int",
                                "Int"
                            }, "Int")
            }
        },
        { "VisualEffect",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("Play", new string[] {
                                "ObjectReference",
                                "Float",
                                "ObjectReference"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Stop", new string[] {
                                "ObjectReference"
                            }, "void")
            }
        },
        { "LeveledItem",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new string[] {
                                "Form",
                                "Int",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("Revert", new string[] {
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetChanceNone", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetChanceNone", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetChanceGlobal", new string[] {
                            }, "GlobalVariable"),
                new TES5InheritanceFunctionSignature("SetChanceGlobal", new string[] {
                                "GlobalVariable"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumForms", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetNthForm", new string[] {
                                "Int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetNthLevel", new string[] {
                                "Int"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetNthLevel", new string[] {
                                "Int",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNthCount", new string[] {
                                "Int"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetNthCount", new string[] {
                                "Int",
                                "Int"
                            }, "Int")
            }
        },
        { "VoiceType",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "LeveledSpell",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("AddForm", new string[] {
                                "Form",
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetChanceNone", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetChanceNone", new string[] {
                                "Int"
                            }, "void"),
                new TES5InheritanceFunctionSignature("GetNumForms", new string[] {
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetNthForm", new string[] {
                                "Int"
                            }, "Form"),
                new TES5InheritanceFunctionSignature("GetNthLevel", new string[] {
                                "Int"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("SetNthLevel", new string[] {
                                "Int",
                                "Int"
                            }, "void")
            }
        },
        { "Light",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "Location",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetKeywordData", new string[] {
                                "Keyword"
                            }, "Float"),
                new TES5InheritanceFunctionSignature("GetRefTypeAliveCount", new string[] {
                                "LocationRefType"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("GetRefTypeDeadCount", new string[] {
                                "LocationRefType"
                            }, "Int"),
                new TES5InheritanceFunctionSignature("HasCommonParent", new string[] {
                                "Location",
                                "Keyword"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("HasRefType", new string[] {
                                "LocationRefType"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsCleared", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsChild", new string[] {
                                "Location"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsLoaded", new string[] {
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("IsSameLocation", new string[] {
                                "Location",
                                "Keyword"
                            }, "Bool"),
                new TES5InheritanceFunctionSignature("SetCleared", new string[] {
                                "Bool"
                            }, "void"),
                new TES5InheritanceFunctionSignature("SetKeywordData", new string[] {
                                "Keyword",
                                "Float"
                            }, "void")
            }
        },
        { "WordOfPower",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        { "WorldSpace",
            new TES5InheritanceFunctionSignature[] {
            }
        },
        //Conversion hooks,
        { "TES4TimerHelper",
            new TES5InheritanceFunctionSignature[] {
                new TES5InheritanceFunctionSignature("GetDayOfWeek", new string[] {}, "int"),

                new TES5InheritanceFunctionSignature("GetSecondsPassed", new string[] {
                        "Float"
                    }, "Float"),
                new TES5InheritanceFunctionSignature("Rotate", new string[] {
                        "ObjectReference",
                        "Float",
                        "Float",
                        "Float"
                    }, "void"),
                new TES5InheritanceFunctionSignature("LegacySay", new string[] {
                        "ObjectReference",
                        "Topic",
                        "Actor",
                        "Bool"
                    }, "Float")
            }
        },
        { "TES4Container",
            new TES5InheritanceFunctionSignature[] {

            }
        }
        };



        private static TES5InheritanceItemCollection findSubtreeFor(string className)
        {
            return findInternalSubtreeFor(className, inheritance);
        }

        private static TES5InheritanceItemCollection findInternalSubtreeFor(string className, TES5InheritanceItemCollection inputTree)
        {
            foreach (var item in inputTree)
            {
                var name = item.Name;
                var itemsWithoutSubItems = item.ItemsWithoutSubItems;
                foreach (var itemWithoutSubItems in itemsWithoutSubItems)
                {//value only
                    if (itemWithoutSubItems.Name == className)
                    {
                        return new TES5InheritanceItemCollection(); //Value matches.
                    }
                }
                if (name == className)
                {
                    return item.Items;
                }
                else
                {
                    TES5InheritanceItemCollection data = findInternalSubtreeFor(className, item.Items);
                    if (data != null)
                    {
                        return data;
                    }
                }
            }
            return null;//Not found.
        }

        private static bool treeContains(string className, TES5InheritanceItemCollection inputTree)
        {
            return findInternalSubtreeFor(className, inputTree) != null;
        }

        public static bool isExtending(ITES5Type extendingType, ITES5Type baseType)
        {
            TES5InheritanceItemCollection subTree = findSubtreeFor(baseType.value());
            if (subTree == null)
            {
                return false;
            }
            return treeContains(extendingType.value(), subTree);
        }

        public static bool IsTypeOrExtendsType(ITES5Type extendingType, ITES5Type baseType)
        {
            return extendingType == baseType || isExtending(extendingType, baseType);
        }

        public static bool IsTypeOrExtendsTypeOrIsImplicitlyComparable(ITES5Type extendingType, ITES5Type baseType)
        {
            if (IsTypeOrExtendsType(extendingType, baseType)) { return true; }
            if (IsNumberType(extendingType) && IsNumberType(baseType)) { return true; }
            return IsTypeOrExtendsType(extendingType.getNativeType(), baseType.getNativeType()) || IsTypeOrExtendsType(baseType.getNativeType(), extendingType.getNativeType());
        }

        public static bool IsNumberType(ITES5Type type)
        {
            return type == TES5BasicType.T_INT || type == TES5BasicType.T_FLOAT;
        }

        private static string targetRootBaseClass(ITES5Type type, TES5InheritanceItem baseClass, bool throwIfNotFound)
        {
            string targetClassName = type.value();
            string baseClassForNode = baseClass.Name;
            TES5InheritanceItemCollection baseClassExtenders = baseClass.Items;
            if (baseClassExtenders.Any())
            {
                foreach (var item in baseClassExtenders)
                {
                    if (item.Name == targetClassName)
                    {
                        if (baseClassForNode == null && throwIfNotFound)
                        {
                            throw new ConversionException("Type " + targetClassName + " is a top-level type in the inheritance graph, so it has no base class.");
                        }
                        return baseClassForNode;
                    }
                }

                foreach (var item in baseClassExtenders)
                {
                    string recursiveReturn = targetRootBaseClass(type, item, false);
                    if (recursiveReturn != null)
                    {
                        return recursiveReturn;
                    }
                }

                //not found in node.
                if (baseClassForNode == null || throwIfNotFound)
                {
                    throw new ConversionException("Type " + targetClassName + " not found in inheritance graph.");
                }
                return null;
            }
            else if (targetClassName == baseClassForNode)
            {
                return baseClassForNode;
            }
            if (throwIfNotFound)
            {
                throw new ConversionException("Type " + targetClassName + " not found in inheritance graph.");
            }
            return null;
        }

        public static ITES5Type GetBaseClassWithCache(ITES5Type type)
        {
            return inheritanceCache.GetOrAdd(type, () =>
            {
                string baseTypeName = targetRootBaseClass(type, inheritanceAsItem, throwIfNotFound: true);
                return TES5TypeFactory.memberByValue(baseTypeName);
            });
        }

        private static ITES5Type GetBaseClassWithoutCache(ITES5Type type)
        {
            string baseTypeName = targetRootBaseClass(type, inheritanceAsItem, throwIfNotFound: false);
            if (baseTypeName == null) { return null; }
            return TES5TypeFactory.memberByValue(baseTypeName);
        }

        public static IEnumerable<ITES5Type> GetSelfAndBaseClasses(ITES5Type type)
        {
            yield return type;
            ITES5Type baseType = type;
            while (true)
            {
                baseType = GetBaseClassWithoutCache(baseType);
                if (baseType == null) { yield break; }
                yield return baseType;
            }
        }

        public static ITES5Type findTypeByMethodParameter(ITES5Type calledOnType, string methodName, int parameterIndex)
        {
            TES5InheritanceFunctionSignature[] callReturnsOfCalledOnType;
            if (!callReturns.TryGetValue(calledOnType.value(), out callReturnsOfCalledOnType) && calledOnType.isNativePapyrusType())
            {
                throw new ConversionException("Inference type exception - no methods found for " + calledOnType.value() + "!");
            }

            foreach (var callReturn in callReturnsOfCalledOnType)
            {
                if (callReturn.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    string[] arguments = callReturn.Arguments;
                    string argument;
                    try
                    {
                        argument = arguments[parameterIndex];
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        throw new InvalidOperationException("Cannot find argument index " + parameterIndex + " in method " + methodName + " in type " + calledOnType.value(), ex);
                    }
                    return TES5TypeFactory.memberByValue(argument);
                }
            }

            ITES5Type calledOnTypeBaseClass;
            try
            {
                calledOnTypeBaseClass = GetBaseClassWithCache(calledOnType);
            }
            catch (ConversionException ex)
            {
                throw new ConversionException("Method " + methodName + " not found in type " + calledOnType.value(), ex);
            }
            return findTypeByMethodParameter(calledOnTypeBaseClass, methodName, parameterIndex);
        }

        public static ITES5Type findReturnTypeForObjectCall(ITES5Type calledOnType, string methodName)
        {
            TES5InheritanceFunctionSignature[] callReturnsOfCalledOnType;
            if (!callReturns.TryGetValue(calledOnType.value(), out callReturnsOfCalledOnType))
            {
                //Type not present in inheritance graph, check if its a basic type ( which means its basically an exception )
                if (calledOnType.isNativePapyrusType())
                {
                    throw new ConversionException("Inference type exception - no call returns for " + calledOnType.value() + "!");
                }
                else
                {
                    //Otherwise, treat it like a base script
                    calledOnType = calledOnType.getNativeType();
                    callReturnsOfCalledOnType = callReturns[calledOnType.value()];
                }
            }

            foreach (var method in callReturnsOfCalledOnType)
            {
                if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    return TES5TypeFactory.memberByValue(method.ReturnType);
                }
            }

            return findReturnTypeForObjectCall(GetBaseClassWithCache(calledOnType), methodName);
        }
        
        public static ITES5Type findTypeByMethod(TES5ObjectCall objectCall)
        {
            string methodName = objectCall.FunctionName;
            List<ITES5Type> possibleMatches = new List<ITES5Type>();
            foreach (var callReturn in callReturns)
            {
                string type = callReturn.Key;
                var methods = callReturn.Value;
                foreach (var method in methods)
                {
                    if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                    {
                        possibleMatches.Add(TES5TypeFactory.memberByValue(type));
                    }
                }
            }

            ITES5Variable calledOn = objectCall.AccessedObject.ReferencesTo;
            List<ITES5Type> extendingMatches = new List<ITES5Type>();
            ITES5Type actualType = calledOn.getPropertyType().getNativeType();
            foreach (ITES5Type possibleMatch in possibleMatches)
            {
                if (possibleMatch == actualType)
                {
                    return possibleMatch; //if the possible match matches the actual basic type, it means that it surely IS one of those.
                }

                //Ok, so are those matches somehow connected at all?
                if (isExtending(possibleMatch, actualType) || isExtending(actualType, possibleMatch))
                {
                    extendingMatches.Add(possibleMatch);
                }
            }

            switch (extendingMatches.Count)
            {
                case 0:
                    {
                        List<string> concatTypes = new List<string>();
                        foreach (var possibleMatch in possibleMatches)
                        {
                            concatTypes.Add(possibleMatch.value());
                        }

                        throw new ConversionException("Cannot find any possible type for method " + methodName + ", trying to extend " + actualType.value() + " with following types: " + string.Join(", ", concatTypes));
                    }

                case 1:
                    {
                        return extendingMatches[0];
                    }

                default:
                    {
                        //We analyze the property name and check inside the ESM analyzer.
                        ESMAnalyzer analyzer = ESMAnalyzer._instance();
                        ITES5Type formType = analyzer.getFormTypeByEDID(calledOn.getReferenceEdid());
                        //WTM:  Change:  I added matching on the type and on its base classes this so that functions like SetFactionOwner will work.
                        //SetFactionOwner gives two extendingMatches:  Cell and ObjectReference (since both classes have the SetFactionOwner function).
                        //But the EDID "WeynonHorsePlayer" results in a type of Actor.  In this case, the base class of Actor (ObjectReference)
                        //must be retrieved so a match can be found.
                        IEnumerable<ITES5Type> formTypeAndBaseTypes = GetSelfAndBaseClasses(formType);
                        if (!formTypeAndBaseTypes.Any(bt => extendingMatches.Contains(bt)))
                        {
                            throw new ConversionException("ESM <-> Inheritance Graph conflict.  ESM returned " + formType.value() + ", which is not present in possible matches from inheritance graph:  " + string.Join(", ", extendingMatches.Select(em => em.value())));
                        }
                        return formType;
                    }
            }
        }
    }
}
