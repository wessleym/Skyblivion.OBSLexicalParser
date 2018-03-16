using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockParameterList : ITES4CodeFilterable
    {
        private List<TES4BlockParameter> variableList = new List<TES4BlockParameter>();
        public void add(TES4BlockParameter declaration)
        {
            this.variableList.Add(declaration);
        }

        public List<TES4BlockParameter> getVariableList()
        {
            return this.variableList;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.variableList.SelectMany(v => v.filter(predicate)).ToArray();
        }
    }
}