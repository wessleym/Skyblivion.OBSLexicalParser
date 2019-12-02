using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class TranspileCommandBase<T> : ITranspileCommand where T : ITES4CodeFilterable
    {
        private readonly ParsingServiceWithCache<T> parserService;
        public TranspileCommandBase(ParsingServiceWithCache<T> parserService)
        {
            this.parserService = parserService;
        }

        protected T ParseOrGetFromCache(string sourcePath)
        {
            return parserService.ParseOrGetFromCache(sourcePath);
        }

        public abstract TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope);

        private static void GetFactories(MetadataLogService metadataLogService, ESMAnalyzer esmAnalyzer, out TES5ObjectCallFactory objectCallFactory, out TES5ChainedCodeChunkFactory chainedCodeChunkFactory, out TES5AdditionalBlockChangesPass additionalBlockChangesPass, out TES5StaticReferenceFactory staticReferenceFactory)
        {
            TES5InheritanceGraphAnalyzer inheritanceGraphAnalyzer = new TES5InheritanceGraphAnalyzer(esmAnalyzer);
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(esmAnalyzer, inheritanceGraphAnalyzer, BuildTarget.StandaloneSourcePath);
            TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
            objectCallFactory = new TES5ObjectCallFactory(inheritanceGraphAnalyzer, typeInferencer);
            TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory, esmAnalyzer);
            TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory);
            TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
            staticReferenceFactory = new TES5StaticReferenceFactory(esmAnalyzer);
            TES5ValueFactoryFunctionFiller.FillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, esmAnalyzer, metadataLogService, staticReferenceFactory);
            TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory, typeInferencer);
            TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory);
            chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory);
            additionalBlockChangesPass = new TES5AdditionalBlockChangesPass(objectCallFactory, referenceFactory);
        }

        protected static void GetFactories(MetadataLogService metadataLogService, ESMAnalyzer esmAnalyzer, out TES5FragmentFactory fragmentFactory)
        {
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory;
            GetFactories(metadataLogService, esmAnalyzer, out _, out chainedCodeChunkFactory, out _, out _);
            fragmentFactory = new TES5FragmentFactory(chainedCodeChunkFactory);
        }

        protected static void GetFactories(MetadataLogService metadataLogService, ESMAnalyzer esmAnalyzer, out TES5ObjectCallFactory objectCallFactory, out TES5BlockFactory blockFactory)
        {
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory;
            TES5AdditionalBlockChangesPass additionalBlockChangesPass;
            TES5StaticReferenceFactory staticReferenceFactory;
            GetFactories(metadataLogService, esmAnalyzer, out objectCallFactory, out chainedCodeChunkFactory, out additionalBlockChangesPass, out staticReferenceFactory);
            TES5InitialBlockCodeFactory initialBlockCodeFactory = new TES5InitialBlockCodeFactory(objectCallFactory);
            blockFactory = new TES5BlockFactory(chainedCodeChunkFactory, additionalBlockChangesPass, initialBlockCodeFactory, objectCallFactory, staticReferenceFactory);
        }
    }
}
