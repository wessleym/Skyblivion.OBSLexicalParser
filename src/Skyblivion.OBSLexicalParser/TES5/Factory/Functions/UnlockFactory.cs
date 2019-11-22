using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class UnlockFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public UnlockFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments()
            {
                new TES5Bool(false)//override different behaviour
            };
            ITES4StringValue? lockAsOwner = functionArguments.GetOrNull(1);
            bool lockAsOwnerBool = lockAsOwner != null && (bool)lockAsOwner.Data;
            methodArguments.Add(new TES5Bool(lockAsOwnerBool));
            return this.objectCallFactory.CreateObjectCall(calledOn, "Lock", methodArguments);
        }
    }
}