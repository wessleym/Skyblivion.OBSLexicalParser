using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockParameterList : ITES4CodeFilterable
    {
        public List<TES4BlockParameter> Parameters { get; } = new List<TES4BlockParameter>();
        public void Add(TES4BlockParameter parameter)
        {
            this.Parameters.Add(parameter);
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.Parameters.SelectMany(v => v.Filter(predicate)).ToArray();
        }
    }
}