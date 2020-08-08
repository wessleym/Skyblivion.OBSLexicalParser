using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class PopCalledRenameFunctionFactory
     * Pops the first argument to calledOn and renames the function
     */
    class PopCalledRenameFunctionFactory : IFunctionFactory
    {
        private readonly string newFunctionName;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public PopCalledRenameFunctionFactory(string newFunctionName, TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.newFunctionName = newFunctionName;
            this.referenceFactory = referenceFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(functionArguments.Pop(0).StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            /*if (this.newFunctionName.Equals("SetStage", System.StringComparison.OrdinalIgnoreCase))//Useful for logging when stages get set
            {
                return new TES5CodeChunkCollection()
                {
                    this.objectCallFactory.CreateObjectCall(newCalledOn, this.newFunctionName, newArguments),
                    this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "MessageBox", new TES5ObjectCallArguments() { new TES5String("Setting "+newCalledOn.Name+" -> " + string.Join(", ", newArguments.Select(a=>a.Output.Single().Replace("\"", "q")))) })
                };
            }*/
            return this.objectCallFactory.CreateObjectCall(newCalledOn, this.newFunctionName, newArguments);
        }
    }
}