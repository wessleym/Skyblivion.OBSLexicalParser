﻿namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5ExpressionOperator
    {
        public string Name { get; }
        protected TES5ExpressionOperator(string name)
        {
            Name = name;
        }
    }
}
