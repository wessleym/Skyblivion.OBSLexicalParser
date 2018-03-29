using Skyblivion.OBSLexicalParser.TES5.AST;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    interface ITES5Type : ITES5Outputtable
    {
        //Get native type on which this type is based
        //If this type is native, it will return itself.
        ITES5Type getNativeType();
        void setNativeType(ITES5Type basicType);
        string value();
        bool isPrimitive();
        /*
        * Is this type a native papyrus type ( the one defined by skyrim itself ) or a custom script?
        */
        bool isNativePapyrusType();
        string getOriginalName();//WTM:  Change:  I added this.
    }
}