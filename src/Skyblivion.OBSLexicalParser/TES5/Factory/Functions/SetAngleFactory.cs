using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetAngleFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetAngleFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            string xyz = functionArguments[0].StringValue;
            ITES4ValueString argument1 = functionArguments[1];
            TES5ObjectCallArguments newArguments = new TES5ObjectCallArguments()
            {
                this.valueFactory.CreateValue(xyz == "x" ? argument1 : new TES4Integer(0), codeScope, globalScope, multipleScriptsScope),
                this.valueFactory.CreateValue(xyz == "y" ? argument1 : new TES4Integer(0), codeScope, globalScope, multipleScriptsScope),
                this.valueFactory.CreateValue(xyz == "z" ? argument1 : new TES4Integer(0), codeScope, globalScope, multipleScriptsScope)
            };
            return this.objectCallFactory.CreateObjectCall(calledOn, "SetAngle", newArguments, comment: function.Comment);
        }
    }
}