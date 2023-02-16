using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetAngleFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetAngleFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES4ValueString arg0Value;
            TES4FunctionArguments revisedArguments;
            function.Arguments.GetFirstAndRemoveInNew(out arg0Value, out revisedArguments);
            string arg0Upper = arg0Value.StringValue.ToUpper();
            if (arg0Upper != "X" && arg0Upper != "Y" && arg0Upper != "Z") { throw new ConversionException("getAngle can handle only X,Y,Z parameters."); }
            string functionName = "GetAngle" + arg0Upper;
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(revisedArguments, codeScope, globalScope, multipleScriptsScope);
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, newArguments, comment: function.Comment);
        }
    }
}