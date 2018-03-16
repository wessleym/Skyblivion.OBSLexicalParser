using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5VariableAssignationFactory
    {
        private TES5ReferenceFactory referenceFactory;
        public TES5VariableAssignationFactory(TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
        }

        public TES5VariableAssignation createAssignation(ITES5Referencer target, ITES5Value value)
        {
            return new TES5VariableAssignation(target, value);
        }
    }
}