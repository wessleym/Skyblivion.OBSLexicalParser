using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MessageBoxFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public MessageBoxFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //todo Refactor - add floating point vars .
            if (functionArguments.Count== 1)
            {
                TES5StaticReference calledOnRef = TES5StaticReference.Debug;
                return this.objectCallFactory.CreateObjectCall(calledOnRef, "MessageBox", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                string edid = NameTransformer.GetEscapedName(string.Join("", functionArguments.Select(v => v.StringValue)), TES5TypeFactory.TES4Prefix + "MessageBox_", true);//WTM:  Change:  PHPFunction.MD5(PHPFunction.Serialize(functionArguments.getValues()))
                IEnumerable<string> messageArguments = (new string[] { edid }).Concat(functionArguments.Select(a => a.StringValue));
                this.metadataLogService.WriteLine("ADD_MESSAGE", messageArguments);
                ITES5Referencer messageBoxResult = this.referenceFactory.CreateReadReference(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST, globalScope, multipleScriptsScope, localScope);
                ITES5Referencer reference = this.referenceFactory.CreateReadReference(edid, globalScope, multipleScriptsScope, localScope);
                return TES5VariableAssignationFactory.CreateAssignation(messageBoxResult, this.objectCallFactory.CreateObjectCall(reference, "show"));
            }
        }
    }
}