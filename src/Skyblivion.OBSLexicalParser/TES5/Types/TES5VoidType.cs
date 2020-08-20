using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5VoidType : ITES5Type
    {
        private TES5VoidType() { }

        TES5BasicType ITES5Type.NativeType { get => throw new ConversionException("VOID TYPE get native type"); set => throw new ConversionException("Cannot set native type void type."); }

        public string Value => "";

        public IEnumerable<string> Output => throw new ConversionException("VOID TYPE value output");

        public bool IsPrimitive => false;

        public bool IsNativePapyrusType => false;

        public bool AllowInference => false;

        public bool AllowNativeTypeInference => AllowInference;

        public string OriginalName => OriginalNameConst;

        public const string OriginalNameConst = "void";//WTM:  Change:  Added
        public static readonly TES5VoidType Instance = new TES5VoidType();//WTM:  Change:  Added
    }
}