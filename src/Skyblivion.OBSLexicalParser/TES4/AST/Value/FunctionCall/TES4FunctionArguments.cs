using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionArguments : IEnumerable<ITES4ValueString>
    {
        private readonly List<ITES4ValueString> values = new List<ITES4ValueString>();

        public IEnumerator<ITES4ValueString> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITES4ValueString declaration)
        {
            this.values.Add(declaration);
        }

        public int Count => this.values.Count;

        public ITES4ValueString this[int index] => values[index];

        public ITES4ValueString? GetOrNull(int i)
        {
            return i < values.Count ? values[i] : null;
        }

        public TES4FunctionArguments RemoveInNew(int index)
        {
            TES4FunctionArguments revised = new TES4FunctionArguments();
            revised.values.AddRange(values);
            revised.values.RemoveAt(index);
            return revised;
        }

        public (ITES4ValueString Value, TES4FunctionArguments RevisedArguments) GetFirstAndRemoveInNew()
        {
            const int index = 0;
            ITES4ValueString value = values[index];
            TES4FunctionArguments revised = RemoveInNew(index);
            return (value, revised);
        }
    }
}