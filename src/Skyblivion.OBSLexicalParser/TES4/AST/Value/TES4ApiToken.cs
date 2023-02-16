namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    class TES4ApiToken : ITES4Reference
    {
        private readonly string token;
        public TES4ApiToken(string token)
        {
            this.token = token;
        }

        public object Constant => StringValue;

        public string StringValue => token;
    }
}