namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPCFactionStealFactory : IsInFactionAndGetCrimeGoldFactory
    {
        //WARNING: This is not an exact implementation
        //According to cs.elderscrolls.com, its about being in the faction AND having an attack on them ( violent crime )
        //It's similar but groups all nonviolent wrongdoings
        public GetPCFactionStealFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory) : base(referenceFactory, objectCallFactory, false) { }
    }
}