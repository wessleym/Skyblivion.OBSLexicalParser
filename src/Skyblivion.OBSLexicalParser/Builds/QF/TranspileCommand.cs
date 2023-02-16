using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using System.Collections.Generic;

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

        public override TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4CodeChunks ast = ParseOrGetFromCache(sourcePath);
            TES4FragmentTarget fragmentTarget = new TES4FragmentTarget(ast, outputPath);
            TES5Target convertedScript = this.converter.Convert(fragmentTarget, globalScope, multipleScriptsScope);
            return convertedScript;
        }
    }
}