using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
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
            //If value is going to be casted but actually can't be casted, throw an exception.
            if (ReferenceAndValueDifferentTypesAndValueIsNonNone() && !ReferenceIsIntAndValueExtendsForm() &&
                //Oblivion stores boolean-like values in short ints.  Exclude such cases.
                !(reference.TES5Type == TES5BasicType.T_INT && value.TES5Type==TES5BasicType.T_BOOL) &&
                !TES5InheritanceGraphAnalyzer.IsTypeOrExtendsTypeOrIsNumberType(value.TES5Type, reference.TES5Type, true))
            {
                throw new ConversionTypeMismatchException(value.TES5Type.OriginalName + " : " + value.TES5Type.NativeType.Name + " cannot be assigned to " + reference.TES5Type.OriginalName + " : " + reference.TES5Type.NativeType.Name);
            }
        }

        private bool ReferenceIsIntAndValueExtendsForm()
        {
            return this.Reference.TES5Type == TES5BasicType.T_INT && TES5InheritanceGraphAnalyzer.IsExtending(this.value.TES5Type, TES5BasicType.T_FORM);
        }

        private bool ReferenceIsFloatAndValueIsNumber()
        {
            return this.Reference.TES5Type == TES5BasicType.T_FLOAT && this.value.TES5Type == TES5BasicType.T_INT;
        }

        private bool ReferenceAndValueDifferentTypesAndValueIsNonNone()
        {
            return !TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(this.value.TES5Type, this.Reference.TES5Type) && !(this.value is TES5None);
        }

        public IEnumerable<string> Output
        {
            get
            {
                string referenceOutput = this.Reference.Output.Single();
                string valueOutput = this.value.Output.Single();
                if (ReferenceAndValueDifferentTypesAndValueIsNonNone() && !ReferenceIsFloatAndValueIsNumber())
                {
                    //WTM:  Change:  Added
                    //This is when the Oblivion code tried to save an actor to a property named "target" in two scripts (DANamiraSpell, DASanguineSpell).
                    //The property is written but never read.
                    if (ReferenceIsIntAndValueExtendsForm())
                    {
                        valueOutput += ".GetFormID()";
                    }
                    else
                    {
                        valueOutput += " as " + this.Reference.TES5Type.Output.Single();
                    }
                }
                yield return referenceOutput + " = " + valueOutput;
            }
        }

        public ITES5Type TES5Type => throw new NotImplementedException();//WTM:  Change:  Added until a new proper interface is made.
    }
}