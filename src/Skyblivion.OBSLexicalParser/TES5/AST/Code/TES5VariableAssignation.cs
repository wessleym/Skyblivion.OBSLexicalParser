using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5VariableAssignation : ITES5CodeChunk, ITES5OutputtableSingle
    {
        public ITES5Value Reference { get; }
        public ITES5Value Value { get; }
        private readonly TES5Comment? comment;
        public TES5VariableAssignation(ITES5Value reference, ITES5Value value, TES5Comment? comment)
        {
            this.Reference = reference;
            this.Value = value;
            this.comment = comment;
            //If value is going to be casted but actually can't be casted, throw an exception.
            if (ReferenceAndValueDifferentTypesAndValueIsNonNone() && !ReferenceIsIntAndValueExtendsForm() &&
                //Oblivion stores boolean-like values in short ints.  Exclude such cases.
                !(reference.TES5Type == TES5BasicType.T_INT && value.TES5Type == TES5BasicType.T_BOOL) &&
                !TES5InheritanceGraphAnalyzer.IsTypeOrExtendsTypeOrIsNumberType(value.TES5Type, reference.TES5Type, true))
            {
                throw new ConversionTypeMismatchException(value.TES5Type.OriginalName + " : " + value.TES5Type.NativeType.Name + " cannot be assigned to " + reference.TES5Type.OriginalName + " : " + reference.TES5Type.NativeType.Name);
            }
        }

        private bool ReferenceIsIntAndValueExtendsForm()
        {
            return this.Reference.TES5Type == TES5BasicType.T_INT && TES5InheritanceGraphAnalyzer.IsExtending(this.Value.TES5Type, TES5BasicType.T_FORM);
        }

        private bool ReferenceIsFloatAndValueIsNumber()
        {
            return this.Reference.TES5Type == TES5BasicType.T_FLOAT && this.Value.TES5Type == TES5BasicType.T_INT;
        }

        private bool ReferenceAndValueDifferentTypesAndValueIsNonNone()
        {
            return !TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(this.Value.TES5Type, this.Reference.TES5Type) && !(this.Value is TES5None);
        }

        private string CommentString => comment != null ? " " + comment.OutputSingle : "";

        private bool RequiresCastOrGetFormID()
        {
            return ReferenceAndValueDifferentTypesAndValueIsNonNone() && !ReferenceIsFloatAndValueIsNumber();
        }

        public string EqualsSignValueOutput
        {
            get
            {
                string valueOutput = this.Value.OutputSingle;
                if (RequiresCastOrGetFormID())
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
                        if (Value is TES5ArithmeticExpression)
                        {
                            valueOutput = "(" + valueOutput + ")";
                        }
                        valueOutput += " as " + this.Reference.TES5Type.OutputSingle;
                    }
                }
                return " = " + valueOutput + CommentString;
            }
        }

        public string OutputSingle
        {
            get
            {
                if (!RequiresCastOrGetFormID())
                {
                    TES5ArithmeticExpression? valueArithmeticExpression = this.Value as TES5ArithmeticExpression;
                    if (valueArithmeticExpression != null && this.Reference.OutputSingle == valueArithmeticExpression.LeftValue.OutputSingle)
                    {//Addition assignment (+=) or subtraction assignment (-=)
                        return this.Reference.OutputSingle + " " + valueArithmeticExpression.Operator.Name + "= " + valueArithmeticExpression.RightValue.OutputSingle + CommentString;
                    }
                }
                return this.Reference.OutputSingle + EqualsSignValueOutput;
            }
        }

        public IEnumerable<string> Output => [OutputSingle];

        public ITES5Type TES5Type => throw new NotImplementedException();//WTM:  Change:  Added until a new proper interface is made.
    }
}