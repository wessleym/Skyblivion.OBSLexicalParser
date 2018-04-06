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

        public IEnumerable<string> Output
        {
            get
            {
                string referenceOutput = this.reference.Output.Single();
                string valueOutput = this.value.Output.Single();
                string code = referenceOutput + " = " + valueOutput;
                if (this.reference.TES5Type != this.value.TES5Type && !(this.value is TES5None))
                {
                    if (this.reference.TES5Type == TES5BasicType.T_INT && TES5InheritanceGraphAnalyzer.isExtending(this.value.TES5Type, TES5BasicType.T_FORM))
                    {//WTM:  Change:  Added
                        code += ".GetFormID()";
                    }
                    else
                    {
                        code += " as " + this.reference.TES5Type.Output.Single();
                    }
                }
                return new string[] { code };
            }
        }

        public ITES5Referencer getReference()
        {
            return this.reference;
        }

        public ITES5Type TES5Type => throw new NotImplementedException();//WTM:  Change:  Added until a new proper interface is made.
    }
}