using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class TranspileCommandBase<T> : ITranspileCommand where T : ITES4CodeFilterable
    {
        private ParsingServiceWithCache<T> parserService;
        public TranspileCommandBase(ParsingServiceWithCache<T> parserService)
        {
            this.parserService = parserService;
        }

        protected T ParseOrGetFromCache(string sourcePath)
        {
            return parserService.ParseOrGetFromCache(sourcePath);
        }

        public abstract TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope);

        private static void GetFactories(MetadataLogService metadataLogService, bool loadESMAnalyzerLazily, out ESMAnalyzer analyzer, out TES5ObjectCallFactory objectCallFactory, out TES5ReferenceFactory referenceFactory, out TES5ValueFactory valueFactory, out TES5ChainedCodeChunkFactory chainedCodeChunkFactory, out TES5AdditionalBlockChangesPass additionalBlockChangesPass)
        {
            analyzer = new ESMAnalyzer(loadESMAnalyzerLazily, DataDirectory.TES4GameFileName);
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, BuildTarget.StandaloneSourcePath);
            TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
            objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
            referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
            valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService);
            TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
            TES5ValueFactoryFunctionFiller.FillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService);
            TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory, typeInferencer);
            TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory);
            chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory);
            additionalBlockChangesPass = new TES5AdditionalBlockChangesPass(objectCallFactory, referenceFactory);
        }

        protected static void GetFactories(MetadataLogService metadataLogService, bool loadESMAnalyzerLazily, out ESMAnalyzer analyzer, out TES5ReferenceFactory referenceFactory, out TES5ValueFactory valueFactory, out TES5FragmentFactory fragmentFactory)
        {
            TES5ObjectCallFactory objectCallFactory;
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory;
            TES5AdditionalBlockChangesPass additionalBlockChangesPass;
            GetFactories(metadataLogService, loadESMAnalyzerLazily, out analyzer, out objectCallFactory, out referenceFactory, out valueFactory, out chainedCodeChunkFactory, out additionalBlockChangesPass);
            fragmentFactory = new TES5FragmentFactory(chainedCodeChunkFactory, additionalBlockChangesPass);
        }

        protected static void GetFactories(MetadataLogService metadataLogService, bool loadESMAnalyzerLazily, out ESMAnalyzer analyzer, out TES5ObjectCallFactory objectCallFactory, out TES5ReferenceFactory referenceFactory, out TES5BlockFactory blockFactory)
        {
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory;
            TES5AdditionalBlockChangesPass additionalBlockChangesPass;
            GetFactories(metadataLogService, loadESMAnalyzerLazily, out analyzer, out objectCallFactory, out referenceFactory, out _, out chainedCodeChunkFactory, out additionalBlockChangesPass);
            TES5InitialBlockCodeFactory initialBlockCodeFactory = new TES5InitialBlockCodeFactory(referenceFactory, objectCallFactory);
            blockFactory = new TES5BlockFactory(chainedCodeChunkFactory, additionalBlockChangesPass, initialBlockCodeFactory, objectCallFactory);
        }
    }
}
