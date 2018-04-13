using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5PrimitiveValueFactory
    {
        public static ITES5Primitive createValue(ITES4Primitive value)
        {
            TES4Type valueType = value.Type;
            object valueData = value.Data;
            if (valueType == TES4Type.T_INT)
            {
                return new TES5Integer((int)valueData);
            }
            else if (valueType == TES4Type.T_STRING)
            {
                return new TES5String((string)valueData);
            }
            else if (valueType == TES4Type.T_FLOAT)
            {
                return new TES5Float((float)valueData);
            }
            throw new ConversionException("Unknown value type to be factored from " + value.GetType().FullName);
        }

        public static TES5ConcatenatedValue createConcatenatedValue(IList<ITES5Value> concatenateValues)
        {
            return new TES5ConcatenatedValue(concatenateValues);
        }
    }
}