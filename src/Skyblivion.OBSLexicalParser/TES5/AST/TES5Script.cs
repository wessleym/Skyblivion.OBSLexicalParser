using Skyblivion.OBSLexicalParser.TES5.AST.Block;
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

        private readonly IReadOnlyCollection<ITES5Outputtable> blocksAndComments;
        private readonly bool isQuestOrTopicInfo;
        public const string Indent = "    ";
        public TES5Script(TES5GlobalScope globalScope, IReadOnlyList<ITES5CodeBlock> blocks, IReadOnlyCollection<ITES5Outputtable> blocksAndComments, bool isQuestOrTopicInfo)
        {
            this.ScriptHeader = globalScope.ScriptHeader;
            this.GlobalScope = globalScope;
            this.Blocks = blocks;
            this.blocksAndComments = blocksAndComments;
            this.isQuestOrTopicInfo = isQuestOrTopicInfo;
        }
        public TES5Script(TES5GlobalScope globalScope, IReadOnlyList<ITES5CodeBlock> blocks, bool isQuestOrTopicInfo)
            : this(globalScope, blocks, blocks, isQuestOrTopicInfo)
        { }

        public IEnumerable<string> Output
        {
            get
            {
                //WTM:  Change:
                if (isQuestOrTopicInfo)
                {
                    return OutputQuestOrTopicInfo;
                }
                return this.ScriptHeader.Output.Concat(this.GlobalScope.Output).Concat(this.blocksAndComments.SelectMany(x => x.Output));
            }
        }

        private static readonly Regex fragmentRE = new Regex("^Fragment_([0-9]+)(_[0-9]+)?$", RegexOptions.Compiled);
        private int GetNextFragmentIndex()
        {
            if (!Blocks.Any()) { return 0; }
            return Blocks.Select(b =>
            {
                Match fragmentMatch = fragmentRE.Match(b.BlockName);
                if (!fragmentMatch.Success) { throw new InvalidOperationException("Fragment match failed for block name " + b.BlockName); }
                return int.Parse(fragmentMatch.Groups[1].Value);
            }).Max() + 10;
        }

        private IEnumerable<string> OutputQuestOrTopicInfo
        {
            get
            {
                yield return ";BEGIN FRAGMENT CODE - Do not edit anything between this and the end comment";
                yield return ";NEXT FRAGMENT INDEX " + GetNextFragmentIndex();
                foreach (string o in ScriptHeader.Output) { yield return o; }
                yield return "";
                foreach (string o in blocksAndComments.SelectMany(x => x.Output)) { yield return o; }
                yield return ";END FRAGMENT CODE - Do not edit anything between this and the begin comment";
                foreach (string o in GlobalScope.Output) { yield return o; }
            }
        }
    }
}