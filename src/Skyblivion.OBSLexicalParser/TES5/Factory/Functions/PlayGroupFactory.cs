using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PlayGroupFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public PlayGroupFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            const string functionName = "playGamebryoAnimation";
            //this function does not use strings for the names, so i cant really understand what is it.
            //todo refactor
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
            ITES4StringValue firstArg = functionArguments[0];
            convertedArguments.Add(new TES5String(firstArg.StringValue));
            /*
            secondArg = functionArguments.getValue(1);
    
        if (secondArg && secondArg.getData() != 0) {
                convertedArguments.add(new TES5Integer(1));
            }*/
            convertedArguments.Add(new TES5Bool(true));
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments);
        }
    }
}