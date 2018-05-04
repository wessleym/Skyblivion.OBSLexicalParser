namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PayFineFactory : PayFineOrPayFineThiefFactory
    {
        public PayFineFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(objectCallFactory, referenceFactory, false)
        { }
    }
}