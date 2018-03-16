using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5PrimitiveValueFactory
    {
        /*
         * @throws \Ormin\OBSLexicalParser\TES5\Exception\ConversionException
        */
        public ITES5Primitive createValue(ITES4Primitive value)
        {
            TES4Type valueType = value.getType();
            object valueData = value.getData();
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

        public TES5ConcatenatedValue createConcatenatedValue(TES5String[] concatenateValues)
        {
            return new TES5ConcatenatedValue(concatenateValues);
        }
    }
}