using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetPosFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetPosFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments callArguments = new TES5ObjectCallArguments();
            TES5ObjectCall dummyX = this.objectCallFactory.CreateObjectCall(calledOn, "GetPositionX");
            TES5ObjectCall dummyY = this.objectCallFactory.CreateObjectCall(calledOn, "GetPositionY");
            TES5ObjectCall dummyZ = this.objectCallFactory.CreateObjectCall(calledOn, "GetPositionZ");
            ITES5Value[] argList;
            switch (functionArguments[0].StringValue.ToLower())
            {
                case "x":
                    {
                        argList = new ITES5Value[]
                        {
                            this.valueFactory.CreateValue(functionArguments[1], codeScope, globalScope, multipleScriptsScope),
                            dummyY,
                            dummyZ
                        };
                        break;
                    }

                case "y":
                    {
                        argList = new ITES5Value[]
                        {
                            dummyX,
                            this.valueFactory.CreateValue(functionArguments[1], codeScope, globalScope, multipleScriptsScope),
                            dummyZ
                        };
                        break;
                    }

                case "z":
                    {
                        argList = new ITES5Value[]
                        {
                            dummyX,
                            dummyY,
                            this.valueFactory.CreateValue(functionArguments[1], codeScope, globalScope, multipleScriptsScope)
                        };
                        break;
                    }

                default:
                    {
                        throw new ConversionException("setPos can handle only X,Y,Z parameters.");
                    }
            }

            foreach (var argListC in argList)
            {
                callArguments.Add(argListC);
            }

            return this.objectCallFactory.CreateObjectCall(calledOn, "SetPosition", callArguments);
        }
    }
}