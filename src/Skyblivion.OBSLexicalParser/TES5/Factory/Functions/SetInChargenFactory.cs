using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
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
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetInChargenFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Referencer newCalledOn = TES5StaticReferenceFactory.Game;
            const string functionName = "SetInChargen";
            bool argumentBool = ((TES4Integer)function.Arguments[0]).IntValue == 1;
            ITES5Value argumentValue = new TES5Bool(argumentBool);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { argumentValue, argumentValue, argumentValue };
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, arguments);
            return newFunction;
        }
    }
}
