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
        public ITES5Value Reference { get; private set; }
        private readonly ITES5Value value;
        public TES5VariableAssignation(ITES5Value reference, ITES5Value value)
        {
            this.Reference = reference;
            this.value = value;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string referenceOutput = this.Reference.Output.Single();
                string valueOutput = this.value.Output.Single();
                string code = referenceOutput + " = " + valueOutput;
                if (this.Reference.TES5Type != this.value.TES5Type && !(this.value is TES5None))
                {
                    if (this.Reference.TES5Type == TES5BasicType.T_INT && TES5InheritanceGraphAnalyzer.IsExtending(this.value.TES5Type, TES5BasicType.T_FORM))
                    {//WTM:  Change:  Added
                        code += ".GetFormID()";
                    }
                    else
                    {
                        code += " as " + this.Reference.TES5Type.Output.Single();
                    }
                }
                yield return code;
            }
        }

        public ITES5Type TES5Type => throw new NotImplementedException();//WTM:  Change:  Added until a new proper interface is made.
    }
}