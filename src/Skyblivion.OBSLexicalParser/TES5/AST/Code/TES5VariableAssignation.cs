using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5VariableAssignation : ITES5ValueCodeChunk
    {
        private ITES5Referencer reference;
        private ITES5Value value;
        public TES5VariableAssignation(ITES5Referencer reference, ITES5Value value)
        {
            this.reference = reference;
            this.value = value;
        }

        public IEnumerable<string> output()
        {
            string referenceOutput = this.reference.output().Single();
            string valueOutput = this.value.output().Single();
            string code = referenceOutput + " = "+ valueOutput;
            if (this.reference.getType() != this.value.getType() && !(this.value is TES5None))
            {
                code+= " as "+this.reference.getType().output().Single();
            }
            return new string[] { code };
        }

        public ITES5Referencer getReference()
        {
            return this.reference;
        }

        public ITES5Value getValue()
        {
            return this.value;
        }

        public ITES5Type getType()//WTM:  Change:  Added until a new proper interface is made.
        {
            throw new NotImplementedException();
        }
    }
}