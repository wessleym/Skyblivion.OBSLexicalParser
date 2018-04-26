using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class TranspileCommand : ITranspileCommand
    {
        private FragmentsParsingService parsingService;
        private TES4ToTES5ASTQFFragmentConverter converter;//WTM:  Note:  converter was of type TES4ToTES5ASTTIFFragmentConverter.
        public TranspileCommand(FragmentsParsingService fragmentsParsingService)
        {
            this.parsingService = fragmentsParsingService;
        }

        public void initialize(Build build, MetadataLogService metadataLogService)
        {
            ESMAnalyzer analyzer = new ESMAnalyzer(DataDirectory.TES4GameFileName);
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, BuildTarget.StandaloneSourcePath);
            TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
            TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
            TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
            TES5VariableAssignationFactory assignationFactory = new TES5VariableAssignationFactory(referenceFactory);
            TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, assignationFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService);
            TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
            TES5ValueFactoryFunctionFiller.fillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, assignationFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService);
            TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory, assignationFactory, typeInferencer);
            TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory, referenceFactory);
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory);
            converter = new TES4ToTES5ASTQFFragmentConverter(analyzer, new TES5FragmentFactory(chainedCodeChunkFactory, new TES5AdditionalBlockChangesPass(objectCallFactory, referenceFactory, assignationFactory)), valueFactory, referenceFactory);
        }

        public TES5Target transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4CodeChunks AST = this.parsingService.parseScript(sourcePath);
            TES5Target convertedScript = this.converter.convert(new TES4FragmentTarget(AST, outputPath), globalScope, multipleScriptsScope);
            return convertedScript;
        }
    }
}