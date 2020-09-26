using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class AddTopicFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly PopCalledRenameFunctionFactory popCalledRenameFunctionFactory;
        private readonly TES4TopicsToTES5GlobalVariableFinder globalVariableFinder;
        public AddTopicFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            popCalledRenameFunctionFactory = new PopCalledRenameFunctionFactory("Add", referenceFactory, objectCallFactory, objectCallArgumentsFactory);
            globalVariableFinder = new TES4TopicsToTES5GlobalVariableFinder();
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Referencer topic = referenceFactory.CreateReference(function.Arguments[0].StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope);
            TES5Property property = (TES5Property)topic.ReferencesTo!;
            Tuple<int, string>? newGlobalVariable;
            if (globalVariableFinder.TryGetGlobalVariable(property.TES4FormID!.Value, out newGlobalVariable))
            {
                TES5Property globalVariableProperty = TES5PropertyFactory.ConstructWithTES5FormID(newGlobalVariable.Item2, TES5BasicType.T_GLOBALVARIABLE, newGlobalVariable.Item2, newGlobalVariable.Item1);
                globalScope.AddPropertyIfNotExists(globalVariableProperty);
                TES5Reference globalVariableReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(globalVariableProperty);
                return objectCallFactory.CreateObjectCall(globalVariableReference, "SetValueInt", new TES5ObjectCallArguments() { new TES5Integer(1) });
            }
            return popCalledRenameFunctionFactory.ConvertFunction(calledOn, function, codeScope, globalScope, multipleScriptsScope);
        }
    }
}
