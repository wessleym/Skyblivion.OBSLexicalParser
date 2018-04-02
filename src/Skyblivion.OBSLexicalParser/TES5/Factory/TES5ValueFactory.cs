using Skyblivion.OBSLexicalParser.TES4.AST.Expression;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.Context;
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
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private Dictionary<string, IFunctionFactory> functionFactories = new Dictionary<string, IFunctionFactory>();
        public TES5ValueFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
        }
        public void addFunctionFactory(string functionName, IFunctionFactory factory)
        {
            string key = functionName.ToLower();
            this.functionFactories.Add(key, factory);
        }
        private ITES5Value convertArithmeticExpression(ITES4Expression expression, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            Tuple<ITES4Value, ITES4Value>[] sets = new Tuple<ITES4Value, ITES4Value>[]
            {
                new Tuple<ITES4Value, ITES4Value>(expression.getLeftValue(), expression.getRightValue()),
                new Tuple<ITES4Value, ITES4Value>(expression.getRightValue(), expression.getLeftValue())
            };

            /*
             * Scenario 1 - Special functions converted on expression level
             */
            foreach (Tuple<ITES4Value, ITES4Value> set in sets)
            {
                ITES4Callable setItem1Callable = set.Item1 as ITES4Callable;
                if (setItem1Callable == null) { continue; }

                TES4Function function = setItem1Callable.getFunction();

                switch (function.getFunctionCall().getFunctionName().ToLower())
                {
                    case "getweaponanimtype":
                        {
                            ITES5Referencer calledOn = this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope);
                            TES5ObjectCall equippedWeaponLeftValue = this.objectCallFactory.createObjectCall(this.objectCallFactory.createObjectCall(calledOn, "GetEquippedWeapon", multipleScriptsScope), "GetWeaponType", multipleScriptsScope);

                            int[] targetedWeaponTypes;
                            switch ((int)set.Item2.getData())
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

                            List<TES5ArithmeticExpression> expressions = new List<TES5ArithmeticExpression>();

                            foreach (var targetedWeaponType in targetedWeaponTypes)
                            {
                                expressions.Add(TES5ExpressionFactory.createArithmeticExpression(equippedWeaponLeftValue, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Integer(targetedWeaponType)));
                            }

                            ITES5Expression resultExpression = expressions[0];

                            expressions.RemoveAt(0);

                            while (expressions.Any())
                            {
                                resultExpression = TES5ExpressionFactory.createLogicalExpression(resultExpression, TES5LogicalExpressionOperator.OPERATOR_OR, expressions.Last());
                                expressions.RemoveAt(expressions.Count - 1);
                            }

                            return resultExpression;
                        }

                    case "getdetected":
                        {
                            TES5ObjectCallArguments inversedArgument = new TES5ObjectCallArguments();
                            inversedArgument.add(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope));
                            TES5ObjectCall getDetectedLeftValue = this.objectCallFactory.createObjectCall(this.referenceFactory.createReadReference(function.getArguments().getValue(0).StringValue, globalScope, multipleScriptsScope, codeScope.getLocalScope()), "isDetectedBy", multipleScriptsScope, inversedArgument);
                            TES5Integer getDetectedRightValue = new TES5Integer(((int)set.Item2.getData() == 0) ? 0 : 1);
                            return TES5ExpressionFactory.createArithmeticExpression(getDetectedLeftValue, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, getDetectedRightValue);
                        }

                    case "getdetectionlevel":
                        {

                            if (!set.Item2.hasFixedValue())
                            {
                                throw new ConversionException("Cannot convert getDetectionLevel calls with dynamic comparision");
                            }

                            TES5Bool tes5Bool = new TES5Bool(((int)set.Item2.getData()) == 3); //true only if the compared value was 3

                            TES5ObjectCallArguments inversedArgument = new TES5ObjectCallArguments();
                            inversedArgument.add(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope));

                            return TES5ExpressionFactory.createArithmeticExpression
                                (
                                    this.objectCallFactory.createObjectCall(this.referenceFactory.createReadReference(function.getArguments().getValue(0).StringValue, globalScope, multipleScriptsScope, codeScope.getLocalScope()), "isDetectedBy", multipleScriptsScope, inversedArgument),
                                    TES5ArithmeticExpressionOperator.OPERATOR_EQUAL,
                                    tes5Bool
                                );
                        }

                    case "getcurrentaiprocedure":
                        {

                            if (!set.Item2.hasFixedValue())
                            {
                                throw new ConversionException("Cannot convert getCurrentAIProcedure() calls with dynamic comparision");
                            }

                            switch ((int)set.Item2.getData())
                            {
                                case 4:
                                    {
                                        //ref.getSleepState() == 3
                                        return TES5ExpressionFactory.createArithmeticExpression(
                                            this.objectCallFactory.createObjectCall(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "IsInDialogueWithPlayer", multipleScriptsScope),
                                            TES5ArithmeticExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Bool(expression.getOperator() == TES4ArithmeticExpressionOperator.OPERATOR_EQUAL) //cast to true if the original op was ==, false otherwise.
                                        );
                                    }

                                case 8:
                                    {

                                        //ref.getSleepState() == 3
                                        return TES5ExpressionFactory.createArithmeticExpression(
                                            this.objectCallFactory.createObjectCall(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "getSleepState", multipleScriptsScope),
                                            TES5ArithmeticExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Integer(3) //SLEEPSTATE_SLEEP
                                        );
                                    }
                                case 13:
                                    {

                                        //ref.getSleepState() == 3
                                        return TES5ExpressionFactory.createArithmeticExpression(
                                            this.objectCallFactory.createObjectCall(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "IsInCombat", multipleScriptsScope),
                                            TES5ArithmeticExpressionOperator.OPERATOR_EQUAL,
                                            new TES5Bool(expression.getOperator() == TES4ArithmeticExpressionOperator.OPERATOR_EQUAL) //cast to true if the original op was ==, false otherwise.
                                        );
                                    }

                                case 0:
                                case 7:
                                case 15:
                                case 17:
                                    {
                                        //@INCONSISTENCE Wander.. idk how to check it tbh. We return always true. Think about better representation
                                        return new TES5Bool(expression.getOperator() == TES4ArithmeticExpressionOperator.OPERATOR_EQUAL);
                                    }
                                default:
                                    {
                                        throw new ConversionException("Cannot convert GetCurrentAiProcedure - unknown TES4 procedure number arg " + ((int)set.Item2.getData()).ToString());
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
                            switch ((int)set.Item2.getData())
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
                            return TES5ExpressionFactory.createArithmeticExpression
                            (
                                this.objectCallFactory.createObjectCall(this.createCalledOnReferenceOfCalledFunction(setItem1Callable, codeScope, globalScope, multipleScriptsScope), "GetSitState", multipleScriptsScope),
                                TES5ArithmeticExpressionOperator.GetFirst(expression.getOperator().Name),
                                new TES5Integer(goTo)
                            );

                        }

                }

            }

            ITES5Value leftValue = this.createValue(expression.getLeftValue(), codeScope, globalScope, multipleScriptsScope);
            ITES5Value rightValue = this.createValue(expression.getRightValue(), codeScope, globalScope, multipleScriptsScope);

            Tuple<ITES5Value, ITES5Value>[] tes5sets = new Tuple<ITES5Value, ITES5Value>[]
                        {
                new Tuple<ITES5Value, ITES5Value>(leftValue, rightValue),
                new Tuple<ITES5Value, ITES5Value>(rightValue, leftValue)
                        };

            TES5BasicType objectReferenceType = TES5BasicType.T_FORM; //used just to make sure.
            TES5ArithmeticExpressionOperator op = TES5ArithmeticExpressionOperator.GetFirstOrNull(expression.getOperator().Name);

            /*
             * Scenario 2: Comparision of ObjectReferences to integers ( quick formid check )
             */
            foreach (var tes5set in tes5sets)
            {

                if (tes5set.Item1.getType().getOriginalName() == objectReferenceType.getOriginalName() || TES5InheritanceGraphAnalyzer.isExtending(tes5set.Item1.getType(), objectReferenceType))
                {
                    if (tes5set.Item2.getType() == TES5BasicType.T_INT)
                    {

                        //Perhaps we should allow to try to cast upwards for primitives, .asPrimitive() or similar
                        //In case we do know at compile time that we"re comparing against zero, then we can assume
                        //we can compare against None, which allows us not call GetFormID() on most probably None object
                        ITES5Primitive tes5SetItem2Primitive = tes5set.Item2 as ITES5Primitive;
                        if (tes5SetItem2Primitive != null && (int)tes5SetItem2Primitive.getValue() == 0)
                        {
                            TES5ArithmeticExpressionOperator targetOperator;
                            if (op == TES5ArithmeticExpressionOperator.OPERATOR_EQUAL)
                            {
                                targetOperator = op;
                            }
                            else
                            {
                                targetOperator = TES5ArithmeticExpressionOperator.OPERATOR_NOT_EQUAL;
                            }
                            return TES5ExpressionFactory.createArithmeticExpression(tes5set.Item1, targetOperator, new TES5None());

                        }
                        else
                        {
                            ITES5Referencer callable = (ITES5Referencer)tes5set.Item1;
                            TES5ObjectCall tes5setNewItem1 = this.objectCallFactory.createObjectCall(callable, "GetFormID", multipleScriptsScope); 
                            return TES5ExpressionFactory.createArithmeticExpression(tes5setNewItem1, op, tes5set.Item2);
                        }
                    }
                }
                else if (tes5set.Item1.getType().getOriginalName() == TES5TypeFactory._void().getOriginalName())
                {

                    ITES5Primitive tes5SetItem2Primitive = tes5set.Item2 as ITES5Primitive;
                    if (tes5SetItem2Primitive is TES5Integer || tes5SetItem2Primitive is TES5Float)
                    {
                        if ((int)tes5SetItem2Primitive.getValue() == 0)
                        {
                            return TES5ExpressionFactory.createArithmeticExpression(tes5set.Item1, op, new TES5None());
                        }
                    }
                }

            }

            return TES5ExpressionFactory.createArithmeticExpression(leftValue, op, rightValue);
        }
        public ITES5Value convertExpression(ITES4Expression expression, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ArithmeticExpressionOperator aeOp = TES5ArithmeticExpressionOperator.GetFirstOrNull(expression.getOperator().Name);
            if (aeOp != null)
            {
                return this.convertArithmeticExpression(expression, codeScope, globalScope, multipleScriptsScope);
            }
            TES5LogicalExpressionOperator leOp = TES5LogicalExpressionOperator.GetFirstOrNull(expression.getOperator().Name);
            if (leOp != null)
            {
                return TES5ExpressionFactory.createLogicalExpression(this.createValue(expression.getLeftValue(), codeScope, globalScope, multipleScriptsScope), leOp, this.createValue(expression.getRightValue(), codeScope, globalScope, multipleScriptsScope));
            }
            TES5BinaryExpressionOperator beOp = TES5BinaryExpressionOperator.GetFirstOrNull(expression.getOperator().Name);
            if (beOp != null)
            {
                return TES5ExpressionFactory.createBinaryExpression(this.createValue(expression.getLeftValue(), codeScope, globalScope, multipleScriptsScope), beOp, this.createValue(expression.getRightValue(), codeScope, globalScope, multipleScriptsScope));
            }
            throw new ConversionException("Unknown expression op");
        }

        /*
         * @throws ConversionException
        */
        public ITES5Value createValue(ITES4Value value, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES4Primitive primitive = value as ITES4Primitive;
            if (primitive != null) { return TES5PrimitiveValueFactory.createValue(primitive); }
            ITES4Reference reference = value as ITES4Reference;
            if (reference != null) { return this.referenceFactory.createReadReference(reference.StringValue, globalScope, multipleScriptsScope, codeScope.getLocalScope()); }
            ITES4Callable callable = value as ITES4Callable;
            if (callable != null) { return this.createCodeChunk(callable, codeScope, globalScope, multipleScriptsScope); }
            ITES4Expression expression = value as ITES4Expression;
            if (expression != null) { return this.convertExpression(expression, codeScope, globalScope, multipleScriptsScope); }
            throw new ConversionException("Unknown ITES4Value: " + value.GetType().FullName);
        }

        public ITES5ValueCodeChunk createCodeChunk(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Function function = chunk.getFunction();
            ITES5Referencer calledOnReference = this.createCalledOnReferenceOfCalledFunction(chunk, codeScope, globalScope, multipleScriptsScope);
            string functionName = function.getFunctionCall().getFunctionName();
            string functionKey = functionName.ToLower();
            IFunctionFactory factory;
            if (!this.functionFactories.TryGetValue(functionKey, out factory))
            {
                throw new ConversionException("Cannot convert function " + functionName + " as conversion handler is not defined.");
            }
            ITES5ValueCodeChunk codeChunk = factory.convertFunction(calledOnReference, function, codeScope, globalScope, multipleScriptsScope);
            return codeChunk;
        }

        public TES5CodeChunkCollection createCodeChunks(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5CodeChunk codeChunk = createCodeChunk(chunk, codeScope, globalScope, multipleScriptsScope);
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            codeChunks.add(codeChunk);
            return codeChunks;
        }

        /*
        * Returns a called-on reference for the called function.
        */
        private ITES5Referencer createCalledOnReferenceOfCalledFunction(ITES4Callable chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4ApiToken calledOn = chunk.getCalledOn();
            if (calledOn != null)
            {
                return this.referenceFactory.createReference(calledOn.StringValue, globalScope, multipleScriptsScope, codeScope.getLocalScope());
            }
            else
            {
                return this.referenceFactory.extractImplicitReference(globalScope, multipleScriptsScope, codeScope.getLocalScope());
            }
        }

    }
}