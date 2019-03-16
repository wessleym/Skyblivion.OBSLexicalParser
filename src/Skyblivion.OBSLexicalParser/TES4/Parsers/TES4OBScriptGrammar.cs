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
    class TES4OBScriptGrammar : TES4ObscriptCodeGrammar
    {
        public TES4OBScriptGrammar()
            : base(false)
        {
            __invoke("Script").Is("ScriptHeader", "Block+").Call((TES4ScriptHeader header, TES4BlockList blockList)=>
            {
                return new TES4Script(header, null, blockList);
            } ) .

            Is("ScriptHeader", "VariableDeclaration+").Call((TES4ScriptHeader header, TES4VariableDeclarationList variableList)=>
            {
                return new TES4Script(header, variableList, null);
            } ) .

            Is("ScriptHeader", "VariableDeclaration+", "Block+").Call((TES4ScriptHeader header, TES4VariableDeclarationList variableList, TES4BlockList blockList)=>
            {
                return new TES4Script(header, variableList, blockList);
            } )

            ;
            __invoke("ScriptHeader").Is("ScriptHeaderToken", "ScriptName").Call((System.Func<object, CommonToken, TES4ScriptHeader>)((object headerToken, CommonToken scriptName)=>
            {
                return new TES4ScriptHeader((string)scriptName.Value);
            }) )

            ;
            __invoke("VariableDeclaration+").Is("VariableDeclaration+", "VariableDeclaration").Call((TES4VariableDeclarationList list, TES4VariableDeclaration variableDeclaration)=>
            {
                list.add(variableDeclaration);
                return list;
            } ) .

            Is("VariableDeclaration").Call((TES4VariableDeclaration variableDeclaration)=>
            {
                TES4VariableDeclarationList list = new TES4VariableDeclarationList();
                list.add(variableDeclaration);
                return list;
            } )

            ;
            __invoke("VariableDeclaration").Is("VariableDeclarationType", "VariableName").Call((System.Func<CommonToken, CommonToken, TES4VariableDeclaration>)((CommonToken variableDeclarationType, CommonToken variableName)=>
            {
                return new TES4VariableDeclaration((string)variableName.Value, TES4Type.GetFirst((string)variableDeclarationType.Value.ToLower()));
            }) )

            ;
            __invoke("Block+").Is("Block+", "Block").Call((TES4BlockList list, TES4CodeBlock blockDeclaration)=>
            {
                list.Add(blockDeclaration);
                return list;
            } ) .

            Is("Block").Call((TES4CodeBlock blockDeclaration)=>
            {
                TES4BlockList list = new TES4BlockList();
                list.Add(blockDeclaration);
                return list;
            } )

            ;
            __invoke("Block").Is("BlockStart", "BlockType", "BlockParameter+", "Code+", "BlockEnd").Call((System.Func<object, CommonToken, TES4BlockParameterList, TES4CodeChunks, object, TES4CodeBlock>)((object blockStart, CommonToken blockType, TES4BlockParameterList blockParameters, TES4CodeChunks codeChunks, object blockEnd) =>
            {
                return new TES4CodeBlock((string)blockType.Value, blockParameters, codeChunks);
            }) ) .

            Is("BlockStart", "BlockType", "BlockParameter+", "BlockEnd").Call((System.Func<object, CommonToken, TES4BlockParameterList, object, TES4CodeBlock>)((object blockStart, CommonToken blockType, TES4BlockParameterList blockParameters, object blockEnd)=>
            {
                return new TES4CodeBlock((string)blockType.Value, blockParameters, null);
            }) ) //rare empty block
            .

            Is("BlockStart", "BlockType", "Code+", "BlockEnd").Call((System.Func<object, CommonToken, TES4CodeChunks, object, TES4CodeBlock>)((object blockStart, CommonToken blockType, TES4CodeChunks codeChunks, object blockEnd) =>
            {
                return new TES4CodeBlock((string)blockType.Value, null, codeChunks);
            }) ) .

            Is("BlockStart", "BlockType", "BlockEnd").Call((System.Func<object, CommonToken, object, TES4CodeBlock>)((object blockStart, CommonToken blockType, object blockEnd) =>
            {
                return new TES4CodeBlock((string)blockType.Value, null, null);
            }) )

            ; //rare empty block
            __invoke("BlockParameter+").Is("BlockParameter+", "BlockParameter").Call((TES4BlockParameterList list, TES4BlockParameter blockParameter)=>
            {
                list.Add(blockParameter);
                return list;
            } ) .

            Is("BlockParameter").Call((TES4BlockParameter blockParameter)=>
            {
                TES4BlockParameterList block = new TES4BlockParameterList();
                block.Add(blockParameter);
                return block;
            } )

            ;
            __invoke("BlockParameter").Is("BlockParameterToken").Call((System.Func<CommonToken, TES4BlockParameter>)((CommonToken token)=>
            {
                return new TES4BlockParameter((string)token.Value);
            }) )

            ;
            this.CreateObscriptCodeParsingTree();
            this.Start("Script");
        }
    }
}