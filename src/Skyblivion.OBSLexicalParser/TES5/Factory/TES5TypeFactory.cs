using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5TypeFactory
    {
        public const string
            TES4Prefix = "TES4",
            TES4_Prefix = "TES4_",
            SKYBPrefix = "SKYB";

        private static ITES5Type? MemberByValue(string typeName)
        {
            if (typeName == TES5VoidType.OriginalNameConst) { return TES5VoidType.Instance; }
            TES5BasicType? basicTypeFromName = TES5BasicType.GetFirstOrNullCaseInsensitive(typeName);
            return basicTypeFromName;
        }
        private static ITES5Type MemberByValue(string typeName, Func<TES5BasicType> basicTypeFunc)
        {
            ITES5Type? typeByName = MemberByValue(typeName);
            if (typeByName != null) { return typeByName; }
            TES5BasicType customTypeBase = basicTypeFunc();
            return new TES5CustomType(typeName, TES4Prefix, customTypeBase);
        }
        /*
        * @param memberByValue string Type to be created.
        * @param basicType TES5BasicType You might override the basic type for this custom type created.
        */
        public static ITES5Type MemberByValue(string typeName, TES5BasicType basicType)
        {
            return MemberByValue(typeName, () => basicType);
        }
    }
}