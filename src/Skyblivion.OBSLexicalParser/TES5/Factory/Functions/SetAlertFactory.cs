using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetAlertFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SetAlertFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES4ValueString arg0Value;
            TES4FunctionArguments revisedArguments;
            function.Arguments.GetFirstAndRemoveInNew(out arg0Value, out revisedArguments);
            int arg0 = (int)arg0Value.Constant;
            string functionName;
            switch (arg0)
            {
                case 0:
                    {
                        functionName = "SheatheWeapon";
                        break;
                    }
                case 1:
                    {
                        functionName = "DrawWeapon";
                        break;
                    }
                default:
                    {
                        throw new ConversionException("Unknown setAlert value, must be 0 or 1");
                    }
            }
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(revisedArguments, codeScope, globalScope, multipleScriptsScope);
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, newArguments, comment: function.Comment);
        }
    }
}