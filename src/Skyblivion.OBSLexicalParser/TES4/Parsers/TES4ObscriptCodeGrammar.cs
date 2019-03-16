using Dissect.Lexer;
using Dissect.Parser;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class TES4ObscriptCodeGrammar : Grammar
    {
        protected TES4ObscriptCodeGrammar(bool createCodeParsingTree)
        {
            if (createCodeParsingTree)
            {
                this.createObscriptCodeParsingTree();
                this.Start("Code+");
            }
        }
        public TES4ObscriptCodeGrammar()
            : this(true)
        { }

        protected void createObscriptCodeParsingTree()
        {
            __invoke("Code+").Is("Code+", "Code").Call((TES4CodeChunks list, ITES4CodeChunk codeDeclaration)=>
            {
                list.Add(codeDeclaration);
                return list;
            } ) .
            Is("Code").Call((ITES4CodeChunk codeDeclaration)=>
            {
                TES4CodeChunks list = new TES4CodeChunks();
                list.Add(codeDeclaration);
                return list;
            });
            __invoke("Code").Is("Branch").Is("SetValue", "NWL").Is("Function", "NWL").Is("ObjectCall", "NWL").Is("LocalVariableDeclaration+").Is("Return");
//todo-THIS should be fixed on lexer level, right now it ignores NWL after the return
            __invoke("LocalVariableDeclaration+").Is("LocalVariableDeclaration+", "LocalVariableDeclaration").Call((TES4VariableDeclarationList list, TES4VariableDeclaration variableDeclaration)=>
            {
                list.add(variableDeclaration);
                return list;
            } ) .

            Is("LocalVariableDeclaration").Call((TES4VariableDeclaration variableDeclaration)=>
            {
                TES4VariableDeclarationList list = new TES4VariableDeclarationList();
                list.add(variableDeclaration);
                return list;
            } )

            ;
            __invoke("LocalVariableDeclaration").Is("LocalVariableDeclarationType", "VariableName").Call((System.Func<CommonToken, CommonToken, TES4VariableDeclaration>)((CommonToken variableDeclarationType, CommonToken variableName)=>
            {
                return new TES4VariableDeclaration((string)variableName.Value, TES4Type.GetFirst((string)variableDeclarationType.Value.ToLower()));
            }) )

            ;
            __invoke("Branch").Is("BranchStart", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .Call((TES4SubBranch branchStart, object end)=>
            {
                return new TES4Branch(branchStart, null, null);
            } ) .

            Is("BranchStart", "BranchSubBranch+", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .Call((TES4SubBranch branchStart, TES4SubBranchList subbranches, object end)=>
            {
                return new TES4Branch(branchStart, subbranches, null);
            } ) .

            Is("BranchStart", "BranchElse", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .Call((TES4SubBranch branchStart, TES4ElseSubBranch branchElse, object end)=>
            {
                return new TES4Branch(branchStart, null, branchElse);
            } ) .

            Is("BranchStart", "BranchSubBranch+", "BranchElse", "BranchEndToken").Call((TES4SubBranch branchStart, TES4SubBranchList subbranches, TES4ElseSubBranch branchElse, object end)=>
            {
                return new TES4Branch(branchStart, subbranches, branchElse);
            } )

            ;
            __invoke("BranchElse").Is("BranchElseToken", "Code+").Call((object branchElseToken, TES4CodeChunks code)=>
            {
                return new TES4ElseSubBranch(code);
            } ) .

            Is("BranchElseToken").Call((branchElseToken)=>
            {
                return new TES4ElseSubBranch(null);
            } )

            ;
            __invoke("BranchStart").Is("BranchStartToken", "Value", "NWL", "Code+").Call((object branchStart, ITES4Value expression, object newLine, TES4CodeChunks code)=>
            {
                return new TES4SubBranch(expression, code);
            } ) .

            Is("BranchStartToken", "Value", "NWL").Call((object branchStart, ITES4Value expression, object newLine) =>
            {
                return new TES4SubBranch(expression, null);
            } )

            ;
            __invoke("BranchSubBranch+").Is("BranchSubBranch+", "BranchSubBranch").Call((TES4SubBranchList list, TES4SubBranch branchSubBranchDeclaration)=>
            {
                list.Add(branchSubBranchDeclaration);
                return list;
            } ) .

            Is("BranchSubBranch").Call((TES4SubBranch branchSubBranchDeclaration)=>
            {
                TES4SubBranchList list = new TES4SubBranchList();
                list.Add(branchSubBranchDeclaration);
                return list;
            } )

            ;
            __invoke("BranchSubBranch").Is("BranchElseifToken", "Value", "NWL", "Code+").Call((object branchElseif, ITES4Value expression, object nwl, TES4CodeChunks codeChunks)=>
            {
                return new TES4SubBranch(expression, codeChunks);
            } ) .

            Is("BranchElseifToken", "Value", "NWL").Call((object branchElseif, ITES4Value expression, object nwl)=>
            {
                return new TES4SubBranch(expression, null);
            } )

            ;
            __invoke("MathOperator").Is("==").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("!=").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is(">").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("<").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("<=").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is(">=").Call((System.Func<CommonToken, TES4ComparisonExpressionOperator>)((CommonToken op)=>
            {
                return TES4ComparisonExpressionOperator.GetFirst((string)op.Value);
            }) )

            ;
            __invoke("LogicalOperator").Is("||").Call((System.Func<CommonToken, TES4LogicalExpressionOperator>)((CommonToken op)=>
            {
                return TES4LogicalExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("&&").Call((System.Func<CommonToken, TES4LogicalExpressionOperator>)((CommonToken op)=>
            {
                return TES4LogicalExpressionOperator.GetFirst((string)op.Value);
            }) )

            ;
            __invoke("Value").Is("Value", "LogicalOperator", "NotLogicalValue").Call((ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right)=>
            {
                return new TES4LogicalExpression(left, op, right);
            } ) .

            Is("NotLogicalValue");
            __invoke("NotLogicalValue").Is("NotLogicalValue", "MathOperator", "NotLogicalAndBinaryValue").Call((ITES4Value left, TES4ComparisonExpressionOperator op, ITES4Value right)=>
            {
                return new TES4ComparisonExpression(left, op, right);
            } ) .

            Is("NotLogicalAndBinaryValue");
            __invoke("NotLogicalAndBinaryValue").Is("NotLogicalAndBinaryValue", "BinaryOperator", "NonExpressionValue").Call((ITES4Value left, TES4ArithmeticExpressionOperator op, ITES4Value right)=>
            {
                return new TES4ArithmeticExpression(left, op, right);
            } ) .

            Is("NonExpressionValue");
            __invoke("NonExpressionValue").Is("ObjectAccess").Is("Function").Is("APIToken").Is("Primitive");
            __invoke("BinaryOperator").Is("+").Call((System.Func<CommonToken, TES4ArithmeticExpressionOperator>)((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("-").Call((System.Func<CommonToken, TES4ArithmeticExpressionOperator>)((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("*").Call((System.Func<CommonToken, TES4ArithmeticExpressionOperator>)((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst((string)op.Value);
            }) ) .

            Is("/").Call((System.Func<CommonToken, TES4ArithmeticExpressionOperator>)((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst((string)op.Value);
            }) )

            ;
            __invoke("ObjectAccess").Is("ObjectCall").Is("ObjectProperty");
            __invoke("ObjectCall").Is("APIToken", "TokenDelimiter", "Function").Call((TES4ApiToken apiToken, object delimiter, TES4Function function)=>
            {
                return new TES4ObjectCall(apiToken, function);
            } )

            ;
            __invoke("ObjectProperty").Is("APIToken", "TokenDelimiter", "APIToken").Call((TES4ApiToken apiToken, object delimiter, TES4ApiToken nextApiToken)=>
            {
                return new TES4ObjectProperty(apiToken, nextApiToken);
            } )

            ;
            __invoke("SetValue").Is("SetInitialization", "ObjectProperty", "Value").Call((object setInitialization, TES4ObjectProperty objectProperty, ITES4Value expression)=>
            {
                return new TES4VariableAssignation(objectProperty, expression);
            } ) .

            Is("SetInitialization", "APIToken", "Value").Call((object setInitialization, TES4ApiToken apiToken, ITES4Value expression)=>
            {
                return new TES4VariableAssignation(apiToken, expression);
            } )

            ;
            __invoke("Function").Is("FunctionCall", "FunctionArguments").Call((TES4FunctionCall functionCall, TES4FunctionArguments functionArguments)=>
            {
                return new TES4Function(functionCall, functionArguments);
            } ) .

            Is("FunctionCall").Call((TES4FunctionCall functionCall) =>
            {
                return new TES4Function(functionCall, new TES4FunctionArguments());
            } )

            ;
            __invoke("FunctionCall").Is("FunctionCallToken").Call((System.Func<CommonToken, TES4FunctionCall>)((CommonToken functionCall)=>
            {
                return new TES4FunctionCall((string)functionCall.Value);
            }) )

            ;
            __invoke("APIToken").Is("ReferenceToken").Call((System.Func<CommonToken, TES4ApiToken>)((CommonToken token)=>
            {
                return new TES4ApiToken((string)token.Value);
            }) )

            ;
            __invoke("FunctionArguments").Is("FunctionArguments", "FunctionParameter").Call((TES4FunctionArguments list, ITES4StringValue value)=>
            {
                list.Add(value);
                return list;
            } ) .

            Is("FunctionParameter").Call((ITES4StringValue value)=>
            {
                TES4FunctionArguments list = new TES4FunctionArguments();
                list.Add(value);
                return list;
            } )

            ;
            __invoke("FunctionParameter").Is("ObjectAccess").Is("Function").Is("APIToken").Is("Primitive");
            __invoke("Primitive").Is("Float").Call((System.Func<CommonToken, TES4Float>)((CommonToken fl)=>
            {
                string floatValue = fl.Value;
                if (floatValue.StartsWith("."))
                {
                    floatValue = "0" + floatValue;
                }

                return new TES4Float(float.Parse(floatValue, CultureInfo.InvariantCulture));
            }) ) .

            Is("Integer").Call((System.Func<CommonToken, TES4Integer>)((CommonToken token)=>
            {
                return new TES4Integer(int.Parse((string)token.Value));
            }) ) .

            Is("Boolean").Call((System.Func<CommonToken, TES4Integer>)((CommonToken token)=>
            {
                if (token.Value.ToLower() == "true")
                {
                    return new TES4Integer(1);
                }
                return new TES4Integer(0);
            }) ) .

            Is("String").Call((System.Func<CommonToken, TES4String>)((CommonToken str)=>
            {
                return new TES4String((string)str.Value);
            }) )

            ;
            __invoke("Return").Is("ReturnToken", "NWL").Call((object returnToken, object nwl) =>
            {
                return new TES4Return();
            } ) .

            Is("ReturnToken").Call((returnToken)=>
            {
                return new TES4Return();
            } )

            ;
        }
    }
}