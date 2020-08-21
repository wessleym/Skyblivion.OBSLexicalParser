using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectPropertyFactory
    {
        private readonly TES5TypeInferencer typeInferencer;
        public TES5ObjectPropertyFactory(TES5TypeInferencer typeInferencer)
        {
            this.typeInferencer = typeInferencer;
        }

        public TES5ObjectProperty CreateObjectProperty(ITES5Referencer reference, string propertyName, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5VariableOrProperty? referencesTo = reference.ReferencesTo;
            if (referencesTo == null) { throw new NullableException(nameof(referencesTo)); }
            this.typeInferencer.InferenceVariableByReferenceEdid(referencesTo, multipleScriptsScope);
            TES5Property remoteProperty = multipleScriptsScope.GetPropertyFromScript(referencesTo.TES5Type.OriginalName, propertyName);
            TES5ObjectProperty objectProperty = new TES5ObjectProperty(reference, remoteProperty);
            return objectProperty;
        }

        public TES5ObjectProperty CreateObjectProperty(string parentReferenceName, string childReferenceName, TES5ReferenceFactory referenceFactory, TES5LocalScope localScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Referencer parentReference = referenceFactory.CreateReference(parentReferenceName, globalScope, multipleScriptsScope, localScope);
            TES5ObjectProperty childReference = CreateObjectProperty(parentReference, childReferenceName, multipleScriptsScope);//Todo rethink the prefix adding
            return childReference;
        }
    }
}