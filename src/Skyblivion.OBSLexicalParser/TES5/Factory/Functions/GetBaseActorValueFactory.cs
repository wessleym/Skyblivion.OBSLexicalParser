namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetBaseActorValueFactory : GetActorValueFactory
    {
        public GetBaseActorValueFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(referenceFactory, objectCallFactory)
        { }
    }
}