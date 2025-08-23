namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetActorValueFactory : GetActorValueOrBaseActorValueFactory
    {
        public GetActorValueFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
            : base(referenceFactory, objectCallFactory, "GetActorValue") { }
    }
}