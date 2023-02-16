#define INCLUDE_LEXER_FIXES
using Dissect.Lexer;
using Dissect.Lexer.TokenStream;
using System.Collections.Generic;
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

        private const string crlfRegex = @"^[\r\n]+[ \t\r\n]*";//CRLF needs to absorb any whitespace that comes after it.  Otherwise, the parser may not know how to handle the whitespace.
        private void AddCommentsRecognition(bool skipCRLF = true)
        {
            List<string> skip = new List<string>() { "WSP", "WSPOneLine"/*, "Comment", ""*/, "to ", "(", ")", "," };
            if (skipCRLF) { skip.Add("CRLF"); }
            this
            .RegexIgnoreCase("CommentInitialization", @"^;").Action("CommentScope")
            //WTM:  Change:  Process comments.  Don't ignore them.
            .Token("(")
            .Token(")")
            .Token(",")
            .Skip(skip.ToArray())
            ;
        }

        protected void BuildObscriptLexer()
        {
            //Global scope.
            this.State("globalScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("CRLF", crlfRegex);//See arenascript.txt lines 26-28.  It has "\r\n \r\n", which was lexed to CRLF+CRLF.  Without the middle space, this would have just become CRLF.  I added [ \t\r\n]* to absorb trailing whitespace.
            this.RegexIgnoreCase("ScriptHeaderToken", "^(scn|scriptName)")
                .Action("ScriptHeaderScope");
            this.RegexIgnoreCase("VariableDeclarationType", @"^(ref|short|long|float|int)")
                .Action("VariableDeclarationScope");
            this.Token("BlockStart", "Begin", true)//WTM:  Change:  this.regexIgnoreCase("BlockStart", @"^Begin")
                .Action("BlockStartNameScope");
            this.AddCommentsRecognition(skipCRLF: false);

            this.State("ExpressionScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("CRLF", crlfRegex).Action("BlockScope")
                .Regex("FunctionCallToken", FUNCTION_REGEX).Action("FunctionScope")
                .RegexIgnoreCase("Boolean", @"^(true|false)")
                .Regex("ReferenceToken", @"^[A-Za-z][A-Za-z0-9]*")//WTM:  Change:  regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .Regex("Float", @"^(-)?([0-9]*)\.[0-9]+")//WTM:  Change:  regexIgnoreCase
                .Regex("Integer", @"^(-)?(0|[1-9][0-9]*)")//WTM:  Change:  regexIgnoreCase
                .Regex("String", @"^""((?:(?<=\\)[""]|[^""])*)""")//WTM:  Change:  regexIgnoreCase
                .Token("TokenDelimiter", ".")//WTM:  Change:  .regexIgnoreCase("TokenDelimiter", @"\.")
                .Token("+")
                .Token("-")
                .Token("*")
                .Token("/")
                .Token("==")
                .Token(">=")
                .Token("<=")
                .Token(">")
                .Token("<")
                .Token("!=")
                .Token("&&")
                .Token("||");

            this.AddCommentsRecognition();

            this.State("BlockScope")
                .Regex("WSP", @"^[ \t\r\n]+")
                .RegexIgnoreCase("BlockEnd", @"^end( [a-zA-Z]+)?").Action("BlockEndScope")
                .RegexIgnoreCase("BranchElseifToken", @"^else[ ]?if(\()?[ \r\n\t]+").Action("ExpressionScope")
                .RegexIgnoreCase("BranchStartToken", @"^if(\()?[ \r\n\t]+").Action("ExpressionScope")
                .Token("BranchElseToken", "else", true)//WTM:  Change:  .regexIgnoreCase("BranchElseToken", @"^else")
                .Token("BranchEndToken", "endif", true)//WTM:  Change:  .regexIgnoreCase("BranchEndToken", @"^endif")
                .RegexIgnoreCase("SetInitialization", @"^set[ \t]+").Action("SetScope")
                .Token("ReturnToken", "return", true)//WTM:  Change:  .regexIgnoreCase("ReturnToken", @"^return")
                .Regex("Float", @"^(-)?([0-9]*)\.[0-9]+")//WTM:  Change:  regexIgnoreCase
                .Regex("Integer", @"^(-)?(0|[1-9][0-9]*)")//WTM:  Change:  regexIgnoreCase
                .Regex("String", @"^""((?:(?<=\\)[""]|[^""])*)""")//WTM:  Change:  regexIgnoreCase
                .RegexIgnoreCase("Boolean", @"^(true|false)")
                .Regex("FunctionCallToken", FUNCTION_REGEX).Action("FunctionScope")
                .RegexIgnoreCase("LocalVariableDeclarationType", @"^(ref|short|long|float|int)")
                .Action("VariableDeclarationScope")
                .Regex("ReferenceToken", @"^[A-Za-z][A-Za-z0-9]*")//WTM:  Change:  regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .Token("TokenDelimiter", ".");//WTM:  Change:  .regexIgnoreCase("TokenDelimiter", @"^\.")

            this.AddCommentsRecognition(skipCRLF: false);

            this.State("FunctionScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("CRLF", crlfRegex).Action("BlockScope")
                .Token("ReturnToken", "return", true)//WTM:  Change:  .regexIgnoreCase("ReturnToken", @"^return")
                .Regex("Float", @"^(-)?([0-9]*)\.[0-9]+")//WTM:  Change:  regexIgnoreCase
                .Regex("Integer", @"^(-)?(0|[1-9][0-9]*)")//WTM:  Change:  regexIgnoreCase
                .Regex("String", @"^""((?:(?<=\\)[""]|[^""])*)""")//WTM:  Change:  regexIgnoreCase
                .RegexIgnoreCase("Boolean", @"^(true|false)")
                .Regex("FunctionCallToken", FUNCTION_REGEX)
                .Regex("ReferenceToken", @"^[A-Za-z][A-Za-z0-9]*")//WTM:  Change:  regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .Token("TokenDelimiter", ".")//WTM:  Change:  .regexIgnoreCase("TokenDelimiter", @"^\.")
                .Token("+").Action(POP_STATE)
                .Token("-").Action(POP_STATE)
                .Token("*").Action(POP_STATE)
                .Token("/").Action(POP_STATE)
                .Token("==").Action(POP_STATE)
                .Token(">=").Action(POP_STATE)
                .Token("<=").Action(POP_STATE)
                .Token(">").Action(POP_STATE)
                .Token("<").Action(POP_STATE)
                .Token("!=").Action(POP_STATE)
                .Token("&&").Action(POP_STATE)
                .Token("||").Action(POP_STATE);

            this.AddCommentsRecognition();


            this.State("SetScope")
                .Regex("ReferenceToken", @"^[A-Za-z][A-Za-z0-9]*")//WTM:  Change:  regexIgnoreCase("ReferenceToken", @"^[a-z][a-zA-Z0-9]*")
                .Token("TokenDelimiter", ".")//WTM:  Change:  .regexIgnoreCase("TokenDelimiter", @"\.")
                .Token("To ").Action("ExpressionScope")
                .Token("to ").Action("ExpressionScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("CRLF", crlfRegex).Action(POP_STATE);
            this.AddCommentsRecognition();

            this.State("BlockEndScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("CRLF", crlfRegex).Action("globalScope");
            this.AddCommentsRecognition();

            this.State("BlockStartNameScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("BlockType", @"^([a-zA-Z0-9_-]+)")//WTM:  Change:  .regexIgnoreCase
                .Action("BlockStartParameterScope");
            this.AddCommentsRecognition();

            this.State("BlockStartParameterScope")
                .Regex("WSPOneLine", @"^[ \t]+")
                .Regex("BlockParameterToken", @"[a-zA-Z0-9_-]+")//WTM:  Change:  .regexIgnoreCase
                .Regex("CRLF", crlfRegex).Action("BlockScope");
            this.AddCommentsRecognition();

            this.State("ScriptHeaderScope")
                .Regex("WSP", @"^[ \r\n\t]+")
                .Regex("ScriptName", @"^([a-zA-Z0-9_-]+)")//WTM:  Change:  .regexIgnoreCase
                .Action(POP_STATE);
            this.AddCommentsRecognition();

            this.State("VariableDeclarationScope")
                .Regex("WSP", @"^[ \r\n\t]+")
                .Regex("VariableName", @"^([a-zA-Z0-9_-]+)")//WTM:  Change:  .regexIgnoreCase
                .Action(POP_STATE);
            this.AddCommentsRecognition();

            this.State("CommentScope")
                .Regex("CommentText", ".*")
                .Action(POP_STATE);
        }

        public ArrayTokenStream LexWithFixes(string str)
        {
            str = OBScriptFixer.FixScriptErrors(str);
            ArrayTokenStream tokens = Lex(str);
            return tokens;
        }
    }
}