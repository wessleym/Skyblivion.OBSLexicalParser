using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    static class TES4ToTES5ASTTIFFragmentConverterFactory
    {
        public static TES4ToTES5ASTTIFFragmentConverter GetConverter(Build build)
        {
            TypeMapper typeMapper = new TypeMapper();
            ESMAnalyzer analyzer = new ESMAnalyzer(typeMapper, "Oblivion.esm");
            TES5PrimitiveValueFactory primitiveValueFactory = new TES5PrimitiveValueFactory();
            MetadataLogService metadataLogService = new MetadataLogService(build);
            TES5BlockFunctionScopeFactory blockLocalScopeFactory = new TES5BlockFunctionScopeFactory();
            TES5CodeScopeFactory codeScopeFactory = new TES5CodeScopeFactory();
            TES5ExpressionFactory expressionFactory = new TES5ExpressionFactory();
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, "./BuildTargets/Standalone/Source/");
            TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
            TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
            TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
            TES5VariableAssignationFactory assignationFactory = new TES5VariableAssignationFactory(referenceFactory);
            TES5LocalVariableListFactory localVariableFactory = new TES5LocalVariableListFactory();
            TES5LocalScopeFactory localScopeFactory = new TES5LocalScopeFactory();
            TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, expressionFactory, assignationFactory, objectPropertyFactory, analyzer, primitiveValueFactory, typeInferencer, metadataLogService);
            TES5ValueFactoryFunctionFiller filler = new TES5ValueFactoryFunctionFiller();
            TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
            filler.fillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, expressionFactory, assignationFactory, objectPropertyFactory, analyzer, primitiveValueFactory, typeInferencer, metadataLogService);
            TES5BranchFactory branchFactory = new TES5BranchFactory(localScopeFactory, codeScopeFactory, valueFactory);
            TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory, assignationFactory, branchFactory, expressionFactory, typeInferencer);
            TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory, referenceFactory, blockLocalScopeFactory);
            TES5ChainedCodeChunkFactory chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory, branchFactory, localVariableFactory);
            return new TES4ToTES5ASTTIFFragmentConverter(analyzer, new TES5FragmentFactory(chainedCodeChunkFactory, new TES5FragmentFunctionScopeFactory(), codeScopeFactory, new TES5AdditionalBlockChangesPass(objectCallFactory, blockLocalScopeFactory, codeScopeFactory, expressionFactory, referenceFactory, branchFactory, assignationFactory, localScopeFactory), localScopeFactory), valueFactory, referenceFactory, new TES5PropertiesFactory(), new TES5NameTransformer());
        }
    }
}
