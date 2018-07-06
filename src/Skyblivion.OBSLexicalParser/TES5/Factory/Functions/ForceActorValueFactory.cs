using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ForceActorValueFactory : SetOrForceActorValueFactory
    {
        public ForceActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
            : base("ForceAV", "ForceActorValue", valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, analyzer, typeInferencer, metadataLogService)
        { }
    }
}