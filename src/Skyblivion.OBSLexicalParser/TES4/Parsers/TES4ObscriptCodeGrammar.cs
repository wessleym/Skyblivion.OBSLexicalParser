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
using System.Collections.Generic;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class TES4ObscriptCodeGrammar : Grammar
    {
        protected TES4ObscriptCodeGrammar(bool createCodeParsingTree)
        {
            if (createCodeParsingTree)
            {
                this.CreateObscriptCodeParsingTree();
                this.Start("Code+");
            }
        }
        public TES4ObscriptCodeGrammar()
            : this(true)
        { }

        protected void CreateObscriptCodeParsingTree()
        {
            __invoke("Code+")
                .Is("Code+", "Code").Call((TES4CodeChunks list, ITES4CodeChunk code) =>
                {
                    list.Add(code);
                    return list;
                })
                .Is("Code").Call((ITES4CodeChunk code) =>
                {
                    TES4CodeChunks chunks = new TES4CodeChunks();
                    chunks.Add(code);
                    return chunks;
                });
            
            __invoke("Code").Is("Branch").Is("SetValue(OptionalComment)", "CRLF").Is("FunctionCallAndArguments(OptionalComment)", "CRLF").Is("ObjectCall(OptionalComment)", "CRLF").Is("LocalVariableDeclaration+").Is("Return").Is("Comment");
            
            __invoke("LocalVariableDeclaration+")
                .Is("LocalVariableDeclaration+", "LocalVariableDeclaration").Call((List<TES4VariableDeclaration> list, TES4VariableDeclaration variableDeclaration) =>
                {
                    list.Add(variableDeclaration);
                    return list;
                })
                .Is("LocalVariableDeclaration").Call((TES4VariableDeclaration variableDeclaration) =>
                {
                    return new List<TES4VariableDeclaration>() { variableDeclaration };
                });

            __invoke("LocalVariableDeclaration")
                .Is("LocalVariableDeclarationType", "VariableName").Call((CommonToken variableDeclarationType, CommonToken variableName) =>
                {
                    return new TES4VariableDeclaration(variableName.Value, TES4Type.GetFirst(variableDeclarationType.Value.ToLower()));
                });

            __invoke("Branch")
                .Is("BranchStart", "BranchEndToken").Call((TES4SubBranch branchStart, object end) =>
                {
                    return new TES4Branch(branchStart, null, null);
                })
                .Is("BranchStart", "BranchSubBranch+", "BranchEndToken").Call((TES4SubBranch branchStart, List<TES4SubBranch> subbranches, object end) =>
                {
                    return new TES4Branch(branchStart, subbranches, null);
                })
                .Is("BranchStart", "BranchElse", "BranchEndToken").Call((TES4SubBranch branchStart, TES4ElseSubBranch branchElse, object end) =>
                {
                    return new TES4Branch(branchStart, null, branchElse);
                })
                .Is("BranchStart", "BranchSubBranch+", "BranchElse", "BranchEndToken").Call((TES4SubBranch branchStart, List<TES4SubBranch> subbranches, TES4ElseSubBranch branchElse, object end) =>
                {
                    return new TES4Branch(branchStart, subbranches, branchElse);
                });

            __invoke("BranchElse")
                .Is("BranchElseToken", "Code+").Call((object branchElseToken, TES4CodeChunks code) =>
                {
                    return new TES4ElseSubBranch(code);
                })
                .Is("BranchElseToken").Call((object branchElseToken) =>
                {
                    return new TES4ElseSubBranch(new TES4CodeChunks());
                });
            
            __invoke("BranchStart")
                .Is("BranchStartLine", "Code+").Call((TES4SubBranch subBranch, TES4CodeChunks code) =>
                {
                    subBranch.CodeChunks.AddRange(code);
                    return subBranch;
                }).
                Is("BranchStartLine");

            __invoke("BranchStartLine")
                .Is("BranchStartToken", "Value", "CRLF").Call((object branchStart, ITES4Value expression, object crlf) =>
                {
                    return new TES4SubBranch(expression, new TES4CodeChunks());
                })
                .Is("BranchStartToken", "Value", "Comment", "CRLF").Call((object branchStart, ITES4Value expression, TES4Comment comment, object crlf) =>
                {
                    TES4CodeChunks chunks = new TES4CodeChunks();
                    chunks.Add(comment);
                    return new TES4SubBranch(expression, chunks);
                });

            __invoke("BranchSubBranch+")
                .Is("BranchSubBranch+", "BranchSubBranch").Call((List<TES4SubBranch> list, TES4SubBranch branchSubBranchDeclaration) =>
                {
                    list.Add(branchSubBranchDeclaration);
                    return list;
                })
                .Is("BranchSubBranch").Call((TES4SubBranch branchSubBranchDeclaration) =>
                {
                    return new List<TES4SubBranch>() { branchSubBranchDeclaration };
                });

            __invoke("BranchSubBranch")
                .Is("BranchSubBranchLine", "Code+").Call((TES4SubBranch subBranch, TES4CodeChunks codeChunks) =>
                {
                    subBranch.CodeChunks.AddRange(codeChunks);
                    return subBranch;
                }).
                Is("BranchSubBranchLine");

            __invoke("BranchSubBranchLine")
                .Is("BranchElseifToken", "Value", "CRLF").Call((object branchElseif, ITES4Value expression, object crlf) =>
                {
                    return new TES4SubBranch(expression, new TES4CodeChunks());
                })
                .Is("BranchElseifToken", "Value", "Comment", "CRLF").Call((object branchElseif, ITES4Value expression, TES4Comment comment, object crlf) =>
                {
                    TES4CodeChunks codeChunks = new TES4CodeChunks();
                    codeChunks.Add(comment);
                    return new TES4SubBranch(expression, codeChunks);
                });

            __invoke("MathOperator")
                .Is("==").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                })
                .Is("!=").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                })
                .Is(">").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                })
                .Is("<").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                })
                .Is("<=").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                })
                .Is(">=").Call((CommonToken op) =>
                {
                    return TES4ComparisonExpressionOperator.GetFirst(op.Value);
                });

            __invoke("LogicalOperator")
                .Is("||").Call((CommonToken op) =>
                {
                    return TES4LogicalExpressionOperator.GetFirst(op.Value);
                })
                .Is("&&").Call((CommonToken op) =>
                {
                    return TES4LogicalExpressionOperator.GetFirst(op.Value);
                });

            __invoke("Value")
                .Is("Value", "LogicalOperator", "NotLogicalValue").Call((ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right) =>
                {
                    return new TES4LogicalExpression(left, op, right);
                })
                .Is("NotLogicalValue");

            __invoke("NotLogicalValue")
                .Is("NotLogicalValue", "MathOperator", "NotLogicalAndBinaryValue").Call((ITES4Value left, TES4ComparisonExpressionOperator op, ITES4Value right) =>
                {
                    return new TES4ComparisonExpression(left, op, right);
                })
                .Is("NotLogicalAndBinaryValue");

            __invoke("NotLogicalAndBinaryValue")
                .Is("NotLogicalAndBinaryValue", "BinaryOperator", "NonExpressionValue").Call((ITES4Value left, TES4ArithmeticExpressionOperator op, ITES4Value right) =>
                {
                    return new TES4ArithmeticExpression(left, op, right);
                })
                .Is("NonExpressionValue");

            __invoke("NonExpressionValue").Is("ObjectAccess").Is("FunctionCallAndArguments").Is("APIToken").Is("Primitive");

            __invoke("BinaryOperator")
                .Is("+").Call((CommonToken op) =>
                {
                    return TES4ArithmeticExpressionOperator.GetFirst(op.Value);
                })
                .Is("-").Call((CommonToken op) =>
                {
                    return TES4ArithmeticExpressionOperator.GetFirst(op.Value);
                })
                .Is("*").Call((CommonToken op) =>
                {
                    return TES4ArithmeticExpressionOperator.GetFirst(op.Value);
                })
                .Is("/").Call((CommonToken op) =>
                {
                    return TES4ArithmeticExpressionOperator.GetFirst(op.Value);
                });

            __invoke("ObjectAccess").Is("ObjectCall").Is("ObjectProperty");

            __invoke("ObjectCall").Is("APIToken", "TokenDelimiter", "FunctionCallAndArguments").Call((TES4ApiToken apiToken, object delimiter, TES4Function function) =>
            {
                return new TES4ObjectCall(apiToken, function);
            });

            __invoke("ObjectCall(OptionalComment)")
                .Is("ObjectCall")
                .Is("ObjectCall", "Comment").Call((TES4ObjectCall objectCall, TES4Comment comment) =>
                {
                    objectCall.Function.SetComment(comment);
                    return objectCall;
                });

            __invoke("ObjectProperty").Is("APIToken", "TokenDelimiter", "APIToken").Call((TES4ApiToken apiToken, object delimiter, TES4ApiToken nextApiToken) =>
            {
                return new TES4ObjectProperty(apiToken, nextApiToken);
            });

            __invoke("SetValue(OptionalComment)")
                .Is("SetValue").Call((TES4VariableAssignation variableAssignation) =>
                {
                    return variableAssignation;
                })
                .Is("SetValue", "Comment").Call((TES4VariableAssignation variableAssignation, TES4Comment comment) =>
                {
                    variableAssignation.SetComment(comment);
                    return variableAssignation;
                });

            __invoke("SetValue")
                .Is("SetInitialization", "ObjectProperty", "Value").Call((object setInitialization, TES4ObjectProperty objectProperty, ITES4Value expression) =>
                {
                    return new TES4VariableAssignation(objectProperty, expression);
                })
                .Is("SetInitialization", "APIToken", "Value").Call((object setInitialization, TES4ApiToken apiToken, ITES4Value expression) =>
                {
                    return new TES4VariableAssignation(apiToken, expression);
                });

            __invoke("FunctionCallAndArguments(OptionalComment)")
                .Is("FunctionCallAndArguments")
                .Is("FunctionCallAndArguments", "Comment").Call((TES4Function function, TES4Comment comment) =>
                {
                    function.SetComment(comment);
                    return function;
                }).

            __invoke("FunctionCallAndArguments")
                .Is("FunctionCall", "FunctionArguments").Call((TES4FunctionCall functionCall, TES4FunctionArguments functionArguments) =>
                {
                    return new TES4Function(functionCall, functionArguments);
                }).
                Is("FunctionCall").Call((TES4FunctionCall functionCall) =>
                {
                    return new TES4Function(functionCall, new TES4FunctionArguments());
                });

            __invoke("FunctionCall").Is("FunctionCallToken").Call((CommonToken functionCall) =>
            {
                return new TES4FunctionCall(functionCall.Value);
            });

            __invoke("APIToken").Is("ReferenceToken").Call((CommonToken token) =>
            {
                return new TES4ApiToken(token.Value);
            });

            __invoke("FunctionArguments")
                .Is("FunctionArguments", "FunctionParameter").Call((TES4FunctionArguments list, ITES4ValueString value) =>
                {
                    list.Add(value);
                    return list;
                })
                .Is("FunctionParameter").Call((ITES4ValueString value) =>
                {
                    return new TES4FunctionArguments() { value };
                });

            __invoke("FunctionParameter").Is("ObjectAccess").Is("FunctionCallAndArguments").Is("APIToken").Is("Primitive");

            __invoke("Primitive")
                .Is("Float").Call((CommonToken fl) =>
                {
                    return new TES4Float(float.Parse(fl.Value, CultureInfo.InvariantCulture));
                })
                .Is("Integer").Call((CommonToken token) =>
                {
                    return new TES4Integer(int.Parse(token.Value, CultureInfo.InvariantCulture));
                })
                .Is("Boolean").Call((CommonToken token) =>
                {
                    if (token.Value.ToLower() == "true")
                    {
                        return new TES4Integer(1);
                    }
                    return new TES4Integer(0);
                })
                .Is("String").Call((CommonToken str) =>
                {
                    return new TES4String(str.Value);
                });

            __invoke("Return").Is("ReturnToken").Call((object returnToken) =>
            {
                return new TES4Return();
            });

            __invoke("Comment+")
                .Is("Comment+", "CRLF", "Comment").Call((List<TES4Comment> comments, object crlf, TES4Comment comment) =>
                {
                    comments.Add(comment);
                    return comments;
                })
                .Is("Comment").Call((TES4Comment comment) =>
                {
                    return new List<TES4Comment>() { comment };
                })
                .Is("Comment", "CRLF").Call((TES4Comment comment, object crlf) =>
                {
                    return new List<TES4Comment>() { comment };
                });

            __invoke("Comment").Is("CommentInitialization", "CommentText").Call((object commentInitialization, CommonToken comment) =>
            {
                return new TES4Comment(comment.Value);
            });
        }
    }
}