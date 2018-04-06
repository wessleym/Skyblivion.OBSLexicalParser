using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5VoidType : ITES5Type
    {
        public ITES5Type getNativeType()
        {
            throw new ConversionException("VOID TYPE get native type");
        }

        public string value()
        {
            return "";
        }

        public IEnumerable<string> Output => throw new ConversionException("VOID TYPE value output");

        public bool isPrimitive()
        {
            return false;
        }

        public bool isNativePapyrusType()
        {
            return false;
        }

        public void setNativeType(ITES5Type basicType)
        {
            throw new ConversionException("Cannot set native type void type.");
        }

        public string getOriginalName()
        {
            return "void";
        }
    }
}