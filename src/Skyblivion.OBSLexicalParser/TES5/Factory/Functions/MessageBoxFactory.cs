using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MessageBoxFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public MessageBoxFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.getArguments();
            //todo Refactor - add floating point vars .
            if (functionArguments.Count== 1)
            {
                TES5StaticReference calledOnRef = TES5StaticReference.Debug;
                return this.objectCallFactory.CreateObjectCall(calledOnRef, "MessageBox", multipleScriptsScope, this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                List<string> messageArguments = new List<string>();
                string uniqueAddition = Math.Abs(functionArguments.GetHashCode()).ToString();//WTM:  Change:  PHPFunction.MD5(PHPFunction.Serialize(functionArguments.getValues()))
                string edid = "TES4MessageBox" + uniqueAddition;
                messageArguments.Add(edid);
                messageArguments.AddRange(functionArguments.Select(a => a.StringValue));
                this.metadataLogService.WriteLine("ADD_MESSAGE", messageArguments);
                ITES5Referencer messageBoxResult = this.referenceFactory.createReadReference(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST, globalScope, multipleScriptsScope, localScope);
                ITES5Referencer reference = this.referenceFactory.createReadReference(edid, globalScope, multipleScriptsScope, localScope);
                return this.assignationFactory.createAssignation(messageBoxResult, this.objectCallFactory.CreateObjectCall(reference, "show", multipleScriptsScope));
            }
        }
    }
}