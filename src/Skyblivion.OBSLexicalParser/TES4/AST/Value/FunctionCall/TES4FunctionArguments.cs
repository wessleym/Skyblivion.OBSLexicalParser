using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionArguments : IEnumerable<ITES4StringValue>
    {
        private List<ITES4StringValue> values = new List<ITES4StringValue>();

        public override int GetHashCode()
        {
            return values.GetHashCode();
        }

        public IEnumerator<ITES4StringValue> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITES4StringValue declaration)
        {
            this.values.Add(declaration);
        }

        public int Count => this.values.Count;

        public ITES4StringValue this[int index] => values[index];

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        public ITES4StringValue Pop(int index)
        {
            ITES4StringValue toReturn = values[index];
            RemoveAt(index);
            return toReturn;
        }

        public ITES4StringValue GetOrNull(int i)
        {
            try
            {
                return values[i];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.SelectMany(v => v.Filter(predicate)).ToArray();
        }
    }
}