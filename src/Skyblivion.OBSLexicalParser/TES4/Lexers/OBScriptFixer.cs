using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    public static class OBScriptFixer
    {
        private static string FixSyntaxErrors(string str)
        {
            if (str.StartsWith("scn CGateOPEN01SCRIPT"))
            {
                str = str.Replace("	else isActionRef mySelf == 0 && busy == 0", "	elseif isActionRef mySelf == 0 && busy == 0");
            }
            else if (str.StartsWith("scn DAHermaeusSoulsSpell"))
            {
                str = str.Replace(
@"	set badtarget to 1
end",
@"	set badtarget to 1
endif
end");
            }
            else if (str.StartsWith("Scriptname Dark02DoorScript") || str.StartsWith("Scriptname Dark10SpecialJournalScript") || str.StartsWith("Scriptname Dark17DiaryScript") || str.StartsWith("Scriptname DarkLachanceNoteScript"))
            {
                str = str.Replace(@"endif

else", "else");
            }
            else if (str.StartsWith("Scriptname Dark05MotierreScript"))
            {
                str = str.Replace("if Dark05Assassinated.HidesHeartSound >= 02", "if Dark05Assassinated.HidesHeartSound >= 2").Replace("if Dark05Assassinated.HidesHeartSound < 02", "if Dark05Assassinated.HidesHeartSound < 2");
            }
            else if (str.StartsWith("scriptname Dark08WhodunitScript"))
            {
                str = str.Replace(
@"begin gamemode
 
short guestsdead
short MatildeBasement
short monsterskilled
short enabled
short MatildeAlly
short MatildeDead
short NevilleDead
short NelsDead
short PrimoDead
short DovesiDead
short AttackDetect
short FafnirGreet
short NelsStupor
short NevilleGear
short NevilleHide
short DovesiRest
short DovesiBasement
short NevilleDoor
short JigsUp
short DovesiTrick
short DovesiSad
short DovesiFlirt
short DovesiShadow
short NoGuards
short NelsConvinced
short DovesiConvinced
short NevilleConvinced
short PrimoConvinced
short Stagger
short HateDovesi
short PrimoHateDovesi
 
Float fQuestDelayTime",
@"short guestsdead
short MatildeBasement
short monsterskilled
short enabled
short MatildeAlly
short MatildeDead
short NevilleDead
short NelsDead
short PrimoDead
short DovesiDead
short AttackDetect
short FafnirGreet
short NelsStupor
short NevilleGear
short NevilleHide
short DovesiRest
short DovesiBasement
short NevilleDoor
short JigsUp
short DovesiTrick
short DovesiSad
short DovesiFlirt
short DovesiShadow
short NoGuards
short NelsConvinced
short DovesiConvinced
short NevilleConvinced
short PrimoConvinced
short Stagger
short HateDovesi
short PrimoHateDovesi
 
Float fQuestDelayTime

begin gamemode");
            }
            else if (str.StartsWith("Scriptname EyeOfNocturnalScript"))
            {
                str = str.Replace("End	GameMode", "End");
            }
            else if (str.StartsWith("ScriptName FGC06BragScript"))
            {
                str = str.Replace(
@"Begin GameMode

short BragWeapon",
@"short BragWeapon

Begin GameMode");
            }
            else if (str.StartsWith("ScriptName FGC06ElidorScript"))
            {
                str = str.Replace(
@"Begin GameMode

short ElidorWeapon",
@"short ElidorWeapon

Begin GameMode");
            }
            else if (str.StartsWith("Scriptname FGC06Script"))
            {
                str = str.Replace(
@"begin gamemode

short gaveweapon
short goblinsdead",
@"short gaveweapon
short goblinsdead

begin gamemode");
            }
            else if (str.StartsWith("ScriptName FGD01DefaultScript"))
            {
                str = str.Replace(
@"Begin GameMode

short foundjournal",
@"short foundjournal

Begin GameMode");
            }
            else if (str.StartsWith("Scriptname FGD03ViranusScript"))
            {
                str = str.Replace(
@"Begin GameMode

short foundblade",
@"short foundblade

Begin GameMode");
            }
            else if (str.StartsWith("Scriptname FGD04DefectorScript"))
            {
                str = str.Replace(
@"Begin Gamemode

short ratMaglir
short journalVAR
short topicVAR",
@"short ratMaglir
short journalVAR
short topicVAR

Begin Gamemode");
            }
            else if (str.StartsWith("Scriptname FGD05OreynScript"))
            {
                str = str.Replace(
@"Begin GameMode

short arpeniaVAR
short doOnce
short athome
short markerVAR",
@"short arpeniaVAR
short doOnce
short athome
short markerVAR

Begin GameMode");
            }
            else if (str.StartsWith("Scriptname FGD06Script"))
            {
                str = str.Replace(
@"Begin GameMode

short doOnce
short convoVAR",
@"short doOnce
short convoVAR

Begin GameMode");
            }
            else if (str.StartsWith("Scriptname JskarScript"))
            {
                str = str.Replace(
@"				Activate

			endif",
@"				Activate");
            }
            else if (str.StartsWith("ScriptName MG09Script"))
            {
                str = str.Replace("endif`", "endif");
            }
            else if (str.StartsWith("Scriptname MS10AldosScript"))
            {
                str = str.Replace(
@"begin OnDeath Player
short Doonce",
@"short Doonce
begin OnDeath Player");
            }
            else if (str.StartsWith("scriptname MS16Script"))
            {
                str = str.Replace(
@"begin gamemode
float fQuestDelayTime",
@"float fQuestDelayTime
begin gamemode");
            }
            else if (str.StartsWith("ScriptName MS26Script"))
            {//gets around a parser problem
                str = str.Replace("Set DateInJail To GameDaysPassed", "Set DateInJail to GameDaysPassed");
            }
            else if (str.StartsWith("scriptname MS49Script"))
            {
                str = str.Replace(
@"begin gamemode

float fQuestDelayTime",
@"float fQuestDelayTime

begin gamemode");
            }
            else if (str.StartsWith("Scriptname NQDChorrolScript"))
            {
                str = str.Replace(
@"begin gamemode

short HasGeldallNotice
short PCSpokeToJauffre",
@"short HasGeldallNotice
short PCSpokeToJauffre

begin gamemode");
            }
            else if (str.StartsWith("scn OBTrapSmallTowerSpike01SCRIPT"))
            {//This replacement matches Moncleus's.
                str = str.Replace(
@"begin gameMode

",
@"begin gameMode

if activated == 0
");
            }
            else if (str.StartsWith("ScriptName RavenCamoranScript"))
            {
                str = str.Replace(
@"Begin Gamemode

ref pointRef
ref backupRef

short realSpeed
float resurrectTimer		; used to resurrect him in Paradise
short dead					; used to resurrect in Paradise",
@"ref pointRef
ref backupRef

short realSpeed
float resurrectTimer		; used to resurrect him in Paradise
short dead					; used to resurrect in Paradise

Begin Gamemode");
            }
            else if (str.StartsWith("\r\nscn SE02QuestScript"))
            {
                str = str.Replace("short metaActiveQuest 			0 = not set or both, 1 = arrows, 2 = tears", "short metaActiveQuest 			;0 = not set or both, 1 = arrows, 2 = tears");
            }
            else if (str.StartsWith("scn SE03Room02KeyGeneratorSCRIPT"))
            {
                str = str.Replace("set keyCount to keyCount -1", "set keyCount to (keyCount - 1)");
            }
            else if (str.StartsWith("ScriptName SE05TestLeverScript"))
            {//This replacement matches Moncleus's.
                str = str.Replace(
@"	if ( timer <= 0 )
		if ( update == 1 )
		





		




			endif
		endif
	else
",
@"	if ( timer > 0 )
");
            }
            else if (str.StartsWith("scn SE09BodyPartActivatorScript"))
            {
                str = str.Replace(
@";Description:
; This script is used to add body parts to the player's inventory. 
:
; We are faking the player picking up and putting down body parts, which are actually activators being enabled/disabled
; and adding/removing the equivelant body part to the players inventory.
:
; This way we can have a non-havocable item seem like its getting added and placed back where it was found
; if the player picks up the other body part of the same type.
;
;SE09.whichHead, SE09.whichXXXX are used in other scripts to know which body part the player picked
",
@";Description:
; This script is used to add body parts to the player's inventory. 
;
; We are faking the player picking up and putting down body parts, which are actually activators being enabled/disabled
; and adding/removing the equivelant body part to the players inventory.
;
; This way we can have a non-havocable item seem like its getting added and placed back where it was found
; if the player picks up the other body part of the same type.
;
;SE09.whichHead, SE09.whichXXXX are used in other scripts to know which body part the player picked");
            }
            else if (str.StartsWith("ScriptName SE10DSScript"))
            {
                str = str.Replace(
@"		else
			Activate
		endif",
@"
;		else
;			Activate
;		endif");
            }
            else if (str.StartsWith("Scriptname SE10MessengerScript"))
            {
                str = str.Replace(
@"			if ( GetStage SE10 == 3 )
				if ( GetDistance SE10MessengerMarker <= 100 )
					set doonce to 2
				endif
			endif
		endif",
@"		if ( GetStage SE10 == 3 )
			if ( GetDistance SE10MessengerMarker <= 100 )
				set doonce to 2
			endif
		endif");
            }
            else if (str.StartsWith("scn SE12GnarlOrderedScript") || str.StartsWith("scn SE12GnarlScript"))
            {
                str = str.Replace(
@"Short Effect
Short ShowShader
Float GrowTimer
Float NewScale",
@"").Replace(
@"short init				; set to 1 when creature is first initialized
ref mySelf",
@"short init				; set to 1 when creature is first initialized
ref mySelf
Short Effect
Short ShowShader
Float GrowTimer
Float NewScale");
            }
            else if (str.StartsWith("scn SE13KnightOfOrderSCRIPT"))
            {
                str = str.Replace(
@"short dying		; 1 = killing myself (after J dies), 2 = I'm dead",
@"").Replace(
@"ref actionRef		; who is activating me?

float timer			; respawn timer",
@"ref actionRef		; who is activating me?

float timer			; respawn timer
short dying		; 1 = killing myself (after J dies), 2 = I'm dead");
            }
            else if (str.StartsWith("scn SE14AttackCountScript"))
            {
                str = str.Replace(
@"Short Effect
Short ShowShader
Float GrowTimer
Float NewScale",
@"").Replace(
@"scn SE14AttackCountScript
",
@"scn SE14AttackCountScript

Short Effect
Short ShowShader
Float GrowTimer
Float NewScale");
            }
            else if (str.StartsWith("scn SEAimableFanrieneSCRIPT"))
            {
                str = str.Replace("set cower to  -cower", "set cower to (cower * -1)");
            }
            else if (str.StartsWith("scriptname SERakheranScript"))
            {
                str = str.Replace("short	HasSpokenTo Player		;Set to 1 after Rakheran greets the player", "short	HasSpokenToPlayer		;Set to 1 after Rakheran greets the player");
            }
            else if (str.StartsWith("ScriptName TandilweScript"))
            {
                str = str.Replace(
@"			endif
			elseif ( solvusref == 0 )",
@"			elseif ( solvusref == 0 )").Replace(
@"			endif
			elseif ( aiaref == 0 )",
@"			elseif ( aiaref == 0 )").Replace(
@"			endif
			elseif ( jorckref == 0 )",
@"			elseif ( jorckref == 0 )");
            }
            else if (str.StartsWith("Scriptname Timer"))
            {
                str = str.Replace("set CurrentTime to GameHour -12", "set CurrentTime to (GameHour - 12)");
            }
            return str;
        }

        private static readonly Regex getStartingPosOrAngleUnquotedArguments = new Regex("(getstartingpos|getstartingangle) (x|y|z)", RegexOptions.Compiled);
        private static string FixBuildErrors(string str)
        {
            //WTM:  Change:  In tif__0101efc0 and other TGExpelledScripts, PayFine gets misunderstood as a function call instead of a variable.
            str = str.Replace("TGExpelled.PayFine", "TGExpelled.PayFineTemp");
            //WTM:  Change:  In tgcastout, the below replacement must be made to accomodate for the above change.
            if (str.StartsWith("ScriptName TGCastOut"))
            {
                str = str.Replace("Float PayFine", "Float PayFineTemp");
            }
            //WTM:  Change:  In darkperenniaghostscript, Disable gets misunderstood as a function call instead of a variable.
            else if (str.StartsWith("Scriptname DarkPerenniaGhostScript"))
            {
                str = str.Replace("Disable", "DisableTemp");
            }
            //WTM:  Change:  In blade3script, Look gets misunderstood as a function call instead of a variable.
            else if (str.StartsWith("Scriptname Blade3Script"))
            {
                str = str
                    .Replace("short Look", "short LookTemp")
                    .Replace("if Look == ", "if LookTemp == ")
                    .Replace("set Look to ", "set LookTemp to ");
            }
            //WTM:  Change:  In qf_tg03elven_01034ea2_100_0, a variable is apparently accidentally quoted.
            else if (str.Contains("TG03LlathasasBustMarker.PlaceAtMe \"TG03LlathasasBust\""))
            {
                str = str.Replace("TG03LlathasasBustMarker.PlaceAtMe \"TG03LlathasasBust\"", "TG03LlathasasBustMarker.PlaceAtMe TG03LlathasasBust");
            }
            //WTM:  Change:  In sebruscusdannusitemscript, getstartingpos's arguments are not quoted.
            else if (str.StartsWith("scn SEBruscusDannusItemSCRIPT"))
            {
                str = getStartingPosOrAngleUnquotedArguments.Replace(str, "$1 \"$2\"");
            }
            //WTM:  Change:  In DABoethiaCageOpenScript01, a variable gets declared as "Short Salutation" which conflicts with a Topic named Salutation.
            //This page shows the Topic named Salutation, which is apparently set based on the player's race:  http://jpmod.oblivion.z49.org/?Vanilla%2FDialogue%2FDABoethia
            else if (str.StartsWith("scriptName DABoethiaCageOpenScript01"))
            {
                str = str.Replace("Short Salutation", "Short SalutationInt").Replace("set salutation to ", "set SalutationInt to ").Replace("Salutation == 1", "SalutationInt == 1");
            }
            //WTM:  Change:  In HackdirtMoslinsInnBedScript, an integer (1) is stored in a variable that previously stored an actor.
            //Since 1 is meant only to prevent the code from running again, I'm instead going to store a reference to HackdirtBrethren01.
            else if (str.StartsWith("scn HackdirtMoslinsInnBedScript"))
            {
                str = str.Replace("set attackRef to 1", "set attackRef to HackdirtBrethren01");
            }
            else if (str.StartsWith("Scriptname DiveRockScript"))
            {
                str = str.Replace("message", "messageVar");
            }
            else if (str.StartsWith("scn OblivionSmallHallPressurePlate01repeatSCRIPT"))
            {
                str = str.Replace("short timer\r\n", "");
            }
            else if (str.StartsWith("Scriptname DarkLachanceHuntedScript") || str.StartsWith("Scriptname DarkLachanceScript"))
            {
                str = str.Replace("Wait", "WaitVar");
            }
            else if (str.StartsWith("Scriptname Dark19StatueScript"))
            {
                str = str.Replace("short Cast", "short CastVar").Replace("if Cast == 0", "if CastVar == 0").Replace("if Cast == 0", "if CastVar == 0").Replace("set Cast to 1", "set CastVar to 1").Replace("set cast to 1", "set CastVar to 1").Replace("if Cast == 1", "if CastVar == 1").Replace("set Cast to 0", "set CastVar to 0");
            }
            else if (str.StartsWith("Scriptname SE10CapstoneScript"))
            {
                str = str.Replace("short cast", "short castVar").Replace("cast == ", "castVar == ").Replace("set cast to ", "set castVar to ").Replace(";set Cast to 1", ";set castVar to 1").Replace(";when cast is set to 1", ";when castVar is set to 1");
            }
            else if (str.StartsWith("scriptname Dark10SkullTriggerScript"))
            {
                str = str.Replace("short cast", "short castVar").Replace("cast == 0", "castVar == 0").Replace("set cast to 1", "set castVar to 1");
            }
            else if (str.StartsWith("scn MQConversationsScript"))
            {
                str = str.Replace("ref OGDeadDaedra", "short OGDeadDaedra");
            }
            else if (str.StartsWith("Scriptname MS43Script"))
            {
                str = str.Replace("DropMe", "DropMeVar");
            }
            else if (str.StartsWith("PlayerRef.AddItem Gold001 300"))//qf_tg05misdirection_01036332_60_0
            {
                str = str.Replace("PlayerRef.AddItem Gold001 300", "Player.AddItem Gold001 300");
            }
            return str.Trim() + "\r\n";
        }

        private static string MatchSourceFiles(string str)//WTM:  Added:  meant to make files modified source files from Monocleus
        {
            if (str.StartsWith("scn CGAkaviriLongswordScript"))
            {
                str = str.Replace(
@"
		if getcontainer == 0
			if getdisabled == 0
				disable
			endif
		else
			removeMe
		endif", "");
            }
            else if (str.StartsWith("scn CGBladesEquipmentScript"))
            {
                str = str.Replace(
@"
		if getcontainer == 0
			if getdisabled == 0
				disable
			endif
		elseif getcontainer != GlenroyRef
			removeMe
		endif", "");
            }
            return str;
        }

        public static string FixScriptErrors(string str)
        {
            return MatchSourceFiles(FixBuildErrors(FixSyntaxErrors(str)));
        }
    }
}
