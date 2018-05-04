namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PayFineThiefFactory : PayFineOrPayFineThiefFactory
    {
        public PayFineThiefFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(objectCallFactory, referenceFactory, true)
        { }
    }
}
