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

        private string GetReturnType()
        {
            string returnTypeValue = this.returnType.Value;
            if (returnTypeValue != "")
            {
                return returnTypeValue + " "
#if PHP_COMPAT
                    (!isStandalone ? " " : "")
#endif
                    ;
            }
            else
            {
                return "";
            }
        }

        public override IEnumerable<string> Output
        {
            get
            {
                if (isQuestFragmentOrTopicFragment) { yield return ";BEGIN FRAGMENT " + this.FunctionScope.BlockName; }
                yield return GetReturnType() + "Function " + this.FunctionScope.BlockName + "(" + string.Join(", ", this.FunctionScope.GetParametersOutput()) + ")";
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