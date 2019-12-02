using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCallCustom : TES5ObjectCall
    {
        private readonly ITES5Type returnType;
        public TES5ObjectCallCustom(ITES5Referencer called, string functionName, ITES5Type returnType, TES5ObjectCallArguments? arguments, TES5InheritanceGraphAnalyzer inheritanceGraphAnalyzer)
            : base(called, functionName, arguments, inheritanceGraphAnalyzer)
        {
            this.returnType = returnType;
        }

        public override ITES5Type TES5Type => returnType;
    }
}
