using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;

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
            List<string> outputs = new List<string>();
            foreach (var argument in this.arguments)
            {
                List<string> subOutput = argument.output();
                string subOutputFirst = subOutput[0];
                outputs.Add(subOutputFirst);
            }

            return new List<string>() { string.Join(", ", outputs) };
        }

        public void add(ITES5Value value)
        {
            this.arguments.Add(value);
        }
    }
}