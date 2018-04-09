using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetInChargenFactory : IFunctionFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        public SetInChargenFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Referencer newCalledOn = TES5StaticReference.Game;
            const string functionName = "SetInChargen";
            bool argumentBool = ((int)function.getArguments()[0].getData()) == 1;
            ITES5Value argumentValue = new TES5Bool(argumentBool);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            arguments.Add(argumentValue);
            arguments.Add(argumentValue);
            arguments.Add(argumentValue);
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, multipleScriptsScope, arguments);
            return newFunction;
        }
    }
}
