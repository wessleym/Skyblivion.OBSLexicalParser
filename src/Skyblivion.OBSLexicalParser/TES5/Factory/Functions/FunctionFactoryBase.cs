using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    abstract class FunctionFactoryBase : IFunctionFactory
    {
        protected string TES4FunctionName { get; private set; }
        protected string TES5FunctionName { get; private set; }
        private readonly TES5ObjectCallFactory objectCallFactory;
        public FunctionFactoryBase(string tes4FunctionName, string tes5FunctionName, TES5ObjectCallFactory objectCallFactory)
        {
            TES4FunctionName = tes4FunctionName;
            TES5FunctionName = tes5FunctionName;
            this.objectCallFactory = objectCallFactory;
        }

        protected TES5ObjectCall CreateObjectCall(ITES5Referencer calledOn, string functionName, TES5ObjectCallArguments arguments, TES4Comment? comment)
        {
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, arguments, comment: comment);
        }

        public abstract ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope);
    }
}
