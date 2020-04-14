using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5VariableAssignationFactory
    {
        public static TES5VariableAssignation CreateAssignation(ITES5Value reference, ITES5Value value)
        {
#if ALTERNATE_TYPE_MAPPING
            if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(reference.TES5Type, TES5BasicType.T_ACTIVATOR) && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(value.TES5Type, TES5BasicType.T_ACTIVATOR))
            {
                TES5Reference? valueReference = value as TES5Reference;
                if (valueReference != null)
                {
                    valueReference.ReferencesTo.TES5Type.NativeType = TES5BasicType.T_OBJECTREFERENCE;//This needs to be an ObjectReference to prevent compilation problems.
                }
                TES5SelfReference? valueSelfReference = value as TES5SelfReference;
                if (valueSelfReference != null)
                {
                    valueSelfReference.ReferencesTo.TES5Type.NativeType = TES5BasicType.T_OBJECTREFERENCE;//This needs to be an ObjectReference to prevent compilation problems.
                }
            }
            else if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(reference.TES5Type, TES5BasicType.T_DOOR) && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(value.TES5Type, TES5BasicType.T_DOOR))
            {
                TES5SelfReference? valueSelfReference = value as TES5SelfReference;
                if (valueSelfReference != null)
                {
                    valueSelfReference.ReferencesTo.TES5Type.NativeType = reference.TES5Type.NativeType;
                }
            }
            else if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(reference.TES5Type, TES5BasicType.T_STATIC) && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(value.TES5Type, TES5BasicType.T_STATIC))
            {
                TES5Reference? valueReference = value as TES5Reference;
                if (valueReference != null)
                {
                    valueReference.ReferencesTo.TES5Type.NativeType = reference.TES5Type.NativeType;
                }
            }
#endif
            return new TES5VariableAssignation(reference, value);
        }
    }
}