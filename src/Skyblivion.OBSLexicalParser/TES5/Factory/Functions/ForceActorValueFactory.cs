using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ForceActorValueFactory : SetOrForceActorValueFactory
    {
        public ForceActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
            : base("ForceAV", "ForceActorValue", valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, expressionFactory, assignationFactory, objectPropertyFactory, analyzer, primitiveValueFactory, typeInferencer, metadataLogService)
        { }
    }
}