using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
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
        private readonly MessageBoxData messageBoxData;
        public const string MessageBoxPrefix = TES5TypeFactory.TES4Prefix + "MessageBox";
        public MessageBoxFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
            this.messageBoxData = new MessageBoxData();
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //todo Refactor - add floating point vars .
            if (functionArguments.Count == 1)
            {
                TES5StaticReference calledOnRef = TES5StaticReferenceFactory.Debug;
                return this.objectCallFactory.CreateObjectCall(calledOnRef, "MessageBox", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                string[] stringArguments = functionArguments.Select(v => v.StringValue).ToArray();
                string edid = messageBoxData.GetEDID(stringArguments);
                IEnumerable<string> messageArguments = (new string[] { edid }).Concat(functionArguments.Select(a => a.StringValue));
                this.metadataLogService.WriteLine("ADD_MESSAGE", messageArguments);
                Nullable<int> tes5FormIDNullable = messageBoxData.GetTES5FormID(edid);
                TES5Property messageBoxProperty = TES5PropertyFactory.ConstructWithTES5FormID(edid, TES5BasicType.T_MESSAGE, edid, tes5FormIDNullable);
                globalScope.AddProperty(messageBoxProperty);
                ITES5Referencer messageBoxReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(messageBoxProperty);
                TES5ObjectCall messageBoxShow = this.objectCallFactory.CreateObjectCall(messageBoxReference, "Show");
                ITES5Referencer messageBoxResult = this.referenceFactory.CreateReadReference(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST, globalScope, multipleScriptsScope, localScope);
                return TES5VariableAssignationFactory.CreateAssignation(messageBoxResult, messageBoxShow);
            }
        }
    }
}