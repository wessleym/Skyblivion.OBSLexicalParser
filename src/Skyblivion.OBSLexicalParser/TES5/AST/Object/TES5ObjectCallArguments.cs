using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCallArguments : ITES5Outputtable, ITES5OutputtableSingle, IEnumerable<ITES5Value>
    {
        private readonly List<ITES5Value> arguments = new List<ITES5Value>();

        public string OutputSingle => string.Join(", ", this.arguments.Select(a => a.OutputSingle));

        public IEnumerable<string> Output => [OutputSingle];

        public void Add(ITES5Value value)
        {
            this.arguments.Add(value);
        }

        public void AddRange(IEnumerable<ITES5Value> values)
        {
            this.arguments.AddRange(values);
        }

        public void Insert(int index, ITES5Value value)
        {
            this.arguments.Insert(index, value);
        }

        public ITES5Value this[int index]
        {
            get
            {
                return arguments[index];
            }
            set
            {
                arguments[index] = value;
            }
        }

        public int Count => arguments.Count;

        public IEnumerator<ITES5Value> GetEnumerator()
        {
            return arguments.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}