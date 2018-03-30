namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5ExpressionOperator
    {
        public string Name { get; private set; }
        public TES5ExpressionOperator(string name)
        {
            Name = name;
        }
    }
}
