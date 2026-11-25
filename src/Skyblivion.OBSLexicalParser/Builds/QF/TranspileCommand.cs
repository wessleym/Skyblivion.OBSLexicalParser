using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class TranspileCommand : TranspileCommandBase<TES4CodeChunks>
    {
        private readonly TES4ToTES5ASTQFFragmentConverter converter;
        public TranspileCommand(FragmentsParsingService fragmentsParsingService, TES5FragmentFactory fragmentFactory)
            : base(fragmentsParsingService)
        {
            converter = new TES4ToTES5ASTQFFragmentConverter(fragmentFactory);
        }

        public override TES5Script Transpile(string sourcePath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4CodeChunks ast = ParseOrGetFromCache(sourcePath);
            TES5Script convertedScript = this.converter.Convert(ast, globalScope, multipleScriptsScope);
            return convertedScript;
        }
    }
}