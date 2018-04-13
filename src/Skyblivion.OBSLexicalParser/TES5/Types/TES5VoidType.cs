using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5VoidType : ITES5Type
    {
        public ITES5Type NativeType { get => throw new ConversionException("VOID TYPE get native type"); set => throw new ConversionException("Cannot set native type void type."); }

        public string Value => "";

        public IEnumerable<string> Output => throw new ConversionException("VOID TYPE value output");

        public bool IsPrimitive => false;

        public bool IsNativePapyrusType => false;

        public string OriginalName => "void";
    }
}