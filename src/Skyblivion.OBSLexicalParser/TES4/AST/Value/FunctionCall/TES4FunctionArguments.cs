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

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        public void GetFirstAndRemoveInNew(out ITES4ValueString value, out TES4FunctionArguments revisedArguments)
        {
            const int index = 0;
            revisedArguments = new TES4FunctionArguments();
            revisedArguments.values.AddRange(values);
            revisedArguments.RemoveAt(index);
            value = values[index];
        }

        public ITES4ValueString? GetOrNull(int i)
        {
            return i < values.Count ? values[i] : null;
        }
    }
}