using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5TypeFactory
    {
        public const string TES4Prefix = "TES4";

        private static ITES5Type? MemberByValue(string typeName
#if ALTERNATE_TYPE_MAPPING
            , bool mayRevertToForm
#endif
            )
        {
            if (typeName == TES5VoidType.OriginalNameConst) { return TES5VoidType.Instance; }
            TES5BasicType? basicTypeFromName = TES5BasicType.GetFirstOrNullCaseInsensitive(typeName);
            return basicTypeFromName
#if ALTERNATE_TYPE_MAPPING
                != null ? (mayRevertToForm ? new TES5BasicTypeRevertible(basicTypeFromName) : (ITES5Type)basicTypeFromName) : null
#endif
            ;
        }
        private static ITES5Type MemberByValue(string typeName, Func<TES5BasicType> basicTypeFunc,
#if !ALTERNATE_TYPE_MAPPING
            bool allowNativeTypeInferenceForCustomTypes
#else
            bool mayRevertToForm
#endif
            )
        {
            ITES5Type? typeByName = MemberByValue(typeName
#if ALTERNATE_TYPE_MAPPING
            , mayRevertToForm
#endif
            );
            if (typeByName != null) { return typeByName; }
            TES5BasicType customTypeBase = basicTypeFunc();
#if ALTERNATE_TYPE_MAPPING
            ITES5Type baseType = customTypeBase;
            if (mayRevertToForm)
            {
                baseType = new TES5BasicTypeRevertible(customTypeBase);
            }
#endif
            return new TES5CustomType(typeName, TES4Prefix,
#if !ALTERNATE_TYPE_MAPPING
                customTypeBase, allowNativeTypeInferenceForCustomTypes
#else
                baseType
#endif
                );
        }
        /*
        * @param memberByValue string Type to be created.
        * @param basicType TES5BasicType You might override the basic type for this custom type created.
        */
        public static ITES5Type MemberByValue(string typeName, TES5BasicType basicType, bool allowNativeTypeInferenceForCustomTypes)
        {
            return MemberByValue(typeName, () => basicType, allowNativeTypeInferenceForCustomTypes);
        }
        public static ITES5Type MemberByValue(string typeName, ESMAnalyzer esmAnalyzer)
        {
            return MemberByValue(typeName, () => esmAnalyzer.GetScriptNativeTypeByScriptNameFromCache(typeName), true);
        }
    }
}