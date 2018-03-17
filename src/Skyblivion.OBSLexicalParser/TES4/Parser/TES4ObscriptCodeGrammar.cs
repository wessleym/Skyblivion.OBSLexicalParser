using Dissect.Lexer;
using Dissect.Parser;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class TES4ObscriptCodeGrammar : Grammar
    {
        public TES4ObscriptCodeGrammar()
        {
            this.createObscriptCodeParsingTree();
            this.start("Code+");
        }

        protected void createObscriptCodeParsingTree()
        {
            __invoke("Code+")._is("Code+", "Code").call((TES4CodeChunks list, ITES4CodeChunk codeDeclaration)=>
            {
                list.add(codeDeclaration);
                return list;
            } ) .
            _is("Code").call((ITES4CodeChunk codeDeclaration)=>
            {
                TES4CodeChunks list = new TES4CodeChunks();
                list.add(codeDeclaration);
                return list;
            });
            __invoke("Code")._is("Branch")._is("SetValue", "NWL")._is("Function", "NWL")._is("ObjectCall", "NWL")._is("LocalVariableDeclaration+")._is("Return");
//todo-THIS should be fixed on lexer level, right now it ignores NWL after the return
            __invoke("LocalVariableDeclaration+")._is("LocalVariableDeclaration+", "LocalVariableDeclaration").call((TES4VariableDeclarationList list, TES4VariableDeclaration variableDeclaration)=>
            {
                list.add(variableDeclaration);
                return list;
            } ) .

            _is("LocalVariableDeclaration").call((TES4VariableDeclaration variableDeclaration)=>
            {
                TES4VariableDeclarationList list = new TES4VariableDeclarationList();
                list.add(variableDeclaration);
                return list;
            } )

            ;
            __invoke("LocalVariableDeclaration")._is("LocalVariableDeclarationType", "VariableName").call((CommonToken variableDeclarationType, CommonToken variableName)=>
            {
                return new TES4VariableDeclaration(variableName.getValue(), TES4Type.GetFirst(variableDeclarationType.getValue().ToLower()));
            } )

            ;
            __invoke("Branch")._is("BranchStart", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .call((TES4SubBranch branchStart, object end)=>
            {
                return new TES4Branch(branchStart, null, null);
            } ) .

            _is("BranchStart", "BranchSubBranch+", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .call((TES4SubBranch branchStart, TES4SubBranchList subbranches, object end)=>
            {
                return new TES4Branch(branchStart, subbranches, null);
            } ) .

            _is("BranchStart", "BranchElse", "BranchEndToken") //If a == 2 { doSomeCode(); endIf
            .call((TES4SubBranch branchStart, TES4ElseSubBranch branchElse, object end)=>
            {
                return new TES4Branch(branchStart, null, branchElse);
            } ) .

            _is("BranchStart", "BranchSubBranch+", "BranchElse", "BranchEndToken").call((TES4SubBranch branchStart, TES4SubBranchList subbranches, TES4ElseSubBranch branchElse, object end)=>
            {
                return new TES4Branch(branchStart, subbranches, branchElse);
            } )

            ;
            __invoke("BranchElse")._is("BranchElseToken", "Code+").call((object branchElseToken, TES4CodeChunks code)=>
            {
                return new TES4ElseSubBranch(code);
            } ) .

            _is("BranchElseToken").call((branchElseToken)=>
            {
                return new TES4ElseSubBranch(null);
            } )

            ;
            __invoke("BranchStart")._is("BranchStartToken", "Value", "NWL", "Code+").call((object branchStart, ITES4Value expression, object newLine, TES4CodeChunks code)=>
            {
                return new TES4SubBranch(expression, code);
            } ) .

            _is("BranchStartToken", "Value", "NWL").call((object branchStart, ITES4Value expression, object newLine) =>
            {
                return new TES4SubBranch(expression, null);
            } )

            ;
            __invoke("BranchSubBranch+")._is("BranchSubBranch+", "BranchSubBranch").call((TES4SubBranchList list, TES4SubBranch branchSubBranchDeclaration)=>
            {
                list.add(branchSubBranchDeclaration);
                return list;
            } ) .

            _is("BranchSubBranch").call((TES4SubBranch branchSubBranchDeclaration)=>
            {
                TES4SubBranchList list = new TES4SubBranchList();
                list.add(branchSubBranchDeclaration);
                return list;
            } )

            ;
            __invoke("BranchSubBranch")._is("BranchElseifToken", "Value", "NWL", "Code+").call((object branchElseif, ITES4Value expression, object nwl, TES4CodeChunks codeChunks)=>
            {
                return new TES4SubBranch(expression, codeChunks);
            } ) .

            _is("BranchElseifToken", "Value", "NWL").call((object branchElseif, ITES4Value expression, object nwl)=>
            {
                return new TES4SubBranch(expression, null);
            } )

            ;
            __invoke("MathOperator")._is("==").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("!=").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is(">").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("<").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("<=").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is(">=").call((CommonToken op)=>
            {
                return TES4ArithmeticExpressionOperator.GetFirst(op.getValue());
            } )

            ;
            __invoke("LogicalOperator")._is("||").call((CommonToken op)=>
            {
                return TES4LogicalExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("&&").call((CommonToken op)=>
            {
                return TES4LogicalExpressionOperator.GetFirst(op.getValue());
            } )

            ;
            __invoke("Value")._is("Value", "LogicalOperator", "NotLogicalValue").call((ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right)=>
            {
                return new TES4LogicalExpression(left, op, right);
            } ) .

            _is("NotLogicalValue");
            __invoke("NotLogicalValue")._is("NotLogicalValue", "MathOperator", "NotLogicalAndBinaryValue").call((ITES4Value left, TES4ArithmeticExpressionOperator op, ITES4Value right)=>
            {
                return new TES4ArithmeticExpression(left, op, right);
            } ) .

            _is("NotLogicalAndBinaryValue");
            __invoke("NotLogicalAndBinaryValue")._is("NotLogicalAndBinaryValue", "BinaryOperator", "NonExpressionValue").call((ITES4Value left, TES4BinaryExpressionOperator op, ITES4Value right)=>
            {
                return new TES4BinaryExpression(left, op, right);
            } ) .

            _is("NonExpressionValue");
            __invoke("NonExpressionValue")._is("ObjectAccess")._is("Function")._is("APIToken")._is("Primitive");
            __invoke("BinaryOperator")._is("+").call((CommonToken op)=>
            {
                return TES4BinaryExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("-").call((CommonToken op)=>
            {
                return TES4BinaryExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("*").call((CommonToken op)=>
            {
                return TES4BinaryExpressionOperator.GetFirst(op.getValue());
            } ) .

            _is("/").call((CommonToken op)=>
            {
                return TES4BinaryExpressionOperator.GetFirst(op.getValue());
            } )

            ;
            __invoke("ObjectAccess")._is("ObjectCall")._is("ObjectProperty");
            __invoke("ObjectCall")._is("APIToken", "TokenDelimiter", "Function").call((TES4ApiToken apiToken, object delimiter, TES4Function function)=>
            {
                return new TES4ObjectCall(apiToken, function);
            } )

            ;
            __invoke("ObjectProperty")._is("APIToken", "TokenDelimiter", "APIToken").call((TES4ApiToken apiToken, object delimiter, TES4ApiToken nextApiToken)=>
            {
                return new TES4ObjectProperty(apiToken, nextApiToken);
            } )

            ;
            __invoke("SetValue")._is("SetInitialization", "ObjectProperty", "Value").call((object setInitialization, TES4ObjectProperty objectProperty, ITES4Value expression)=>
            {
                return new TES4VariableAssignation(objectProperty, expression);
            } ) .

            _is("SetInitialization", "APIToken", "Value").call((object setInitialization, TES4ApiToken apiToken, ITES4Value expression)=>
            {
                return new TES4VariableAssignation(apiToken, expression);
            } )

            ;
            __invoke("Function")._is("FunctionCall", "FunctionArguments").call((TES4FunctionCall functionCall, TES4FunctionArguments functionArguments)=>
            {
                return new TES4Function(functionCall, functionArguments);
            } ) .

            _is("FunctionCall").call((TES4FunctionCall functionCall) =>
            {
                return new TES4Function(functionCall, new TES4FunctionArguments());
            } )

            ;
            __invoke("FunctionCall")._is("FunctionCallToken").call((CommonToken functionCall)=>
            {
                return new TES4FunctionCall(functionCall.getValue());
            } )

            ;
            __invoke("APIToken")._is("ReferenceToken").call((CommonToken token)=>
            {
                return new TES4ApiToken(token.getValue());
            } )

            ;
            __invoke("FunctionArguments")._is("FunctionArguments", "FunctionParameter").call((TES4FunctionArguments list, ITES4StringValue value)=>
            {
                list.add(value);
                return list;
            } ) .

            _is("FunctionParameter").call((ITES4StringValue value)=>
            {
                TES4FunctionArguments list = new TES4FunctionArguments();
                list.add(value);
                return list;
            } )

            ;
            __invoke("FunctionParameter")._is("ObjectAccess")._is("Function")._is("APIToken")._is("Primitive");
            __invoke("Primitive")._is("Float").call((CommonToken fl)=>
            {
                string floatValue = fl.getValue();
                if (floatValue.StartsWith("."))
                {
                    floatValue = "0" + floatValue;
                }

                return new TES4Float(float.Parse(floatValue));
            } ) .

            _is("Integer").call((CommonToken token)=>
            {
                return new TES4Integer(int.Parse(token.getValue()));
            } ) .

            _is("Boolean").call((CommonToken token)=>
            {
                if (token.getValue().ToLower() == "true")
                {
                    return new TES4Integer(1);
                }
                return new TES4Integer(0);
            } ) .

            _is("String").call((CommonToken str)=>
            {
                return new TES4String(str.getValue());
            } )

            ;
            __invoke("Return")._is("ReturnToken", "NWL").call((object returnToken, object nwl) =>
            {
                return new TES4Return();
            } ) .

            _is("ReturnToken").call((returnToken)=>
            {
                return new TES4Return();
            } )

            ;
        }
    }
}