using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    //WTM:  Change:  In PHP, this implemented ITES4CodeChunk.  I moved it to TES4VariableDeclaration.  I later added ITES4CodeFilterable here.
    class TES4VariableDeclarationList : ITES4CodeFilterable
    {
        private List<TES4VariableDeclaration> variableList = new List<TES4VariableDeclaration>();
        public void add(TES4VariableDeclaration declaration)
        {
            this.variableList.Add(declaration);
        }

        public List<TES4VariableDeclaration> getVariableList()
        {
            return this.variableList;
        }
        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return variableList.SelectMany(vl => vl.filter(predicate)).ToArray();
        }
    }
}