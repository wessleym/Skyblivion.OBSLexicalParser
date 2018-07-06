using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class TranspileCommand : TranspileCommandBase<TES4CodeChunks>
    {
        private readonly TES4ToTES5ASTQFFragmentConverter converter;
        public TranspileCommand(FragmentsParsingService fragmentsParsingService, Build build, MetadataLogService metadataLogService)
            : base(fragmentsParsingService)
        {
            ESMAnalyzer analyzer;
            TES5ReferenceFactory referenceFactory;
            TES5ValueFactory valueFactory;
            TES5FragmentFactory fragmentFactory;
            GetFactories(metadataLogService, out analyzer, out referenceFactory, out valueFactory, out fragmentFactory);
            converter = new TES4ToTES5ASTQFFragmentConverter(analyzer, fragmentFactory, valueFactory, referenceFactory);
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