using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class LockFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public LockFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            string functionName = function.FunctionCall.FunctionName;
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments()
            {
                new TES5Bool(true)//override different behaviour
            };
            ITES4StringValue? lockAsOwnerBool = functionArguments.GetOrNull(1);
            bool newLockBool = lockAsOwnerBool != null && (int)lockAsOwnerBool.Data== 1;
            methodArguments.Add(new TES5Bool(newLockBool));
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, methodArguments);
        }
    }
}