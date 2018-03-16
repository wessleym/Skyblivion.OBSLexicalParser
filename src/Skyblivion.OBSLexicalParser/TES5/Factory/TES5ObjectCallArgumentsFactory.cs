using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectCallArgumentsFactory
    {
        private TES5ValueFactory valueFactory;
        public TES5ObjectCallArgumentsFactory(TES5ValueFactory factory)
        {
            this.valueFactory = factory;
        }

        public TES5ObjectCallArguments createArgumentList(TES4FunctionArguments arguments, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ObjectCallArguments list = new TES5ObjectCallArguments();
            if (arguments == null)
            {
                return list;
            }

            foreach (var argument in arguments.getValues())
            {
                ITES5Value newValue = this.valueFactory.createValue(argument, codeScope, globalScope, multipleScriptsScope);
                list.add(newValue);
            }

            return list;
        }
    }
}