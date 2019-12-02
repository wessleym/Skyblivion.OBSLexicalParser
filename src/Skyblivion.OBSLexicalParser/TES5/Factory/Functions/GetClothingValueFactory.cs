using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetClothingValueFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetClothingValueFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
             * @INCONSISTENCE
             * a) It returns the total value not the percentage "estimation"
             * b) Its only for the armor worn, not all eq - can be implemented though..
             */
            TES5ObjectCallArguments getArmorWornArg = new TES5ObjectCallArguments() { new TES5Integer(2) };
            return this.objectCallFactory.CreateObjectCall(this.objectCallFactory.CreateObjectCall(calledOn, "GetWornForm", getArmorWornArg), "GetGoldValue");
        }
    }
}