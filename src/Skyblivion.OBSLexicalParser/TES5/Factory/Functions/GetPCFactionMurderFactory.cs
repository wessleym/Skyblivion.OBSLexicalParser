namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPCFactionMurderFactory : GetPCFactionAttackFactory
    {
        public GetPCFactionMurderFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(referenceFactory, objectCallFactory)
        { }
    }
}