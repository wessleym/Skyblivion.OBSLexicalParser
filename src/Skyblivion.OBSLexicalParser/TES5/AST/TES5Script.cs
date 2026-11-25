using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5Script : ITES5Outputtable
    {
        public TES5ScriptHeader ScriptHeader { get; }
        public TES5GlobalScope GlobalScope { get; }
        public IReadOnlyList<ITES5CodeBlock> Blocks { get; }
        private readonly IReadOnlyList<ITES5Outputtable> blocksAndComments;
        private readonly bool isQuestOrTopicInfo;
        public TES5Script(TES5GlobalScope globalScope, IReadOnlyList<ITES5CodeBlock> blocks, IReadOnlyList<ITES5Outputtable> blocksAndComments, bool isQuestOrTopicInfo)
        {
            this.ScriptHeader = globalScope.ScriptHeader;
            this.GlobalScope = globalScope;
            this.Blocks = blocks;
            this.blocksAndComments = blocksAndComments;
            this.isQuestOrTopicInfo = isQuestOrTopicInfo;
        }
        public TES5Script(TES5GlobalScope globalScope, IReadOnlyList<ITES5CodeBlock> blocks, bool isQuestOrTopicInfo) : this(globalScope, blocks, blocks, isQuestOrTopicInfo) { }

        public IEnumerable<string> Output
        {
            get
            {
                //WTM:  Change:
                if (isQuestOrTopicInfo)
                {
                    return OutputQuestOrTopicInfo;
                }
                var scriptHeaderAndGlobalScope = new TES5ScriptHeaderAndGlobalScope(ScriptHeader, GlobalScope);
                return scriptHeaderAndGlobalScope.ScriptHeaderOutput
                    .Concat(scriptHeaderAndGlobalScope.GlobalScopeOutput)
                    .Concat(
                        blocksAndComments.SelectMany((x, i) =>
                        {
                            //Add line breaks only when the previous output was not a comment or (when the previous comment was just comment characters (e.g., ;;;;;) and (the current line is not just a comment with comment characters)).
                            TES5Comment? previousComment = i != 0 ? blocksAndComments[i - 1] as TES5Comment : null;
                            TES5Comment? currentComment = x as TES5Comment;
                            return (previousComment == null || (previousComment.ContainsOnlyCommentCharacters && !(currentComment != null && currentComment.ContainsOnlyCommentCharacters)) ? new string[] { "" } : []).Concat(x.Output);
                        })
                    );
            }
        }

        private static readonly Regex fragmentRE = new Regex("^Fragment_([0-9]+)(_[0-9]+)?$", RegexOptions.Compiled);
        private int GetNextFragmentIndex()
        {
            if (Blocks.Count == 0) { return 0; }
            return Blocks.Select(b =>
            {
                Match fragmentMatch = fragmentRE.Match(b.BlockName);
                if (!fragmentMatch.Success) { throw new InvalidOperationException("Fragment match failed for block name " + b.BlockName); }
                return int.Parse(fragmentMatch.Groups[1].Value);
            }).Max() + 1;
        }

        private IEnumerable<string> OutputQuestOrTopicInfo
        {
            get
            {
                var scriptHeaderAndGlobalScope = new TES5ScriptHeaderAndGlobalScope(ScriptHeader, GlobalScope);
                yield return ";BEGIN FRAGMENT CODE - Do not edit anything between this and the end comment";
                yield return ";NEXT FRAGMENT INDEX " + GetNextFragmentIndex();
                foreach (string o in scriptHeaderAndGlobalScope.ScriptHeaderOutput) { yield return o; }
                foreach (string o in blocksAndComments.SelectMany(x => new string[] { "" }.Concat(x.Output)))
                {
                    yield return o;
                }
                yield return ";END FRAGMENT CODE - Do not edit anything between this and the begin comment";
                foreach (string o in scriptHeaderAndGlobalScope.GlobalScopeOutput) { yield return o; }
            }
        }

        public const string Indent = "    ";
        //Only indents non-empty lines
        public static string WithIndent(string output)
        {
            return !string.IsNullOrWhiteSpace(output) ? Indent + output : "";
        }

        public static bool AvoidEmptyStart(string output, int i)
        {
            return !(i == 0 && output == "");
        }
    }
}