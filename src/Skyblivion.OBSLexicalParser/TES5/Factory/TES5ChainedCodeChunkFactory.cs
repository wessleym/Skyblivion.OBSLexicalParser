using Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ChainedCodeChunkFactory : ITES5CodeChunkFactory
    {
        private TES5BranchFactory branchFactory;
        private TES5VariableAssignationConversionFactory assignationFactory;
        private TES5ReturnFactory returnFactory;
        private TES5ValueFactory objectCallFactory;
        private TES5LocalVariableListFactory localVariableListFactory;
        public TES5ChainedCodeChunkFactory(TES5ValueFactory objectCallFactory, TES5ReturnFactory returnFactory, TES5VariableAssignationConversionFactory assignationFactory, TES5BranchFactory branchFactory, TES5LocalVariableListFactory localVariableListFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.returnFactory = returnFactory;
            this.assignationFactory = assignationFactory;
            this.branchFactory = branchFactory;
            this.localVariableListFactory = localVariableListFactory;
            branchFactory.setCodeChunkFactory(this); //ugly!!!
        }

        public TES5CodeChunkCollection createCodeChunk(ITES4CodeChunk chunk, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4Branch branch = chunk as TES4Branch;
            if (branch != null) { return this.branchFactory.createCodeChunk(branch, codeScope, globalScope, multipleScriptsScope); }
            TES4Return returnChunk = chunk as TES4Return;
            if (returnChunk != null) { return this.returnFactory.createCodeChunk(codeScope.getLocalScope().getFunctionScope(), globalScope, multipleScriptsScope); }
            ITES4Callable callable = chunk as ITES4Callable;
            if (callable != null) { return this.objectCallFactory.createCodeChunk(callable, codeScope, globalScope, multipleScriptsScope); }
            TES4VariableAssignation assignation = chunk as TES4VariableAssignation;
            if (assignation != null) { return this.assignationFactory.createCodeChunk(assignation, codeScope, globalScope, multipleScriptsScope); }
            TES4VariableDeclarationList declarationList = chunk as TES4VariableDeclarationList;
            if (declarationList != null) { return this.localVariableListFactory.createCodeChunk(declarationList, codeScope); }
            throw new ConversionException("Cannot convert a chunk: " + chunk.GetType().FullName);
        }
    }
}