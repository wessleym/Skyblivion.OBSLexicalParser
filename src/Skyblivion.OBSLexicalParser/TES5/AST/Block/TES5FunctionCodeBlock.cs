using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5FunctionCodeBlock : TES5CodeBlock
    {
        public override TES5CodeScope CodeScope { get; set; }
        public override TES5FunctionScope FunctionScope { get; protected set; }
        private readonly ITES5Type returnType;
        private readonly bool isStandalone;//Only needed for PHP_COMPAT
        private readonly bool isQuestFragmentOrTopicFragment;
        public TES5FunctionCodeBlock(TES5FunctionScope functionScope, TES5CodeScope codeScope, ITES5Type returnType, bool isStandalone, bool isQuestFragmentOrTopicFragment)
        {
            this.FunctionScope = functionScope;
            this.CodeScope = codeScope;
            this.returnType = returnType;
            this.isStandalone = isStandalone;
            this.isQuestFragmentOrTopicFragment = isQuestFragmentOrTopicFragment;
        }

        public override IEnumerable<string> Output
        {
            get
            {
                string returnTypeValue = this.returnType.Value;
                string functionReturnType;
                if (returnTypeValue != "")
                {
                    functionReturnType = returnTypeValue + " ";
#if PHP_COMPAT
                    if (!isStandalone) { functionReturnType += " "; }
#endif
                }
                else
                {
                    functionReturnType = "";
                }
                if (isQuestFragmentOrTopicFragment) { yield return ";BEGIN FRAGMENT " + this.FunctionScope.BlockName; }
                yield return functionReturnType + "Function " + this.FunctionScope.BlockName + "(" + string.Join(", ", this.FunctionScope.GetVariablesOutput()) + ")";
                if (isQuestFragmentOrTopicFragment) { yield return ";BEGIN CODE"; }
                foreach (string o in this.CodeScope.Output) { yield return TES5Script.Indent + o; }
                if (isQuestFragmentOrTopicFragment) { yield return ";END CODE"; }
                yield return "EndFunction";
                if (isQuestFragmentOrTopicFragment)
                {
                    yield return ";END FRAGMENT";
                    yield return "";
                }
            }
        }

        public override void AddChunk(ITES5CodeChunk chunk)
        {
            this.CodeScope.AddChunk(chunk);
        }
    }
}