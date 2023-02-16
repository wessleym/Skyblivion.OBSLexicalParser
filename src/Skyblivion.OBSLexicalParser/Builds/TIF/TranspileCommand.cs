using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class TranspileCommand : TranspileCommandBase<TES4CodeChunks>
    {
        private readonly TES4ToTES5ASTTIFFragmentConverter converter;
        public TranspileCommand(FragmentsParsingService fragmentsParsingService, MetadataLogService metadataLogService, ESMAnalyzer esmAnalyzer)
            : base(fragmentsParsingService)
        {
            TES5FragmentFactory fragmentFactory;
            GetFactories(metadataLogService, esmAnalyzer, out fragmentFactory);
            converter = new TES4ToTES5ASTTIFFragmentConverter(fragmentFactory);
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