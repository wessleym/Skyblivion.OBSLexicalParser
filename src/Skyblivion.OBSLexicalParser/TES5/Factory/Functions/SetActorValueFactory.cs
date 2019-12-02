namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetActorValueFactory : SetOrForceActorValueFactory
    {
        public SetActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base("SetAV", "SetActorValue", valueFactory, objectCallFactory, referenceFactory)
        { }
    }
}