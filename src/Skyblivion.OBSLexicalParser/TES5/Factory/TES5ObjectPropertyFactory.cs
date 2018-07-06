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

        public TES5ObjectProperty CreateObjectProperty(TES5MultipleScriptsScope multipleScriptsScope, ITES5Referencer reference, string propertyName)
        {
            ITES5VariableOrProperty referencesTo = reference.ReferencesTo;
            this.typeInferencer.InferenceVariableByReferenceEdid(referencesTo, multipleScriptsScope);
            TES5Property remoteProperty = multipleScriptsScope.GetPropertyFromScript(referencesTo.TES5Type.OriginalName, propertyName);
            TES5ObjectProperty objectProperty = new TES5ObjectProperty(reference, remoteProperty);
            return objectProperty;
        }
    }
}