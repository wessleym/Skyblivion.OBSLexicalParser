using Skyblivion.OBSLexicalParser.TES4.AST.Expression;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ValueFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly Dictionary<string, IFunctionFactory> functionFactories = new Dictionary<string, IFunctionFactory>();
        public TES5ValueFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }
        public void AddFunctionFactory(string functionName, IFunctionFactory factory)
        {
            string key = functionName.ToLower();
            this.functionFactories.Add(key, factory);
        }
        private ITES5Value ConvertComparisonExpression(ITES4BinaryExpression expression, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            Tuple<ITES4Value, ITES4Value>[] sets = new Tuple<ITES4Value, ITES4Value>[]
            {
                new Tuple<ITES4Value, ITES4Value>(expression.LeftValue, expression.RightValue),
                new Tuple<ITES4Value, ITES4Value>(expression.RightValue, expression.LeftValue)
            };

            /*
             * Scenario 1 - Special functions converted on expression level
             */
            foreach (Tuple<ITES4Value, ITES4Value> set in sets)
            {
                ITES4Callable? setItem1Callable = set.Item1 as ITES4Callable;
                if (setItem1Callable == null) { continue; }

                TES4Function function = setItem1Callable.Function;

                switch (function.FunctionCall.FunctionName.ToLower())
                {
                    case "getweaponanimtype":
                        {
                            ITES5Referencer calledOn = this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope);
                            TES5ObjectCall equippedWeaponLeftValue = this.objectCallFactory.CreateObjectCall(this.objectCallFactory.CreateObjectCall(calledOn, "GetEquippedWeapon"), "GetWeaponType");

                            int[] targetedWeaponTypes;
                            switch ((int)set.Item2.Data)
                            {

                                case 0:
                                    {
                                        targetedWeaponTypes = new int[] { 0 };
                                        break;
                                    }
                                case 1:
                                    {
                                        targetedWeaponTypes = new int[] { 1, 2, 3, 4 };
                                        break;
                                    }
                                case 2:
                                    {
                                        targetedWeaponTypes = new int[] { 5, 6, 8 };
                                        break;
                                    }
                                case 3:
                                    {
                                        targetedWeaponTypes = new int[] { 7, 9 };
                                        break;
                                    }
                                default:
                                    {
                                        throw new ConversionException("GetWeaponAnimType() - Unknown weapon type in expression");
                                    }

                            }

                            List<TES5ComparisonExpression> expressions = new List<TES5ComparisonExpression>();

                            foreach (var targetedWeaponType in targetedWeaponTypes)
                            {
                                expressions.Add(TES5ExpressionFactory.CreateComparisonExpression(equippedWeaponLeftValue, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(targetedWeaponType)));
                            }

                            ITES5Expression resultExpression = expressions[0];

                            expressions.RemoveAt(0);

                            while (expressions.Any())
                            {
                                resultExpression = TES5ExpressionFactory.CreateLogicalExpression(resultExpression, TES5LogicalExpressionOperator.OPERATOR_OR, expressions.Last());
                                expressions.RemoveAt(expressions.Count - 1);
                            }

                            return resultExpression;
                        }

                    case "getdetected":
                        {
                            TES5ObjectCallArguments inversedArgument = new TES5ObjectCallArguments()
                            {
                                this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope)
                            };
                            TES5ObjectCall getDetectedLeftValue = this.objectCallFactory.CreateObjectCall(this.referenceFactory.CreateReadReference(function.Arguments[0].StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope), "isDetectedBy", inversedArgument);
                            TES5Integer getDetectedRightValue = new TES5Integer(((int)set.Item2.Data== 0) ? 0 : 1);
                            return TES5ExpressionFactory.CreateComparisonExpression(getDetectedLeftValue, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, getDetectedRightValue);
                        }

                    case "getdetectionlevel":
                        {

                            if (!set.Item2.HasFixedValue)
                            {
                                throw new ConversionException("Cannot convert getDetectionLevel calls with dynamic comparision");
                            }

                            TES5Bool tes5Bool = new TES5Bool(((int)set.Item2.Data) == 3); //true only if the compared value was 3

                            TES5ObjectCallArguments inversedArgument = new TES5ObjectCallArguments()
                            {
                                this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope)
                            };

                            return TES5ExpressionFactory.CreateComparisonExpression
                                (
                                    this.objectCallFactory.CreateObjectCall(this.referenceFactory.CreateReadReference(function.Arguments[0].StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope), "isDetectedBy", inversedArgument),
                                    TES5ComparisonExpressionOperator.OPERATOR_EQUAL,
                                    tes5Bool
                                );
                        }

                    case "getcurrentaiprocedure":
                        {

                            if (!set.Item2.HasFixedValue)
                            {
                                throw new ConversionException("Cannot convert getCurrentAIProcedure() calls with dynamic comparision");
                            }

                            switch ((int)set.Item2.Data)
                            {
                                case 4:
                                    {
                                        return TES5ExpressionFactory.CreateComparisonExpression(
                                            this.objectCallFactory.CreateObjectCall(this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "IsInDialogueWithPlayer"),
                                            TES5ComparisonExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Bool(expression.Operator == TES4ComparisonExpressionOperator.OPERATOR_EQUAL) //cast to true if the original op was ==, false otherwise.
                                        );
                                    }

                                case 8:
                                    {

                                        //ref.getSleepState() == 3
                                        return TES5ExpressionFactory.CreateComparisonExpression(
                                            this.objectCallFactory.CreateObjectCall(this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "getSleepState"),
                                            TES5ComparisonExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Integer(3) //SLEEPSTATE_SLEEP
                                        );
                                    }
                                case 13:
                                    {
                                        return TES5ExpressionFactory.CreateComparisonExpression(
                                            this.objectCallFactory.CreateObjectCall(this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "IsInCombat"),
                                            TES5ComparisonExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Bool(expression.Operator== TES4ComparisonExpressionOperator.OPERATOR_EQUAL) //cast to true if the original op was ==, false otherwise.
                                        );
                                    }

                                case 0://Travel
                                case 7://Wander
                                case 15://Pursue
                                case 17://Done
                                    {
                                        //@INCONSISTENCE idk how to check it tbh. We return always true. Think about better representation
                                        return new TES5Bool(expression.Operator == TES4ComparisonExpressionOperator.OPERATOR_EQUAL);
                                    }
                                default:
                                    {
                                        throw new ConversionException("Cannot convert GetCurrentAiProcedure - unknown TES4 procedure number arg " + ((int)set.Item2.Data).ToString());
                                    }
                            }
                        }

                    case "isidleplaying":
                    case "getknockedstate":
                    case "gettalkedtopc":
                        {
                            return new TES5Bool(true); //This is so unimportant that i think it"s not worth to find a good alternative and waste time.
                        }

                    case "getsitting":
                        {
                            //WARNING: Needs to implement Horse sittings, too.
                            //SEE: http://www.gameskyrim.com/papyrus-isridinghorse-function-t255012.html
                            int goTo;
                            switch ((int)set.Item2.Data)
                            {
                                case 0:
                                    {
                                        goTo = 0;
                                        break;
                                    }

                                case 1:
                                case 2:
                                case 11:
                                case 12:
                                    {
                                        goTo = 2;
                                        break;
                                    }

                                case 3:
                                case 13:
                                    {
                                        goTo = 3;
                                        break;
                                    }

                                case 4:
                                case 14:
                                    {
                                        goTo = 4;
                                        break;
                                    }

                                default:
                                    {
                                        throw new ConversionException("GetSitting - unknown state found");
                                    }
                            }

                            //ref.getSleepState() == 3
                            return TES5ExpressionFactory.CreateComparisonExpression
                            (
                                this.objectCallFactory.CreateObjectCall(this.CreateCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "GetSitState"),
                                TES5ComparisonExpressionOperator.GetFirst(expression.Operator.Name),
                                new TES5Integer(goTo)
                            );

                        }

                }

            }

            ITES5Value leftValue = this.CreateValue(expression.LeftValue, codeScope, globalScope, multipleScriptsScope);
            ITES5Value rightValue = this.CreateValue(expression.RightValue, codeScope, globalScope, multipleScriptsScope);

            Tuple<ITES5Value, ITES5Value>[] tes5sets = new Tuple<ITES5Value, ITES5Value>[]
            {
                new Tuple<ITES5Value, ITES5Value>(leftValue, rightValue),
                new Tuple<ITES5Value, ITES5Value>(rightValue, leftValue)
            };

            TES5BasicType objectReferenceType = TES5BasicType.T_FORM; //used just to make sure.
            TES5ComparisonExpressionOperator op = TES5ComparisonExpressionOperator.GetFirstOrNull(expression.Operator.Name);

            /*
             * Scenario 2: Comparision of ObjectReferences to integers ( quick formid check )
             */
            bool flipOperator = false;
            foreach (var tes5set in tes5sets)
            {
                TES5ComparisonExpressionOperator newOp = !flipOperator ? op : op.Flip();
                if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(tes5set.Item1.TES5Type, objectReferenceType) || TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(tes5set.Item1.TES5Type.NativeType, objectReferenceType))
                {
                    if (tes5set.Item2.TES5Type == TES5BasicType.T_INT)
                    {
                        //Perhaps we should allow to try to cast upwards for primitives, .asPrimitive() or similar
                        //In case we do know at compile time that we"re comparing against zero, then we can assume
                        //we can compare against None, which allows us not call GetFormID() on most probably None object
                        TES5Integer tes5SetItem2Integer = (TES5Integer)tes5set.Item2;
                        if (tes5SetItem2Integer.IntValue == 0)
                        {
                            newOp = op == TES5ComparisonExpressionOperator.OPERATOR_EQUAL ? op : TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL;
                            return TES5ExpressionFactory.CreateComparisonExpression(tes5set.Item1, newOp, new TES5None());

                        }
                        else
                        {
                            ITES5Referencer callable = (ITES5Referencer)tes5set.Item1;
                            TES5ObjectCall tes5setNewItem1 = this.objectCallFactory.CreateObjectCall(callable, "GetFormID");
                            return TES5ExpressionFactory.CreateComparisonExpression(tes5setNewItem1, newOp, tes5set.Item2);
                        }
                    }
                }
                else if (tes5set.Item1.TES5Type.OriginalName == TES5VoidType.OriginalNameConst)
                {
#if PHP_COMPAT
                    TES5IntegerOrFloat tes5SetItem2Number = tes5set.Item2 as TES5IntegerOrFloat;
                    if (tes5SetItem2Number != null && tes5SetItem2Number.ConvertedIntValue == 0)
                    {
                        return TES5ExpressionFactory.createArithmeticExpression(tes5set.Item1, newOp, new TES5None());
                    }
#else
                    throw new ConversionException("Type was void.");//This shouldn't happen anymore.
#endif
                }
                if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsTypeOrIsNumberType(tes5set.Item1.TES5Type, tes5set.Item2.TES5Type))
                {//WTM:  Change:  Added entire if branch
                    if (tes5set.Item1.TES5Type.NativeType== TES5BasicType.T_QUEST && tes5set.Item2.TES5Type == TES5BasicType.T_INT)
                    {
                        TES5ObjectCall getStage = this.objectCallFactory.CreateObjectCall((ITES5Referencer)tes5set.Item1, "GetStage");
                        return TES5ExpressionFactory.CreateComparisonExpression(getStage, newOp, tes5set.Item2);
                    }
                    if (tes5set.Item1.TES5Type == TES5BasicType.T_BOOL && tes5set.Item2.TES5Type == TES5BasicType.T_INT)
                    {
                        int item2Value = ((TES5Integer)tes5set.Item2).IntValue;
                        if (item2Value != 0 && item2Value != 1) { throw new ConversionException("Unexpected Value:  " + item2Value.ToString()); }
                        ITES5Value newItem2 = new TES5Bool(item2Value == 1);
                        newOp = op == TES5ComparisonExpressionOperator.OPERATOR_EQUAL ? op : TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL;
                        return TES5ExpressionFactory.CreateComparisonExpression(tes5set.Item1, newOp, newItem2);
                    }
                    throw new ConversionException("Type could not be converted.");
                }
                flipOperator = true;
            }
            return TES5ExpressionFactory.CreateComparisonExpression(leftValue, op, rightValue);
        }

        public ITES5Value ConvertExpression(ITES4BinaryExpression expression, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ComparisonExpressionOperator ceOp = TES5ComparisonExpressionOperator.GetFirstOrNull(expression.Operator.Name);
            if (ceOp != null)
            {
                return this.ConvertComparisonExpression(expression, codeScope, globalScope, multipleScriptsScope);
            }
            TES5LogicalExpressionOperator leOp = TES5LogicalExpressionOperator.GetFirstOrNull(expression.Operator.Name);
            if (leOp != null)
            {
                return TES5ExpressionFactory.CreateLogicalExpression(this.CreateValue(expression.LeftValue, codeScope, globalScope, multipleScriptsScope), leOp, this.CreateValue(expression.RightValue, codeScope, globalScope, multipleScriptsScope));
            }
            TES5ArithmeticExpressionOperator aeOp = TES5ArithmeticExpressionOperator.GetFirstOrNull(expression.Operator.Name);
            if (aeOp != null)
            {
                return TES5ExpressionFactory.CreateArithmeticExpression(this.CreateValue(expression.LeftValue, codeScope, globalScope, multipleScriptsScope), aeOp, this.CreateValue(expression.RightValue, codeScope, globalScope, multipleScriptsScope));
            }
            throw new ConversionException("Unknown expression op:  " + expression.Operator.Name);
        }

        /*
         * @throws ConversionException
        */
        public ITES5Value CreateValue(ITES4Value value, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES4Primitive? primitive = value as ITES4Primitive;
            if (primitive != null) { return TES5PrimitiveValueFactory.CreateValue(primitive); }
            ITES4Reference? reference = value as ITES4Reference;
            if (reference != null) { return this.referenceFactory.CreateReadReference(reference.StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope); }
            ITES4Callable? callable = value as ITES4Callable;
            if (callable != null) { return this.CreateCodeChunk(callable, codeScope, globalScope, multipleScriptsScope); }
            ITES4BinaryExpression? expression = value as ITES4BinaryExpression;
            if (expression != null) { return this.ConvertExpression(expression, codeScope, globalScope, multipleScriptsScope); }
            throw new ConversionException("Unknown ITES4Value: " + value.GetType().FullName);
        }

        public ITES5ValueCodeChunk CreateCodeChunk(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Function function = chunk.Function;
            ITES5Referencer calledOnReference = this.CreateCalledOnReferenceOfCalledFunction(chunk, codeScope, globalScope, multipleScriptsScope);
            string functionName = function.FunctionCall.FunctionName;
            string functionKey = functionName.ToLower();
            IFunctionFactory factory;
            if (!this.functionFactories.TryGetValue(functionKey, out factory))
            {
                throw new ConversionException("Cannot convert function " + functionName + " as conversion handler is not defined.");
            }
            TES5FunctionFactoryUseTracker.Add(functionKey, factory, globalScope.ScriptHeader.OriginalScriptName);
            ITES5ValueCodeChunk codeChunk = factory.ConvertFunction(calledOnReference, function, codeScope, globalScope, multipleScriptsScope);
            return codeChunk;
        }

        public TES5CodeChunkCollection CreateCodeChunks(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return new TES5CodeChunkCollection() { CreateCodeChunk(chunk, codeScope, globalScope, multipleScriptsScope) };
        }

        /*
        * Returns a called-on reference for the called function.
        */
        private ITES5Referencer CreateCalledOnReferenceOfCalledFunction(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4ApiToken? calledOn = chunk.CalledOn;
            if (calledOn != null)
            {
                return this.referenceFactory.CreateReference(calledOn.StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope);
            }
            else
            {
                return this.referenceFactory.ExtractImplicitReference(globalScope, multipleScriptsScope, codeScope.LocalScope);
            }
        }

    }
}