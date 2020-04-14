using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetForceRunFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetForceRunFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Change:  This used to output an argument of 2 always.
            int forceRunFlag = ((TES4Integer)function.Arguments[0]).IntValue;
            bool run = forceRunFlag == 1;
            int afSpeedMult = !run ? 1 : 2;
            const string functionName = "ForceMovementSpeed";
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { new TES5Float(afSpeedMult) };
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments);
        }
    }
}