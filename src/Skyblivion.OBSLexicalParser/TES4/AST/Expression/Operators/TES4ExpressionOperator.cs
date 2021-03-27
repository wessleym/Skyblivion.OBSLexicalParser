namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4ExpressionOperator
    {
        public string Name { get; }
        protected TES4ExpressionOperator(string name)
        {
            Name = name;
        }
    }
}
