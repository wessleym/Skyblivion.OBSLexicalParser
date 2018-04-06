#define INCLUDE_LEXER_FIXES
using Dissect.Lexer;
using Dissect.Lexer.TokenStream;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class OBScriptLexer : StatefulLexer
    {
        //WTM:  Change:  I added the function names in INCLUDE_LEXER_FIXES.  I added all of them to TES5ValueFactoryFunctionFiller as NotSupportedFactory.
        //See http://en.uesp.net/wiki/Tes4Mod:Script_Functions
        public static readonly Regex FUNCTION_REGEX = new Regex(
@"^(activate|addachievement|additem|addscriptpackage|addspell|addtopic|autosave|cast|clearownership|closecurrentobliviongate|closeobliviongate|completequest|createfullactorcopy|deletefullactorcopy|disablelinkedpathpoints|disableplayercontrols|disable|dispel|dropme|drop|duplicateallitems|enablefasttravel|enablelinkedpathpoints|enableplayercontrols|enable|equipitem|essentialdeathreload|evp|evaluatepackage|forceactorvalue|forceav|forcecloseobliviongate|" +
#if INCLUDE_LEXER_FIXES
@"forceflee|" +
#endif
@"forceweather|fw|getactionref|getav|getactorvalue|getamountsoldstolen|getangle|getarmorrating|getattacked|getbaseav|getbaseactorvalue|getbuttonpressed|getclothingvalue|getcombattarget|getcontainer|getcrimegold|getcrimeknown|getcrime|getcurrentaipackage|getcurrentaiprocedure|getcurrenttime|getdayofweek|getdeadcount|getdead|getdestroyed|getdetected|getdetectionlevel|getdisabled|getdisposition|getdistance|getequipped|getfactionrank|getforcesneak|getgamesetting|getgold|getgs|getheadingangle|getincell|getinfaction|getinsamecell|getinworldspace|getisalerted|getiscreature|getiscurrentpackage|getiscurrentweather|getisid|getisplayablerace|getisplayerbirthsign|getisrace|getisreference|getissex|getitemcount|getknockedstate|getlevel|getlocked|getlos|getopenstate|getparentref|getpcexpelled|getpcfactionmurder|getpcfactionattack|getpcfactionsteal|getpcfame|getpcinfamy|getpcisrace|getpcissex|getpcmiscstat|getplayercontrolsdisabled|getplayerinseworld|getpos|getquestrunning|getrandompercent|getrestrained|getsecondspassed|this|getself|getshouldattack|getsitting|getsleeping|getstagedone|getstage|getstartingangle|getstartingpos|gettalkedtopc|getweaponanimtype|gotojail|hasmagiceffect|hasvampirefed|isactionref|isactordetected|isactorusingatorch|isactor|isanimplaying|isessential|isguard|isidleplaying|isincombat|isindangerouswater|isininterior|isowner|ispcamurderer|ispcsleeping|isplayerinjail|israining|isridinghorse|issneaking|isspelltarget|isswimming|istalking|istimepassing|isweaponout|isxbox|kill|lock|look|menumode|messagebox|message|modactorvalue|" +
#if INCLUDE_LEXER_FIXES
@"modamountsoldstolen|" +
#endif
@"modav|modcrimegold|moddisposition|modfactionreaction|modpcfame|modpcinfamy|modpcmiscstat|modpcskill|movetomarker|moveto|" +
#if INCLUDE_LEXER_FIXES
@"payfinethief|" +
#endif
@"payfine|pickidle|placeatme|playgroup|playmagiceffectvisuals|pme|" +
#if INCLUDE_LEXER_FIXES
@"playbink|" +
#endif
@"playmagicshadervisuals|pms|playsound3d|playsound|purgecellbuffers|pcb|pushactoraway|refreshtopiclist|releaseweatheroverride|removeallitems|removeitem|removeme|removescriptpackage|removespell|reset3dstate|resetfalldamagetimer|resethealth|resetinterior|resurrect|rotate|sayto|say|scripteffectelapsedseconds|sendtrespassalarm|setactoralpha|saa|setactorfullname|setactorrefraction|setactorsai|setactorvalue|setav|setalert|setallreachable|setallvisible|setangle|" +
#if INCLUDE_LEXER_FIXES
@"setcellfullname|" +
#endif
@"setcellpublicflag|" +
#if INCLUDE_LEXER_FIXES
@"setcellownership|" +
#endif
@"setclass|setcombatstyle|setcrimegoldnonviolent|setcrimegold|setdestroyed|setdoordefaultopen|setessential|setfactionrank|setfactionreaction|setforcerun|setforcesneak|setghost|setignorefriendlyhits|" +
#if INCLUDE_LEXER_FIXES
@"setinchargen|" +
#endif
@"setinvestmentgold|setitemvalue|" +
#if INCLUDE_LEXER_FIXES
@"setlevel|" +
#endif
@"setnoavoidance|setnorumors|setopenstate|setownership|setpackduration|setpcexpelled|setpcfactionattack|setpcfactionmurder|setpcfactionsteal|setpcfame|setpcinfamy|setplayerinseworld|setpos|" +
#if INCLUDE_LEXER_FIXES
@"setpublic|" +
#endif
@"setquestobject|setrestrained|setrigidbodymass|setscale|setsceneiscomplex|setshowquestitems|setstage|setunconscious|setweather|showbirthsignmenu|showclassmenu|showdialogsubtitles|showenchantment|showmap|showracemenu|showspellmaking|sme|startcombat|startconversation|startquest|stopcombatalarmonactor|stopcombat|scaonactor|stoplook|stopmagiceffectvisuals|stopmagicshadervisuals|stopwaiting|sms|stopquest|sw|trapupdate|triggerhitshader|unequipitem|unlock|wait|wakeuppc|yield)"
, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private void addCommentsRecognition()
        {
            this
            .regex("Comment", @"^;.*")
            .token("(")
            .token(")")
            .token(",")
            .token("TimerDescending") //idk wtf is that.
            .skip("WSP", "WSPEOL", "Comment", "", "to ", "(", ")", ",", "TimerDescending", "NotNeededTrash");
        }

        protected void buildObscriptLexer()
        {
            //Global scope.
            this._state("globalScope");
            this.regex("WSP", @"^[ \r\n\t]+");
            this.regexIgnoreCase("ScriptHeaderToken", "^(scn|scriptName)").action("ScriptHeaderScope");
            this.regexIgnoreCase("VariableDeclarationType", @"^(ref|short|long|float|int)").action("VariableDeclarationScope");
            this.regexIgnoreCase("BlockStart", @"^Begin").action("BlockStartNameScope");
            this.addCommentsRecognition();

            this._state("ExpressionScope")
                .regex("WSP", @"^[ \t]+")
                .regex("NWL", @"^[\r\n]+").action("BlockScope")
                .regex("FunctionCallToken", FUNCTION_REGEX).action("FunctionScope")
                .regexIgnoreCase("Boolean", @"^(true|false)")
                .regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .regexIgnoreCase("Float", @"^(-)?([0-9]*)\.[0-9]+")
                .regexIgnoreCase("Integer", @"^(-)?(0|[1-9][0-9]*)")
                .regexIgnoreCase("String", @"^""((?:(?<=\\)[""]|[^""])*)""")
                .regexIgnoreCase("TokenDelimiter", @"\.")
                .token("+")
                .token("-")
                .token("*")
                .token("/")
                .token("==")
                .token(">=")
                .token("<=")
                .token(">")
                .token("<")
                .token("!=")
                .token("&&")
                .token("||")
                .skip("NWL");

            this.addCommentsRecognition();

            this._state("BlockScope")
                .regex("WSP", @"^[ \t\r\n]+")
                .regexIgnoreCase("BlockEnd", @"^end( [a-zA-Z]+)?").action("BlockEndScope")
                .regexIgnoreCase("BranchElseifToken", @"^else[ ]?if(\()?[ \r\n\t]+").action("ExpressionScope")
                .regexIgnoreCase("BranchStartToken", @"^if(\()?[ \r\n\t]+").action("ExpressionScope")
                .regexIgnoreCase("BranchElseToken", @"^else")
                .regexIgnoreCase("BranchEndToken", @"^endif")
                .regexIgnoreCase("SetInitialization", @"^set[ \t]+").action("SetScope")
                .regexIgnoreCase("ReturnToken", @"^return")
                .regexIgnoreCase("Float", @"^(-)?([0-9]*)\.[0-9]+")
                .regexIgnoreCase("Integer", @"^(-)?(0|[1-9][0-9]*)")
                .regexIgnoreCase("String", @"^""((?:(?<=\\)[""]|[^""])*)""")
                .regexIgnoreCase("Boolean", @"^(true|false)")
                .regex("FunctionCallToken", FUNCTION_REGEX).action("FunctionScope")
                .regexIgnoreCase("LocalVariableDeclarationType", @"^(ref|short|long|float|int)").action("VariableDeclarationScope")
                .regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .regexIgnoreCase("TokenDelimiter", @"^\.");

            this.addCommentsRecognition();

            this._state("FunctionScope")
                .regex("WSP", @"^[ \t]+")
                .regex("NWL", @"^[\r\n]+").action("BlockScope")
                .regexIgnoreCase("ReturnToken", @"^return")
                .regexIgnoreCase("Float", @"^(-)?([0-9]*)\.[0-9]+")
                .regexIgnoreCase("Integer", @"^(-)?(0|[1-9][0-9]*)")
                .regexIgnoreCase("String", @"^""((?:(?<=\\)[""]|[^""])*)""")
                .regexIgnoreCase("Boolean", @"^(true|false)")
                .regex("FunctionCallToken", FUNCTION_REGEX)
                .regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .regexIgnoreCase("TokenDelimiter", @"^\.")
                .token("+").action(POP_STATE)
                .token("-").action(POP_STATE)
                .token("*").action(POP_STATE)
                .token("/").action(POP_STATE)
                .token("==").action(POP_STATE)
                .token(">=").action(POP_STATE)
                .token("<=").action(POP_STATE)
                .token(">").action(POP_STATE)
                .token("<").action(POP_STATE)
                .token("!=").action(POP_STATE)
                .token("&&").action(POP_STATE)
                .token("||").action(POP_STATE);


            this.addCommentsRecognition();


            this._state("SetScope")
                 .regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                 .regexIgnoreCase("TokenDelimiter", @"\.")
                 .token("To ").action("ExpressionScope")
                 .token("to ").action("ExpressionScope")
                 .regex("WSP", @"^[ \t]+")
                 .regex("NWL", @"^[\r\n]+").action(POP_STATE);
            this.addCommentsRecognition();

            this._state("BlockEndScope")
                .regex("WSP", @"^[ \t]+")
                //.regexIgnoreCase("NotNeededTrash",@"^([a-zA-Z0-9_-]+)") I kinda forgot why it is here..
                .regex("WSPEOL", @"^[\r\n]+").action("globalScope");
            this.addCommentsRecognition();

            this._state("BlockStartNameScope")
                .regex("WSP", @"^[ \t]+")
                .regexIgnoreCase("BlockType", @"^([a-zA-Z0-9_-]+)").action("BlockStartParameterScope");
            this.addCommentsRecognition();

            this._state("BlockStartParameterScope")
                .regex("WSP", @"^[ \t]+")
                .regexIgnoreCase("BlockParameterToken", @"[a-zA-Z0-9_-]+")
                .regex("WSPEOL", @"^[\r\n]+").action("BlockScope");
            this.addCommentsRecognition();

            this._state("ScriptHeaderScope")
                .regex("WSP", @"^[ \r\n\t]+")
                .regexIgnoreCase("ScriptName", @"^([a-zA-Z0-9_-]+)").action(POP_STATE);
            this.addCommentsRecognition();

            this._state("VariableDeclarationScope")
                .regex("WSP", @"^[ \r\n\t]+")
                .regexIgnoreCase("VariableName", @"^([a-zA-Z0-9_-]+)").action(POP_STATE);
            this.addCommentsRecognition();
        }

        public ArrayTokenStream LexWithFixes(string str)
        {
            //WTM:  Change:  In tif__0101efc0 and other TGExpelledScripts, PayFine gets misunderstood as a function call instead of a variable.
            str = str.Replace("TGExpelled.PayFine", "TGExpelled.PayFineTemp");
            //WTM:  Change:  In tgcastout, the below replacement must be made so the variable name can stay in synch.
            if (str.StartsWith("ScriptName TGCastOut"))
            {
                str = str.Replace("Float PayFine", "Float PayFineTemp");
            }
            //WTM:  Change:  In darkperenniaghostscript, Disable gets misunderstood as a function call instead of a variable.
            if (str.StartsWith("Scriptname DarkPerenniaGhostScript"))
            {
                str = str.Replace("Disable", "DisableTemp");
            }
            //WTM:  Change:  In blade3script, Look gets misunderstood as a function call instead of a variable.
            if (str.StartsWith("Scriptname Blade3Script"))
            {
                str = str
                    .Replace("short Look", "short LookTemp")
                    .Replace("if Look == ", "if LookTemp == ")
                    .Replace("set Look to ", "set LookTemp to ");
            }
            //WTM:  Change:  In qf_tg03elven_01034ea2_100_0, a variable is apparently accidentally quoted.
            str = str.Replace("TG03LlathasasBustMarker.PlaceAtMe \"TG03LlathasasBust\"", "TG03LlathasasBustMarker.PlaceAtMe TG03LlathasasBust");
            ArrayTokenStream tokens = lex(str);
            return tokens;
        }
    }
}