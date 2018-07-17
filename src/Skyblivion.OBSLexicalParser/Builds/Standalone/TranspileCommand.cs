using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class TranspileCommand : TranspileCommandBase<TES4Script>
    {
        private readonly TES4ToTES5ASTConverter converter;
        public TranspileCommand(StandaloneParsingService standaloneParsingService, Build build, MetadataLogService metadataLogService, bool loadESMAnalyzerLazily)
            : base(standaloneParsingService)
        {
            ESMAnalyzer analyzer;
            TES5BlockFactory blockFactory;
            TES5ObjectCallFactory objectCallFactory;
            TES5ReferenceFactory referenceFactory;
            GetFactories(metadataLogService, loadESMAnalyzerLazily, out analyzer, out objectCallFactory, out referenceFactory, out blockFactory);
            converter = new TES4ToTES5ASTConverter(analyzer, blockFactory, objectCallFactory, referenceFactory);
        }

        public override TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Script script = ParseOrGetFromCache(sourcePath);
            TES4Target tes4Target = new TES4Target(script, outputPath);
            TES5Target target = this.converter.Convert(tes4Target, globalScope, multipleScriptsScope);
            return target;
        }
    }
}