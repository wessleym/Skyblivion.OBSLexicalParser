namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPCFactionAttackFactory : IsInFactionAndGetCrimeGoldFactory
    {
        //WARNING: This is not an exact implementation
        //According to cs.elderscrolls.com, its about being in the faction AND having an attack on them ( violent crime )
        //It's similar but groups all violent wrongdoings ( including assaults, murders etc ).
        public GetPCFactionAttackFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, LogUnknownFunctionWithReplacementFactory logUnknownFunctionWithFalseFactory) : base(referenceFactory, objectCallFactory, true, logUnknownFunctionWithFalseFactory) { }
    }
}