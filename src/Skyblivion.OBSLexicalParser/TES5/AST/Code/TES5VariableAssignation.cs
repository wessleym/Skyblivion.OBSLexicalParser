using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5VariableAssignation : ITES5CodeChunk
    {
        private ITES5Referencer reference;
        private ITES5Value value;
        public TES5VariableAssignation(ITES5Referencer reference, ITES5Value value)
        {
            this.reference = reference;
            this.value = value;
        }

        public List<string> output()
        {
            List<string> referenceOutputList = this.reference.output();
            string referenceOutputFirst = referenceOutputList[0];
            List<string> valueOutputList = this.value.output();
            string valueOutputFirst = valueOutputList[0];
            string code = referenceOutputFirst+" = "+valueOutputFirst;
            if (this.reference.getType() != this.value.getType() && !(this.value is TES5None))
            {
                code+= " as "+this.reference.getType().output();
            }

            List<string> codeLines = new List<string>() { code };
            return codeLines;
        }

        public ITES5Referencer getReference()
        {
            return this.reference;
        }

        public ITES5Value getValue()
        {
            return this.value;
        }
    }
}