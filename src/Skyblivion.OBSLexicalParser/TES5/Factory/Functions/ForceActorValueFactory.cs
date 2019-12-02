namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ForceActorValueFactory : SetOrForceActorValueFactory
    {
        public ForceActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base("ForceAV", "ForceActorValue", valueFactory, objectCallFactory, referenceFactory)
        { }
    }
}