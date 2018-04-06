using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCallArguments : ITES5Outputtable, IEnumerable<ITES5Value>
    {
        private List<ITES5Value> arguments = new List<ITES5Value>();
        public IEnumerable<string> Output => new string[] { string.Join(", ", this.arguments.Select(a => a.Output.Single())) };

        public void Add(ITES5Value value)
        {
            this.arguments.Add(value);
        }

        public void AddRange(IEnumerable<ITES5Value> values)
        {
            this.arguments.AddRange(values);
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