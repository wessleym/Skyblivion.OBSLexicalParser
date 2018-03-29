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
            __invoke("Script")._is("ScriptHeader", "Block+").call((TES4ScriptHeader header, TES4BlockList blockList)=>
            {
                return new TES4Script(header, null, blockList);
            } ) .

            _is("ScriptHeader", "VariableDeclaration+").call((TES4ScriptHeader header, TES4VariableDeclarationList variableList)=>
            {
                return new TES4Script(header, variableList, null);
            } ) .

            _is("ScriptHeader", "VariableDeclaration+", "Block+").call((TES4ScriptHeader header, TES4VariableDeclarationList variableList, TES4BlockList blockList)=>
            {
                return new TES4Script(header, variableList, blockList);
            } )

            ;
            __invoke("ScriptHeader")._is("ScriptHeaderToken", "ScriptName").call((object headerToken, CommonToken scriptName)=>
            {
                return new TES4ScriptHeader(scriptName.getValue());
            } )

            ;
            __invoke("VariableDeclaration+")._is("VariableDeclaration+", "VariableDeclaration").call((TES4VariableDeclarationList list, TES4VariableDeclaration variableDeclaration)=>
            {
                list.add(variableDeclaration);
                return list;
            } ) .

            _is("VariableDeclaration").call((TES4VariableDeclaration variableDeclaration)=>
            {
                TES4VariableDeclarationList list = new TES4VariableDeclarationList();
                list.add(variableDeclaration);
                return list;
            } )

            ;
            __invoke("VariableDeclaration")._is("VariableDeclarationType", "VariableName").call((CommonToken variableDeclarationType, CommonToken variableName)=>
            {
                return new TES4VariableDeclaration(variableName.getValue(), TES4Type.GetFirst(variableDeclarationType.getValue().ToLower()));
            } )

            ;
            __invoke("Block+")._is("Block+", "Block").call((TES4BlockList list, TES4CodeBlock blockDeclaration)=>
            {
                list.add(blockDeclaration);
                return list;
            } ) .

            _is("Block").call((TES4CodeBlock blockDeclaration)=>
            {
                TES4BlockList list = new TES4BlockList();
                list.add(blockDeclaration);
                return list;
            } )

            ;
            __invoke("Block")._is("BlockStart", "BlockType", "BlockParameter+", "Code+", "BlockEnd").call((object blockStart, CommonToken blockType, TES4BlockParameterList blockParameters, TES4CodeChunks codeChunks, object blockEnd) =>
            {
                return new TES4CodeBlock(blockType.getValue(), blockParameters, codeChunks);
            } ) .

            _is("BlockStart", "BlockType", "BlockParameter+", "BlockEnd").call((object blockStart, CommonToken blockType, TES4BlockParameterList blockParameters, object blockEnd)=>
            {
                return new TES4CodeBlock(blockType.getValue(), blockParameters, null);
            } ) //rare empty block
            .

            _is("BlockStart", "BlockType", "Code+", "BlockEnd").call((object blockStart, CommonToken blockType, TES4CodeChunks codeChunks, object blockEnd) =>
            {
                return new TES4CodeBlock(blockType.getValue(), null, codeChunks);
            } ) .

            _is("BlockStart", "BlockType", "BlockEnd").call((object blockStart, CommonToken blockType, object blockEnd) =>
            {
                return new TES4CodeBlock(blockType.getValue(), null, null);
            } )

            ; //rare empty block
            __invoke("BlockParameter+")._is("BlockParameter+", "BlockParameter").call((TES4BlockParameterList list, TES4BlockParameter blockParameter)=>
            {
                list.add(blockParameter);
                return list;
            } ) .

            _is("BlockParameter").call((TES4BlockParameter blockParameter)=>
            {
                TES4BlockParameterList block = new TES4BlockParameterList();
                block.add(blockParameter);
                return block;
            } )

            ;
            __invoke("BlockParameter")._is("BlockParameterToken").call((CommonToken token)=>
            {
                return new TES4BlockParameter(token.getValue());
            } )

            ;
            this.createObscriptCodeParsingTree();
            this.start("Script");
        }
    }
}