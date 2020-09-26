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
        public static void FillFunctions(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, MetadataLogService metadataLogService, ESMAnalyzer esmAnalyzer)
        {
            DefaultFunctionFactory defaultFunctionFactory = new DefaultFunctionFactory(objectCallFactory, objectCallArgumentsFactory);
            LogUnknownFunctionFactory logUnknownFunctionFactory = new LogUnknownFunctionFactory(objectCallFactory);
            FillerFactory fillerFactory = new FillerFactory();
            valueFactory.AddFunctionFactory("activate", new ActivateFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("addachievement", fillerFactory);
            valueFactory.AddFunctionFactory("additem", new AddOrRemoveItemFactory(valueFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("addscriptpackage", new AddScriptPackageFactory(referenceFactory, metadataLogService, objectCallFactory));
            valueFactory.AddFunctionFactory("addspell", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("addtopic", new AddTopicFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("autosave", new AutosaveFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("cast", new CastFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("clearownership", new ClearOwnershipFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("closecurrentobliviongate", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("closeobliviongate", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("completequest", new PopCalledRenameFunctionFactory("CompleteQuest", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("createfullactorcopy", new CreateFullActorCopyFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("deletefullactorcopy", new RenamedFunctionFactory("Delete", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("disablelinkedpathpoints", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("disableplayercontrols", new DisablePlayerControlsFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("disable", new DisableFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("dispel", new RenamedFunctionFactory("DispelSpell", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("dropme", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("drop", new RenamedFunctionFactory("DropObject", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("duplicateallitems", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("enablefasttravel", new EnableFastTravelFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("enablelinkedpathpoints", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("enableplayercontrols", new EnablePlayerControlsFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("enable", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("equipitem", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("essentialdeathreload", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("evaluatepackage", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("forceactorvalue", new ForceActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("forcecloseobliviongate", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("forceflee", /*defaultFunctionFactory*/new NotSupportedFactory());
            valueFactory.AddFunctionFactory("forceweather", new ForceWeatherFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getactionref", new GetActionRefFactory());
            valueFactory.AddFunctionFactory("getactorvalue", new GetActorValueFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getamountsoldstolen", new GetAmountSoldStolenFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getangle", new GetAngleFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getarmorrating", new GetArmorRatingFactory(objectCallFactory));//WTM:  Change:  Added (SKSE)
            valueFactory.AddFunctionFactory("getattacked", new RenamedFunctionFactory("IsInCombat", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getbaseactorvalue", new GetBaseActorValueFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getbuttonpressed", new GetButtonPressedFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getclothingvalue", new GetClothingValueFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getcombattarget", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getcontainer", new GetContainerFactory());
            valueFactory.AddFunctionFactory("getcrimegold", new GetCrimeGoldFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcrimeknown", new GetCrimeKnownFactory());
            valueFactory.AddFunctionFactory("getcrime", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getcurrentaipackage", new RenamedFunctionFactory("GetCurrentPackage", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getcurrentaiprocedure", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getcurrenttime", new GetCurrentTimeFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getdayofweek", new GetDayOfWeekFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdeadcount", new PopCalledRenameActorBaseFunctionFactory("GetDeadCount", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdead", new RenamedFunctionFactory("IsDead", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdestroyed", new RenamedFunctionFactory("LegacyGetDestroyed", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdetected", new GetDetectedFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getdisabled", new RenamedFunctionFactory("IsDisabled", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getdisposition", new GetDispositionFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getdistance", new GetDistanceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getequipped", new RenamedFunctionFactory("IsEquipped", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getfactionrank", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getforcesneak", new GetForceSneakFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("getgamesetting", new GetGameSettingFactory());
            valueFactory.AddFunctionFactory("getgold", new RenamedFunctionFactory("GetGoldAmount", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getheadingangle", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getincell", new GetInCellFactory(objectCallFactory, esmAnalyzer));
            valueFactory.AddFunctionFactory("getinfaction", new RenamedFunctionFactory("IsInFaction", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getinsamecell", new GetInSameCellFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getinworldspace", new GetInWorldspaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisalerted", new RenamedFunctionFactory("IsAlerted", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getiscurrentpackage", new GetIsCurrentPackageFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getiscurrentweather", new GetIsCurrentWeatherFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisid", new GetIsIdFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisplayablerace", new GetIsPlayableRaceFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getisplayerbirthsign", new NotSupportedFactory());
            valueFactory.AddFunctionFactory("getisrace", new GetIsRaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getisreference", new GetIsReferenceFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getissex", new GetIsSexFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getitemcount", new GetItemCountFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getknockedstate", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("getlevel", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getlocked", new RenamedFunctionFactory("IsLocked", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getlos", new RenamedFunctionFactory("HasLOS", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getopenstate", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getparentref", new RenamedFunctionFactory("GetEnableParent", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getpcexpelled", new PopCalledRenameFunctionFactory("IsPlayerExpelled", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getpcfactionmurder", new GetPCFactionMurderFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getpcfactionattack", new GetPCFactionAttackFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcfactionsteal", new GetPCFactionStealFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcfame", new GetPCFameFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getpcinfamy", new GetPCInfamyFactory(referenceFactory));
            valueFactory.AddFunctionFactory("getpcisrace", new GetPCIsRaceFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpcissex", new GetPCIsSexFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getpcmiscstat", new GetPCMiscStatFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getplayercontrolsdisabled", new GetPlayerControlsDisabledFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getplayerinseworld", new GetPlayerInSEWorldFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getpos", new GetPosFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getquestrunning", new PopCalledRenameFunctionFactory("IsRunning", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getrandompercent", new GetRandomPercentFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getrestrained", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("getsecondspassed", new GetSecondsPassedFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("getself", new GetSelfFactory());
            valueFactory.AddFunctionFactory("getshouldattack", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("getsleeping", new RenamedFunctionFactory("GetSleepState", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstagedone", new PopCalledRenameFunctionFactory("GetStageDone", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstage", new PopCalledRenameFunctionFactory("GetStage", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("getstartingangle", /*defaultFunctionFactory*/new NotSupportedFactory());
            valueFactory.AddFunctionFactory("getstartingpos", /*defaultFunctionFactory*/new NotSupportedFactory());
            valueFactory.AddFunctionFactory("gettalkedtopc", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("getweaponanimtype", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("gotojail", new GoToJailFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("hasmagiceffect", new HasMagicEffectFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("hasvampirefed", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("isactionref", new IsActionRefFactory(referenceFactory));
            valueFactory.AddFunctionFactory("isactorusingatorch", new RenamedFunctionFactory("IsTorchOut", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isactor", new IsActorFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("isanimplaying", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isessential", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isguard", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isidleplaying", new ReturnTrueFactory());
            valueFactory.AddFunctionFactory("isincombat", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isindangerouswater", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("isininterior", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isowner", new IsOwnerFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("ispcamurderer", new IsPCAMurdererFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("ispcsleeping", new IsPCSleepingFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("isplayerinjail", new IsPlayerInJailFactory(referenceFactory, objectPropertyFactory));
            valueFactory.AddFunctionFactory("israining", new IsRainingFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("isridinghorse", new RenamedFunctionFactory("IsOnMount", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("issneaking", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("isspelltarget", new IsSpellTargetFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("isswimming", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("istalking", new RenamedFunctionFactory("IsInDialogueWithPlayer", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("istimepassing", new ReturnTrueFactory());//WTM:  Note:  This is said to always return true:  https://www.creationkit.com/index.php?title=IsTimePassing
            valueFactory.AddFunctionFactory("isweaponout", new RenamedFunctionFactory("IsWeaponDrawn", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("isxbox", new ReturnFalseFactory());
            valueFactory.AddFunctionFactory("kill", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("lock", new LockFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("look", new RenamedFunctionFactory("SetLookAt", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("menumode", new MenuModeFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("messagebox", new MessageBoxFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory, metadataLogService));
            valueFactory.AddFunctionFactory("message", new MessageFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modactorvalue", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modamountsoldstolen", new ModAmountSoldStolenFactory(objectCallFactory, objectCallArgumentsFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("modcrimegold", new ModCrimeGoldFactory(referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("moddisposition", new ModDispositionFactory(objectCallFactory, objectCallArgumentsFactory, logUnknownFunctionFactory));
            valueFactory.AddFunctionFactory("modfactionreaction", new PopCalledRenameFunctionFactory("ModReaction", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("modpcfame", new ModPCFameFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcinfamy", new ModPCInfamyFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcmiscstat", new ModPCMiscStatFactory(objectCallFactory, logUnknownFunctionFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("movetomarker", new RenamedFunctionFactory("MoveTo", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("moveto", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("payfine", new PayFineFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("payfinethief", new PayFineThiefFactory(objectCallFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("pickidle", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("placeatme", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("playbink", new PlayBinkFactory(objectCallFactory, objectCallArgumentsFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("playgroup", new PlayGroupFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("playmagiceffectvisuals", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("playmagicshadervisuals", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("playsound3d", new PlaySound3DFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("playsound", new PlaySoundFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("purgecellbuffers", fillerFactory);
            valueFactory.AddFunctionFactory("pushactoraway", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("refreshtopiclist", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("releaseweatheroverride", new ReleaseWeatherOverrideFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("removeallitems", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("removeitem", new AddOrRemoveItemFactory(valueFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("removeme", new RemoveMeFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("removescriptpackage", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("removespell", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("reset3dstate", new RenamedFunctionFactory("Reset", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("resetfalldamagetimer", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("resethealth", new ResetHealthFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("resetinterior", new PopCalledRenameFunctionFactory("Reset", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("resurrect", new ResurrectFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("rotate", new RotateFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("sayto", new SayToFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("say", new SayFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("sendtrespassalarm", new SendTrespassAlarmFactory(objectCallFactory, objectCallArgumentsFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setactoralpha", new SetActorAlphaFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setactorfullname", new SetActorFullNameFactory(objectCallFactory, objectCallArgumentsFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setactorrefraction", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setactorsai", new RenamedFunctionFactory("EnableAI", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setactorvalue", new SetActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setalert", new SetAlertFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setallreachable", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setallvisible", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setangle", new SetAngleFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setcellfullname", new SetCellFullNameFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setcellpublicflag", new SetCellPublicFlagFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setcellownership", new SetCellOwnershipFactory(objectCallFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setclass", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setcombatstyle", new SetCombatStyleFactory(objectCallFactory, objectCallArgumentsFactory, logUnknownFunctionFactory));
            valueFactory.AddFunctionFactory("setcrimegold", new SetCrimeGoldFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setdestroyed", new RenamedFunctionFactory("BlockActivation", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setdoordefaultopen", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setessential", new PopCalledRenameActorBaseFunctionFactory("SetEssential", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setfactionrank", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("setfactionreaction", new PopCalledRenameFunctionFactory("SetReaction", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setforcerun", new SetForceRunFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setforcesneak", new SetForceSneakFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setghost", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("setignorefriendlyhits", new SetIgnoreFriendlyHitsFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setinchargen", new SetInChargenFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setinvestmentgold", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setitemvalue", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setlevel", new SetLevelFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setnoavoidance", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setnorumors", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setopenstate", new SetOpenStateFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setownership", new SetOwnershipFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setpackduration", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setpcexpelled", new PopCalledRenameFunctionFactory("SetPlayerExpelled", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setpcfactionattack", new SetPCFactionAttackFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfactionmurder", new SetPCFactionMurderFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfactionsteal", new SetPCFactionStealFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcfame", new SetPCFameFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setpcinfamy", new SetPCInfamyFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("setplayerinseworld", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setpos", new SetPosFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setpublic", new SetCellPublicFlagFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("setquestobject", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setrestrained", new SetRestrainedFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("setrigidbodymass", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setscale", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("setsceneiscomplex", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setshowquestitems", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("setstage", new PopCalledRenameFunctionFactory("SetStage", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("setunconscious", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("setweather", new SetWeatherFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("showbirthsignmenu", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("showclassmenu", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("showdialogsubtitles", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("showenchantment", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("showmap", new ShowMapFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("showracemenu", new ShowRaceMenuFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("showspellmaking", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("startcombat", new StartCombatFactory(objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("startconversation", new StartConversationFactory(logUnknownFunctionFactory));
            valueFactory.AddFunctionFactory("startquest", new PopCalledRenameFunctionFactory("Start", referenceFactory, objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("stopcombatalarmonactor", new RenamedFunctionFactory("StopCombatAlarm", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("stopcombat", new StopCombatFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("stoplook", new StopLookFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("stopmagiceffectvisuals", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("stopmagicshadervisuals", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("stopwaiting", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("stopquest", new StopQuestFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
            valueFactory.AddFunctionFactory("trapupdate", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("triggerhitshader", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("unequipitem", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("unlock", new UnlockFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("wait", logUnknownFunctionFactory);
            valueFactory.AddFunctionFactory("wakeuppc", new WakeUpPCFactory(objectCallFactory));//WTM:  Change:  Added
            valueFactory.AddFunctionFactory("yield", defaultFunctionFactory);
            valueFactory.AddFunctionFactory("evp", new RenamedFunctionFactory("EvaluatePackage", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("forceav", new ForceActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("fw", new ForceWeatherFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getav", new GetActorValueFactory(referenceFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("getbaseav", new GetBaseActorValueFactory(objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("getgs", new GetGameSettingFactory());
            valueFactory.AddFunctionFactory("getiscreature", new GetIsCreatureFactory(objectCallFactory));//WTM:  Change:  This previously used IsActorFactory.
            valueFactory.AddFunctionFactory("this", new GetSelfFactory());
            valueFactory.AddFunctionFactory("isactordetected", new RenamedFunctionFactory("IsInCombat", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("modav", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("modpcskill", new ModActorValueFactory(referenceFactory, valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("pme", logUnknownFunctionFactory);//WTM:  Note:  PlayMagicEffectVisuals
            valueFactory.AddFunctionFactory("pms", logUnknownFunctionFactory);//WTM:  Note:  PlayMagicShaderVisuals
            valueFactory.AddFunctionFactory("pcb", fillerFactory);//WTM:  Note:  PurgeCellBuffers
            valueFactory.AddFunctionFactory("scripteffectelapsedseconds", new GetSecondsPassedFactory(objectCallFactory));
            valueFactory.AddFunctionFactory("saa", new SetActorAlphaFactory(valueFactory, objectCallFactory));
            valueFactory.AddFunctionFactory("setav", new SetActorValueFactory(valueFactory, objectCallFactory, referenceFactory));
            valueFactory.AddFunctionFactory("sme", logUnknownFunctionFactory);//WTM:  Note:  StopMagicEffectVisuals
            valueFactory.AddFunctionFactory("sms", logUnknownFunctionFactory);//WTM:  Note:  StopMagicShaderVisuals
            valueFactory.AddFunctionFactory("scaonactor", new RenamedFunctionFactory("StopCombatAlarm", objectCallFactory, objectCallArgumentsFactory));
            valueFactory.AddFunctionFactory("sw", new SetWeatherFactory(objectCallFactory, objectCallArgumentsFactory, referenceFactory));
        }
    }
}