using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value
{
    class TES5ConcatenatedValue : ITES5Value
    {
        private IList<ITES5Value> concatenatedValues;
        public TES5ConcatenatedValue(IList<ITES5Value> concatenatedValues)
        {
            this.concatenatedValues = concatenatedValues;
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_STRING; //concatenated value is always a string ( what else.. :) )
        }

        public List<string> output()
        {
            List<string> outputs = new List<string>();
            foreach (var concatValue in this.concatenatedValues)
            {
                outputs.Add(string.Join("", concatValue.output()));
            }
            return new List<string>() { string.Join(" + ", outputs) };
        }
    }
}