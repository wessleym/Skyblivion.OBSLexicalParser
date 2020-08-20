using Skyblivion.OBSLexicalParser.TES5.AST;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    interface ITES5Type : ITES5Outputtable
    {
        //Get native type on which this type is based
        //If this type is native, it will return itself.
        TES5BasicType NativeType { get; set; }
        string Value { get; }
        bool IsPrimitive { get; }
        bool AllowInference { get; }//WTM:  Change:  Added
        bool AllowNativeTypeInference { get; }//WTM:  Change:  Added
        /*
        * Is this type a native papyrus type ( the one defined by skyrim itself ) or a custom script?
        */
        bool IsNativePapyrusType { get; }
        string OriginalName { get; }//WTM:  Change:  Added
    }
}