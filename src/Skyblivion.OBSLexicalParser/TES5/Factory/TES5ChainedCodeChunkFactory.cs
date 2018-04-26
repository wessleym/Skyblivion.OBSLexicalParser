using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ChainedCodeChunkFactory : ITES5CodeChunkFactory
    {
        private TES5VariableAssignationConversionFactory assignationFactory;
        private TES5ReturnFactory returnFactory;
        private TES5ValueFactory valueFactory;
        public TES5ChainedCodeChunkFactory(TES5ValueFactory valueFactory, TES5ReturnFactory returnFactory, TES5VariableAssignationConversionFactory assignationFactory)
        {
            this.valueFactory = valueFactory;
            this.returnFactory = returnFactory;
            this.assignationFactory = assignationFactory;
        }

        public TES5CodeChunkCollection createCodeChunk(ITES4CodeChunk chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Branch branch = chunk as TES4Branch;
            if (branch != null) { return TES5BranchFactory.CreateCodeChunk(branch, codeScope, globalScope, multipleScriptsScope, this, valueFactory); }
            TES4Return returnChunk = chunk as TES4Return;
            if (returnChunk != null) { return this.returnFactory.createCodeChunk(codeScope.LocalScope.FunctionScope, globalScope, multipleScriptsScope); }
            ITES4Callable callable = chunk as ITES4Callable;
            if (callable != null) { return this.valueFactory.createCodeChunks(callable, codeScope, globalScope, multipleScriptsScope); }
            TES4VariableAssignation assignation = chunk as TES4VariableAssignation;
            if (assignation != null) { return this.assignationFactory.createCodeChunk(assignation, codeScope, globalScope, multipleScriptsScope); }
            TES4VariableDeclarationList declarationList = chunk as TES4VariableDeclarationList;
            if (declarationList != null) { TES5LocalVariableListFactory.createCodeChunk(declarationList, codeScope); return null; }
            throw new ConversionException("Cannot convert a chunk: " + chunk.GetType().FullName);
        }
    }
}