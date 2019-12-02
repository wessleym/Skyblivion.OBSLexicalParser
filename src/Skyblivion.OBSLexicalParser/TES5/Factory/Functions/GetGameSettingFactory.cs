using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetGameSettingFactory : IFunctionFactory
    {
        public GetGameSettingFactory()
        { }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            string setting = functionArguments[0].StringValue;
            switch (setting.ToLower())
            {
                case "icrimegoldattackmin":
                case "icrimegoldattack":
                    {
                        return new TES5Integer(25);
                    }

                case "icrimegoldjailbreak":
                    {
                        return new TES5Integer(50);
                    }

                default:
                    {
                        throw new ConversionException("GetGameSetting() - unknown setting.");
                    }
            }
        }
    }
}