using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class RotateFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public RotateFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5LocalScope localScope = codeScope.LocalScope;
            int x = 0, y = 0, z = 0;
            int secondArgumentData = (int)functionArguments[1].Data;
            switch (functionArguments[0].StringValue.ToLower())
            {
                case "x":
                    {
                        x = secondArgumentData;
                        break;
                    }
                case "y":
                    {
                        y = secondArgumentData;
                        break;
                    }
                case "z":
                    {
                        z = secondArgumentData;
                        break;
                    }
            }

            TES5ObjectCallArguments rotateArguments = new TES5ObjectCallArguments()
            {
                calledOn,
                new TES5Integer(x),
                new TES5Integer(y),
                new TES5Integer(z)
            };
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope), "Rotate", rotateArguments);
            return newFunction;
        }
    }
}