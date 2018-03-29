using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectCallArguments : ITES5Outputtable
    {
        private List<ITES5Value> arguments = new List<ITES5Value>();
        public List<ITES5Value> getArguments()
        {
            return this.arguments;
        }

        public List<string> output()
        {
            return new List<string>() { string.Join(", ", this.arguments.Select(a=>a.output().Single())) };
        }

        public void add(ITES5Value value)
        {
            this.arguments.Add(value);
        }
    }
}