using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    //WTM:  Change:  In PHP, this implemented ITES4CodeChunk.  I moved it to TES4VariableDeclaration.  I later added ITES4CodeFilterable here.
    class TES4VariableDeclarationList : ITES4CodeFilterable
    {
        public List<TES4VariableDeclaration> VariableList { get; }
        public TES4VariableDeclarationList()
        {
            VariableList = new List<TES4VariableDeclaration>();
        }

        public void Add(TES4VariableDeclaration declaration)
        {
            VariableList.Add(declaration);
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return VariableList.SelectMany(vl => vl.Filter(predicate)).ToArray();
        }
    }
}