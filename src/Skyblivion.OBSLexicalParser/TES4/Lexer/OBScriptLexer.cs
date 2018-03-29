using Dissect.Lexer;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Lexer
{
    class OBScriptLexer : StatefulLexer
    {
        //WTM:  Change:  I added the functions in INCLUDE_LEXER_FIXES, but while they can be output by BuildInteroperableCompilationGraphs, it doesn't seem they can be converted by BuildTargetCommand.
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
, RegexOptions.IgnoreCase);

        private void addCommentsRecognition()
        {
            this
            .regex("Comment", new Regex(@"^;.*"))
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
            this.regex("WSP", new Regex(@"^[ \r\n\t]+"));
            this.regex("ScriptHeaderToken", new Regex("^(scn|scriptName)", RegexOptions.IgnoreCase)).action("ScriptHeaderScope");
            this.regex("VariableDeclarationType", new Regex(@"^(ref|short|long|float|int)", RegexOptions.IgnoreCase)).action("VariableDeclarationScope");
            this.regex("BlockStart", new Regex(@"^Begin", RegexOptions.IgnoreCase)).action("BlockStartNameScope");
            this.addCommentsRecognition();

            this._state("ExpressionScope")
                .regex("WSP", new Regex(@"^[ \t]+"))
                .regex("NWL", new Regex(@"^[\r\n]+")).action("BlockScope")
                .regex("FunctionCallToken", FUNCTION_REGEX).action("FunctionScope")
                .regex("Boolean", new Regex(@"^(true|false)", RegexOptions.IgnoreCase))
                .regex("ReferenceToken", new Regex(@"^[a-z][a-zA-Z0-9]*", RegexOptions.IgnoreCase))
                .regex("Float", new Regex(@"^(-)?([0-9]*)\.[0-9]+", RegexOptions.IgnoreCase))
                .regex("Integer", new Regex(@"^(-)?(0|[1-9][0-9]*)", RegexOptions.IgnoreCase))
                .regex("String", new Regex(@"^""((?:(?<=\\)[""]|[^""])*)""", RegexOptions.IgnoreCase))
                .regex("TokenDelimiter", new Regex(@"\.", RegexOptions.IgnoreCase))
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
                .regex("WSP", new Regex(@"^[ \t\r\n]+"))
                .regex("BlockEnd", new Regex(@"^end( [a-zA-Z]+)?", RegexOptions.IgnoreCase)).action("BlockEndScope")
                .regex("BranchElseifToken", new Regex(@"^else[ ]?if(\()?[ \r\n\t]+", RegexOptions.IgnoreCase)).action("ExpressionScope")
                .regex("BranchStartToken", new Regex(@"^if(\()?[ \r\n\t]+", RegexOptions.IgnoreCase)).action("ExpressionScope")
                .regex("BranchElseToken", new Regex(@"^else", RegexOptions.IgnoreCase))
                .regex("BranchEndToken", new Regex(@"^endif", RegexOptions.IgnoreCase))
                .regex("SetInitialization", new Regex(@"^set[ \t]+", RegexOptions.IgnoreCase)).action("SetScope")
                .regex("ReturnToken", new Regex(@"^return", RegexOptions.IgnoreCase))
                .regex("Float", new Regex(@"^(-)?([0-9]*)\.[0-9]+", RegexOptions.IgnoreCase))
                .regex("Integer", new Regex(@"^(-)?(0|[1-9][0-9]*)", RegexOptions.IgnoreCase))
                .regex("String", new Regex(@"^""((?:(?<=\\)[""]|[^""])*)""", RegexOptions.IgnoreCase))
                .regex("Boolean", new Regex(@"^(true|false)", RegexOptions.IgnoreCase))
                .regex("FunctionCallToken", FUNCTION_REGEX).action("FunctionScope")
                .regex("LocalVariableDeclarationType", new Regex(@"^(ref|short|long|float|int)", RegexOptions.IgnoreCase)).action("VariableDeclarationScope")
                .regex("ReferenceToken", new Regex(@"^[a-z][a-zA-Z0-9]*", RegexOptions.IgnoreCase))
                .regex("TokenDelimiter", new Regex(@"^\.", RegexOptions.IgnoreCase));

            this.addCommentsRecognition();

            this._state("FunctionScope")
                .regex("WSP", new Regex(@"^[ \t]+"))
                .regex("NWL", new Regex(@"^[\r\n]+")).action("BlockScope")
                .regex("ReturnToken", new Regex(@"^return", RegexOptions.IgnoreCase))
                .regex("Float", new Regex(@"^(-)?([0-9]*)\.[0-9]+", RegexOptions.IgnoreCase))
                .regex("Integer", new Regex(@"^(-)?(0|[1-9][0-9]*)", RegexOptions.IgnoreCase))
                .regex("String", new Regex(@"^""((?:(?<=\\)[""]|[^""])*)""", RegexOptions.IgnoreCase))
                .regex("Boolean", new Regex(@"^(true|false)", RegexOptions.IgnoreCase))
                .regex("FunctionCallToken", FUNCTION_REGEX)
                .regex("ReferenceToken", new Regex(@"^[a-z][a-zA-Z0-9]*", RegexOptions.IgnoreCase))
                .regex("TokenDelimiter", new Regex(@"^\.", RegexOptions.IgnoreCase))
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
                 .regex("ReferenceToken", new Regex(@"^[a-z][a-zA-Z0-9]*", RegexOptions.IgnoreCase))
                 .regex("TokenDelimiter", new Regex(@"\.", RegexOptions.IgnoreCase))
                 .token("To ").action("ExpressionScope")
                 .token("to ").action("ExpressionScope")
                 .regex("WSP", new Regex(@"^[ \t]+"))
                 .regex("NWL", new Regex(@"^[\r\n]+")).action(POP_STATE);
            this.addCommentsRecognition();

            this._state("BlockEndScope")
                .regex("WSP", new Regex(@"^[ \t]+"))
                //            .regex("NotNeededTrash",new Regex(@"^([a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase)) I kinda forgot why it is here..
                .regex("WSPEOL", new Regex(@"^[\r\n]+")).action("globalScope");
            this.addCommentsRecognition();

            this._state("BlockStartNameScope")
                .regex("WSP", new Regex(@"^[ \t]+"))
                .regex("BlockType", new Regex(@"^([a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase)).action("BlockStartParameterScope");
            this.addCommentsRecognition();

            this._state("BlockStartParameterScope")
                .regex("WSP", new Regex(@"^[ \t]+"))
                .regex("BlockParameterToken", new Regex(@"[a-zA-Z0-9_-]+", RegexOptions.IgnoreCase))
                .regex("WSPEOL", new Regex(@"^[\r\n]+")).action("BlockScope");
            this.addCommentsRecognition();

            this._state("ScriptHeaderScope")
                .regex("WSP", new Regex(@"^[ \r\n\t]+"))
                .regex("ScriptName", new Regex(@"^([a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase)).action(POP_STATE);
            this.addCommentsRecognition();

            this._state("VariableDeclarationScope")
                .regex("WSP", new Regex(@"^[ \r\n\t]+"))
                .regex("VariableName", new Regex(@"^([a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase)).action(POP_STATE);
            this.addCommentsRecognition();
        }
    }
}