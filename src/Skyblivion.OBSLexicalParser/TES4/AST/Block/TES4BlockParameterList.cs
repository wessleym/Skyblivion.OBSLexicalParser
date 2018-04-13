using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockParameterList : ITES4CodeFilterable
    {
        public List<TES4BlockParameter> VariableList { get; private set; } = new List<TES4BlockParameter>();
        public void add(TES4BlockParameter declaration)
        {
            this.VariableList.Add(declaration);
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.VariableList.SelectMany(v => v.Filter(predicate)).ToArray();
        }
    }
}