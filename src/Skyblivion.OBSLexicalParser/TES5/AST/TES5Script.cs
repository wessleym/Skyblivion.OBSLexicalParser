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
        public TES5BlockList BlockList { get; }
        private readonly bool isQuestOrTopicInfo;
        public const string Indent = "    ";
        public TES5Script(TES5GlobalScope globalScope, TES5BlockList blockList, bool isQuestOrTopicInfo)
        {
            this.ScriptHeader = globalScope.ScriptHeader;
            this.GlobalScope = globalScope;
            this.BlockList = blockList;
            this.isQuestOrTopicInfo = isQuestOrTopicInfo;
        }

        public IEnumerable<string> Output
        {
            get
            {
                //WTM:  Change:
                if (isQuestOrTopicInfo)
                {
                    return OutputQuestOrTopicInfo;
                }
                return this.ScriptHeader.Output.Concat(this.GlobalScope.Output).Concat(this.BlockList.Output);
            }
        }

        private static readonly Regex fragmentRE = new Regex("^Fragment_([0-9]+)(_[0-9]+)?$", RegexOptions.Compiled);
        private int GetNextFragmentIndex()
        {
            if (!BlockList.Blocks.Any()) { return 0; }
            return BlockList.Blocks.Select(b =>
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
                foreach (string o in BlockList.Output) { yield return o; }
                yield return ";END FRAGMENT CODE - Do not edit anything between this and the begin comment";
                foreach (string o in GlobalScope.Output) { yield return o; }
            }
        }
    }
}