namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectProperty : ITES4ObjectAccess, ITES4Reference
    {
        private readonly TES4ApiToken parentReference;
        private readonly TES4ApiToken accessField;
        public TES4ObjectProperty(TES4ApiToken parentReference, TES4ApiToken accessField)
        {
            this.parentReference = parentReference;
            this.accessField = accessField;
        }

        public object Constant => StringValue;

        public string StringValue => this.parentReference.StringValue + "." + this.accessField.StringValue;
    }
}
