using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5TypeFactory
    {
        public const string TES4Prefix = "TES4";
        public static ITES5Type Void(ESMAnalyzer esmAnalyzer)
        {
            return MemberByValue(TES5VoidType.OriginalNameConst, null, esmAnalyzer);
        }

        /*
        * @param memberByValue string Type to be created.
        * @param basicType TES5BasicType You might override the basic type for this custom type created.
        */
        public static ITES5Type MemberByValue(string memberByValue, ITES5Type? basicType, ESMAnalyzer esmAnalyzer)
        {
            if (memberByValue == TES5VoidType.OriginalNameConst) { return new TES5VoidType(); }
            TES5BasicType tes5BasicType = TES5BasicType.GetFirstOrNull(PHPFunction.UCWords(memberByValue));
            if (tes5BasicType != null) { return tes5BasicType; }
            if (basicType == null)
            {
                basicType = esmAnalyzer.GetScriptType(memberByValue);
            }
            return new TES5CustomType(memberByValue, TES4Prefix, basicType);
        }
    }
}