using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.DI
{
    static class TES5ValueFactoryFunctionFiller
    {
        /*
        * Injects function handlers to value factory, as only two-way link in whole project
        */
        public static void FillFunctions(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer esmAnalyzer, MetadataLogService metadataLogService, TES5StaticReferenceFactory staticReferenceFactory)
        {
            valueFactory.AddFunctionFactory("activate", new ActivateFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("addachievement", new FillerFactory());
            valueFactory.AddFunctionFactory("additem", new AddOrRemoveItemFactory(valueFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("addscriptpackage", new AddScriptPackageFactory(referenceFactory, metadataLogService, objectCallFactory));
            valueFactory.AddFunctionFactory("addspell", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("addtopic", new PopCalledRenameFunctionFactory("Add", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("autosave", new AutosaveFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("cast", new CastFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("clearownership", new ClearOwnershipFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("closecurrentobliviongate", new FillerFactory());
            valueFactory.AddFunctionFactory("closeobliviongate", new FillerFactory());
            valueFactory.AddFunctionFactory("completequest", new PopCalledRenameFunctionFactory("CompleteQuest", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("createfullactorcopy", new CreateFullActorCopyFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("deletefullactorcopy", new RenamedFunctionFactory("Delete", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("disablelinkedpathpoints", new FillerFactory());
            valueFactory.AddFunctionFactory("disableplayercontrols", new DisablePlayerControlsFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("disable", new DisableFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("dispel", new RenamedFunctionFactory("DispelSpell", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("dropme", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("drop", new RenamedFunctionFactory("DropObject", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("duplicateallitems", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("enablefasttravel", new EnableFastTravelFactory(objectCallFactory, objectCallArgumentsFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("enablelinkedpathpoints", new FillerFactory());
            valueFactory.AddFunctionFactory("enableplayercontrols", new EnablePlayerControlsFactory(objectCallFactory, objectCallArgumentsFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("enable", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("equipitem", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("essentialdeathreload", new FillerFactory());
            valueFactory.AddFunctionFactory("evaluatepackage", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("forceactorvalue", new ForceActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("forcecloseobliviongate", new FillerFactory());
            valueFactory.AddFunctionFactory("forceflee", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("forceweather", new ForceWeatherFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getactionref", new GetActionRefFactory());
            valueFactory.AddFunctionFactory("getactorvalue", new GetActorValueFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getamountsoldstolen", new GetAmountSoldStolenFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("getangle", new GetAngleFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getarmorrating", new GetArmorRatingFactory());
            valueFactory.AddFunctionFactory("getattacked", new RenamedFunctionFactory("IsInCombat", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getbaseactorvalue", new GetBaseActorValueFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getbuttonpressed", new GetButtonPressedFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getclothingvalue", new GetClothingValueFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getcombattarget", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcontainer", new GetContainerFactory());
            valueFactory.AddFunctionFactory("getcrimegold", new GetCrimeGoldFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcrimeknown", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("getcrime", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcurrentaipackage", new RenamedFunctionFactory("GetCurrentPackage", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcurrentaiprocedure", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcurrenttime", new GetCurrentTimeFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getdayofweek", new GetDayOfWeekFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdeadcount", new PopCalledRenameFunctionFactory("GetDeadCount", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdead", new RenamedFunctionFactory("IsDead", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdestroyed", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdetected", new GetDetectedFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getdisabled", new RenamedFunctionFactory("IsDisabled", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdisposition", new GetDispositionFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getdistance", new GetDistanceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getequipped", new RenamedFunctionFactory("IsEquipped", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getfactionrank", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getforcesneak", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("getgamesetting", new GetGameSettingFactory());
            valueFactory.AddFunctionFactory("getgold", new RenamedFunctionFactory("GetGoldAmount", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getheadingangle", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getincell", new GetInCellFactory(objectCallFactory, esmAnalyzer, staticReferenceFactory));
            valueFactory.AddFunctionFactory("getinfaction", new RenamedFunctionFactory("IsInFaction", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getinsamecell", new GetInSameCellFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getinworldspace", new GetInWorldspaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisalerted", new RenamedFunctionFactory("IsAlerted", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getiscurrentpackage", new GetIsCurrentPackageFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getiscurrentweather", new GetIsCurrentWeatherFactory(referenceFactory, objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("getisid", new GetIsIdFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisplayablerace", new GetIsPlayableRaceFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getisplayerbirthsign", new NotSupportedFactory());
            valueFactory.AddFunctionFactory("getisrace", new GetIsRaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisreference", new GetIsReferenceFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getissex", new GetIsSexFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getitemcount", new GetItemCountFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getknockedstate", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("getlevel", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getlocked", new RenamedFunctionFactory("IsLocked", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getlos", new RenamedFunctionFactory("HasLOS", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getopenstate", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getparentref", new RenamedFunctionFactory("GetEnableParent", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getpcexpelled", new PopCalledRenameFunctionFactory("IsPlayerExpelled", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getpcfactionmurder", new GetPCFactionMurderFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getpcfactionattack", new GetPCFactionAttackFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcfactionsteal", new GetPCFactionStealFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcfame", new GetPCFameFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getpcinfamy", new GetPCInfamyFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getpcisrace", new GetPCIsRaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcissex", new GetPCIsSexFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getpcmiscstat", new GetPCMiscStatFactory(objectCallFactory, objectCallArgumentsFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("getplayercontrolsdisabled", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("getplayerinseworld", new GetPlayerInSEWorldFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpos", new GetPosFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getquestrunning", new PopCalledRenameFunctionFactory("IsRunning", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getrandompercent", new GetRandomPercentFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("getrestrained", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getsecondspassed", new GetSecondsPassedFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getself", new GetSelfFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getshouldattack", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("getsleeping", new RenamedFunctionFactory("GetSleepState", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstagedone", new PopCalledRenameFunctionFactory("GetStageDone", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstage", new PopCalledRenameFunctionFactory("GetStage", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstartingangle", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstartingpos", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("gettalkedtopc", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("getweaponanimtype", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("gotojail", new GoToJailFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("hasmagiceffect", new HasMagicEffectFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("hasvampirefed", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("isactionref", new IsActionRefFactory(referenceFactory));
            valueFactory.AddFunctionFactory("isactorusingatorch", new RenamedFunctionFactory("IsTorchOut", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isactor", new IsActorFactory());
            valueFactory.AddFunctionFactory("isanimplaying", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isessential", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isguard", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isidleplaying", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("isincombat", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isindangerouswater", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("isininterior", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isowner", new IsOwnerFactory(objectCallFactory, referenceFactory, esmAnalyzer));
            valueFactory.AddFunctionFactory("ispcamurderer", new IsPCAMurdererFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("ispcsleeping", new IsPCSleepingFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("isplayerinjail", new IsPlayerInJailFactory(referenceFactory, objectPropertyFactory));
            valueFactory.AddFunctionFactory("israining", new IsRainingFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("isridinghorse", new RenamedFunctionFactory("IsOnMount", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("issneaking", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isspelltarget", new IsSpellTargetFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("isswimming", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("istalking", new RenamedFunctionFactory("IsInDialogueWithPlayer", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("istimepassing", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("isweaponout", new RenamedFunctionFactory("IsWeaponDrawn", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isxbox", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("kill", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("lock", new LockFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("look", new RenamedFunctionFactory("SetLookAt", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("menumode", new MenuModeFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("messagebox", new MessageBoxFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory, metadataLogService, staticReferenceFactory));
            valueFactory.AddFunctionFactory("message", new MessageFactory(valueFactory, objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("modactorvalue", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modamountsoldstolen", new ModAmountSoldStolenFactory(objectCallFactory, objectCallArgumentsFactory, staticReferenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("modcrimegold", new ModCrimeGoldFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("moddisposition", new ModDispositionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("modfactionreaction", new PopCalledRenameFunctionFactory("ModReaction", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("modpcfame", new ModPCFameFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcinfamy", new ModPCInfamyFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcmiscstat", new FillerFactory());
            valueFactory.AddFunctionFactory("movetomarker", new RenamedFunctionFactory("MoveTo", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("moveto", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("payfine", new PayFineFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("payfinethief", new PayFineThiefFactory(objectCallFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("pickidle", new FillerFactory());
            valueFactory.AddFunctionFactory("placeatme", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("playbink", new PlayBinkFactory(objectCallFactory, objectCallArgumentsFactory, staticReferenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("playgroup", new PlayGroupFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("playmagiceffectvisuals", new FillerFactory());
            valueFactory.AddFunctionFactory("playmagicshadervisuals", new FillerFactory());
            valueFactory.AddFunctionFactory("playsound3d", new PlaySound3DFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("playsound", new PlaySoundFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("purgecellbuffers", new FillerFactory());
            valueFactory.AddFunctionFactory("pushactoraway", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("refreshtopiclist", new FillerFactory());
            valueFactory.AddFunctionFactory("releaseweatheroverride", new ReleaseWeatherOverrideFactory(objectCallFactory, staticReferenceFactory));
            valueFactory.AddFunctionFactory("removeallitems", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("removeitem", new AddOrRemoveItemFactory(valueFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("removeme", new RemoveMeFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("removescriptpackage", new FillerFactory());
            valueFactory.AddFunctionFactory("removespell", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("reset3dstate", new RenamedFunctionFactory("Reset", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("resetfalldamagetimer", new FillerFactory());
            valueFactory.AddFunctionFactory("resethealth", new ResetHealthFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("resetinterior", new PopCalledRenameFunctionFactory("Reset", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("resurrect", new ResurrectFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("rotate", new RotateFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("sayto", new SayToFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("say", new SayFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("sendtrespassalarm", new FillerFactory());
            valueFactory.AddFunctionFactory("setactoralpha", new SetActorAlphaFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setactorfullname", new FillerFactory());
            valueFactory.AddFunctionFactory("setactorrefraction", new FillerFactory());
            valueFactory.AddFunctionFactory("setactorsai", new RenamedFunctionFactory("EnableAI", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setactorvalue", new SetActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setalert", new SetAlertFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setallreachable", new FillerFactory());
            valueFactory.AddFunctionFactory("setallvisible", new FillerFactory());
            valueFactory.AddFunctionFactory("setangle", new SetAngleFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setcellfullname", new SetCellFullNameFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setcellpublicflag", new SetCellPublicFlagFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setcellownership", new SetCellOwnershipFactory(objectCallFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setclass", new FillerFactory());
            valueFactory.AddFunctionFactory("setcombatstyle", new FillerFactory());
            valueFactory.AddFunctionFactory("setcrimegold", new SetCrimeGoldFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setdestroyed", new RenamedFunctionFactory("BlockActivation", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setdoordefaultopen", new FillerFactory());
            valueFactory.AddFunctionFactory("setessential", new PopCalledRenameFunctionFactory("SetEssential", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setfactionrank", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setfactionreaction", new PopCalledRenameFunctionFactory("SetReaction", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setforcerun", new SetForceRunFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setforcesneak", new SetForceSneakFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setghost", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setignorefriendlyhits", new FillerFactory());
            valueFactory.AddFunctionFactory("setinchargen", new SetInChargenFactory(objectCallFactory, staticReferenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setinvestmentgold", new FillerFactory());
            valueFactory.AddFunctionFactory("setitemvalue", new FillerFactory());
            valueFactory.AddFunctionFactory("setlevel", new SetLevelFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setnoavoidance", new FillerFactory());
            valueFactory.AddFunctionFactory("setnorumors", new FillerFactory());
            valueFactory.AddFunctionFactory("setopenstate", new SetOpenStateFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setownership", new SetOwnershipFactory(objectCallFactory, objectCallArgumentsFactory, esmAnalyzer));
            valueFactory.AddFunctionFactory("setpackduration", new FillerFactory());
            valueFactory.AddFunctionFactory("setpcexpelled", new PopCalledRenameFunctionFactory("SetPlayerExpelled", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setpcfactionattack", new SetPCFactionAttackFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfactionmurder", new SetPCFactionMurderFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfactionsteal", new SetPCFactionStealFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfame", new SetPCFameFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcinfamy", new SetPCInfamyFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setplayerinseworld", new FillerFactory());
            valueFactory.AddFunctionFactory("setpos", new SetPosFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setpublic", new SetCellPublicFlagFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setquestobject", new FillerFactory());
            valueFactory.AddFunctionFactory("setrestrained", new FillerFactory());
            valueFactory.AddFunctionFactory("setrigidbodymass", new FillerFactory());
            valueFactory.AddFunctionFactory("setscale", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setsceneiscomplex", new FillerFactory());
            valueFactory.AddFunctionFactory("setshowquestitems", new FillerFactory());
            valueFactory.AddFunctionFactory("setstage", new PopCalledRenameFunctionFactory("SetStage", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setunconscious", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setweather", new SetWeatherFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("showbirthsignmenu", new FillerFactory());
            valueFactory.AddFunctionFactory("showclassmenu", new FillerFactory());
            valueFactory.AddFunctionFactory("showdialogsubtitles", new FillerFactory());
            valueFactory.AddFunctionFactory("showenchantment", new FillerFactory());
            valueFactory.AddFunctionFactory("showmap", new ShowMapFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("showracemenu", new FillerFactory());
            valueFactory.AddFunctionFactory("showspellmaking", new FillerFactory());
            valueFactory.AddFunctionFactory("startcombat", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("startconversation", new RenamedFunctionFactory("OBStartConversation", objectCallFactory, objectCallArgumentsFactory)/*new StartConversationFactory(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService)*/);//WTM:  Change
            valueFactory.AddFunctionFactory("startquest", new PopCalledRenameFunctionFactory("Start", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("stopcombatalarmonactor", new RenamedFunctionFactory("StopCombatAlarm", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("stopcombat", new StopCombatFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("stoplook", new StopLookFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("stopmagiceffectvisuals", new FillerFactory());
            valueFactory.AddFunctionFactory("stopmagicshadervisuals", new FillerFactory());
            valueFactory.AddFunctionFactory("stopwaiting", new FillerFactory());
            valueFactory.AddFunctionFactory("stopquest", new StopQuestFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("trapupdate", new FillerFactory());
            valueFactory.AddFunctionFactory("triggerhitshader", new FillerFactory());
            valueFactory.AddFunctionFactory("unequipitem", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("unlock", new UnlockFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("wait", new FillerFactory());
            valueFactory.AddFunctionFactory("wakeuppc", new NotSupportedFactory());
            valueFactory.AddFunctionFactory("yield", new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("evp", new RenamedFunctionFactory("EvaluatePackage", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("forceav", new ForceActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("fw", new ForceWeatherFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getav", new GetActorValueFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getbaseav", new GetBaseActorValueFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getgs", new GetGameSettingFactory());
            valueFactory.AddFunctionFactory("getiscreature", new GetIsCreatureFactory(objectCallFactory));//WTM:  Change:  This previously used IsActorFactory.
            valueFactory.AddFunctionFactory("this", new GetSelfFactory(referenceFactory));
            valueFactory.AddFunctionFactory("isactordetected", new RenamedFunctionFactory("IsInCombat", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("modav", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcskill", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("pme", new FillerFactory());
            valueFactory.AddFunctionFactory("pms", new FillerFactory());
            valueFactory.AddFunctionFactory("pcb", new FillerFactory());
            valueFactory.AddFunctionFactory("scripteffectelapsedseconds", new GetSecondsPassedFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("saa", new SetActorAlphaFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setav", new SetActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("sme", new FillerFactory());
            valueFactory.AddFunctionFactory("scaonactor", new RenamedFunctionFactory("StopCombatAlarm", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("sms", new FillerFactory());
            valueFactory.AddFunctionFactory("sw", new SetWeatherFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
        }
    }
}