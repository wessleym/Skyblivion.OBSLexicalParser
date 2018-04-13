using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectPropertyFactory
    {
        private TES5TypeInferencer typeInferencer;
        public TES5ObjectPropertyFactory(TES5TypeInferencer typeInferencer)
        {
            this.typeInferencer = typeInferencer;
        }

        public TES5ObjectProperty createObjectProperty(TES5MultipleScriptsScope multipleScriptsScope, ITES5Referencer reference, string propertyName)
        {
            ITES5Variable referencesTo = reference.ReferencesTo;
            this.typeInferencer.inferenceVariableByReferenceEdid(referencesTo, multipleScriptsScope);
            TES5Property remoteProperty = multipleScriptsScope.getPropertyFromScript(referencesTo.PropertyType.OriginalName, propertyName);
            TES5ObjectProperty objectProperty = new TES5ObjectProperty(reference, remoteProperty);
            return objectProperty;
        }
    }
}