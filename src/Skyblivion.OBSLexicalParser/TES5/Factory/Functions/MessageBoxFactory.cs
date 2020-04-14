using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MessageBoxFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public MessageBoxFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
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
                TES5StaticReference calledOnRef = TES5StaticReferenceFactory.Debug;
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