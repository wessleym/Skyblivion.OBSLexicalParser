using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectCallArgumentsFactory
    {
        private readonly TES5ValueFactory valueFactory;
        public TES5ObjectCallArgumentsFactory(TES5ValueFactory factory)
        {
            this.valueFactory = factory;
        }

        public TES5ObjectCallArguments CreateArgumentList(TES4FunctionArguments arguments, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ObjectCallArguments list = new TES5ObjectCallArguments();
            if (arguments == null)
            {
                return list;
            }
            list.AddRange(arguments.Select(a => this.valueFactory.CreateValue(a, codeScope, globalScope, multipleScriptsScope)));
            return list;
        }
    }
}