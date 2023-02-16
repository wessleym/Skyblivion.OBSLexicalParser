using Dissect.Lexer;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class TES4OBScriptGrammar : TES4ObscriptCodeGrammar
    {
        public TES4OBScriptGrammar()
            : base(false)
        {
            __invoke("Script")
                .Is("ScriptHeader", "Block+").Call((TES4ScriptHeader header, List<ITES4CodeBlockOrComment> blockList) =>
                {
                    return new TES4Script(header, new List<ITES4ScriptHeaderVariableDeclarationOrComment>(), blockList);
                })
                .Is("ScriptHeader", "VariableDeclarationOrComment+").Call((TES4ScriptHeader header, List<ITES4ScriptHeaderVariableDeclarationOrComment> variableDeclarationsOrComments) =>
                {
                    return new TES4Script(header, variableDeclarationsOrComments, new List<ITES4CodeBlockOrComment>());
                })
                .Is("ScriptHeader", "VariableDeclarationOrComment+", "Block+").Call((TES4ScriptHeader header, List<ITES4ScriptHeaderVariableDeclarationOrComment> variableDeclarationsOrComments, List<ITES4CodeBlockOrComment> blockList) =>
                {
                    return new TES4Script(header, variableDeclarationsOrComments, blockList);
                })
#if NEWBT
                .Is("ScriptHeader").Call((TES4ScriptHeader header) =>//WTM:  Change:  I added this section to allow for a few more scripts to parsed (scripts that are just a header).
                {
                    return new TES4Script(header, null, null);
                })
#endif
                ;

            __invoke("ScriptHeader")
                .Is("ScriptHeaderToken", "ScriptName", "CRLF").Call((object headerToken, CommonToken scriptName, object crlf) =>
                {
                    return new TES4ScriptHeader(scriptName.Value);
                });

            __invoke("VariableDeclarationOrComment+")
                .Is("VariableDeclarationOrComment+", "VariableDeclarationOrComment").Call((List<ITES4ScriptHeaderVariableDeclarationOrComment> list, ITES4ScriptHeaderVariableDeclarationOrComment variableDeclarationOrComment) =>
                {
                    list.Add(variableDeclarationOrComment);
                    return list;
                })
                .Is("VariableDeclarationOrComment").Call((ITES4ScriptHeaderVariableDeclarationOrComment variableDeclarationOrComment)=>
                {
                    return new List<ITES4ScriptHeaderVariableDeclarationOrComment>() { variableDeclarationOrComment };
                });

            __invoke("VariableDeclarationOrComment")
                .Is("VariableDeclaration", "CRLF").Call((TES4VariableDeclaration variableDeclaration, object crlf) =>
                {
                    return (ITES4ScriptHeaderVariableDeclarationOrComment)variableDeclaration;
                })
                .Is("VariableDeclaration", "Comment", "CRLF").Call((TES4VariableDeclaration variableDeclaration, TES4Comment comment, object crlf) =>
                {
                    variableDeclaration.SetComment(comment);
                    return (ITES4ScriptHeaderVariableDeclarationOrComment)variableDeclaration;
                })
                .Is("Comment", "CRLF").Call((TES4Comment comment, object crlf) =>
                {
                    return (ITES4ScriptHeaderVariableDeclarationOrComment)comment;
                });

            __invoke("VariableDeclaration")
                .Is("VariableDeclarationType", "VariableName").Call((CommonToken variableDeclarationType, CommonToken variableName) =>
                {
                    return new TES4VariableDeclaration(variableName.Value, TES4Type.GetFirst(variableDeclarationType.Value.ToLower()));
                });

            __invoke("Block+")
                .Is("Block+", "Comment+", "Block").Call((List<ITES4CodeBlockOrComment> list, List<TES4Comment> comments, TES4CodeBlock blockDeclaration) =>
                {
                    list.AddRange(comments);
                    list.Add(blockDeclaration);
                    return list;
                })
                .Is("Block+", "Block").Call((List<ITES4CodeBlockOrComment> list, TES4CodeBlock blockDeclaration) =>
                {
                    list.Add(blockDeclaration);
                    return list;
                })
                .Is("Block").Call((TES4CodeBlock block) =>
                {
                    return new List<ITES4CodeBlockOrComment> { block };
                })
                .Is("Block+", "Comment+").Call((List<ITES4CodeBlockOrComment> list, List<TES4Comment> comments) =>
                {
                    list.AddRange(comments);
                    return list;
                });

            __invoke("Block")
                .Is("BlockStartBlockType(OptionalParameter+)(OptionalComment)", "CRLF", "Code+", "BlockEnd", "CRLF").Call((TES4CodeBlock codeBlock, object crlf, TES4CodeChunks codeChunks, object blockEnd, object crlf2) =>
                {
                    codeBlock.Chunks.AddRange(codeChunks);
                    return codeBlock;
                })
                .Is("BlockStartBlockType(OptionalParameter+)(OptionalComment)", "CRLF", "Code+", "BlockEnd", "Comment", "CRLF").Call((TES4CodeBlock codeBlock, object crlf, TES4CodeChunks codeChunks, object blockEnd, TES4Comment comment, object crlf2) =>
                {
                    codeBlock.Chunks.AddRange(codeChunks);
                    codeBlock.Chunks.Add(comment);//This actually positions the comment on the line before the end line instead of right on the end line.
                    return codeBlock;
                })
                .Is("BlockStartBlockType(OptionalParameter+)(OptionalComment)", "CRLF", "BlockEnd", "CRLF");

            __invoke("BlockStartBlockType(OptionalParameter+)(OptionalComment)")//rare empty block
                .Is("BlockStartBlockType(OptionalParameter+)", "Comment").Call((TES4CodeBlock codeBlock, TES4Comment comment) =>
                {
                    codeBlock.Chunks.Add(comment);
                    return codeBlock;
                })
                .Is("BlockStartBlockType(OptionalParameter+)");

            __invoke("BlockStartBlockType(OptionalParameter+)")
                .Is("BlockStart", "BlockType", "BlockParameter+").Call((object blockStart, CommonToken blockType, List<TES4BlockParameter> blockParameters) =>
                {
                    return new TES4CodeBlock(blockType.Value, blockParameters, new TES4CodeChunks());
                })
                .Is("BlockStart", "BlockType").Call((object blockStart, CommonToken blockType) =>
                {
                    return new TES4CodeBlock(blockType.Value, new List<TES4BlockParameter>(), new TES4CodeChunks());
                });

            __invoke("BlockParameter+")
                .Is("BlockParameter+", "BlockParameter").Call((List<TES4BlockParameter> list, TES4BlockParameter blockParameter) =>
                {
                    list.Add(blockParameter);
                    return list;
                })
                .Is("BlockParameter").Call((TES4BlockParameter blockParameter) =>
                {
                    return new List<TES4BlockParameter>() { blockParameter };
                });

            __invoke("BlockParameter").Is("BlockParameterToken").Call((CommonToken token) =>
            {
                return new TES4BlockParameter(token.Value);
            });

            this.CreateObscriptCodeParsingTree();
            this.Start("Script");
        }
    }
}