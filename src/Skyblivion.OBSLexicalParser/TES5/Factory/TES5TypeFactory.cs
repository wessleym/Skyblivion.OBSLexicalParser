using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5TypeFactory
    {
        public const string TES4Prefix = "TES4";
        public static ITES5Type Void()
        {
            return MemberByValue("void");
        }

        /*
        * @param memberByValue string Type to be created.
        * @param basicType TES5BasicType You might override the basic type for this custom type created.
        */
        public static ITES5Type MemberByValue(string memberByValue, ITES5Type basicType = null)
        {
            if (memberByValue == null) { throw new ArgumentNullException(nameof(memberByValue)); }
            if (memberByValue == "void") { return new TES5VoidType(); }
            TES5BasicType tes5BasicType = TES5BasicType.GetFirstOrNull(PHPFunction.UCWords(memberByValue));
            if (tes5BasicType != null) { return tes5BasicType; }
            //Ugly - todo: REFACTOR THIS TO NON-STATIC CLASS AND MOVE THIS TO DI
            if (basicType == null)
            {
                ESMAnalyzer analyzer = ESMAnalyzer._instance();
                basicType = analyzer.GetScriptType(memberByValue);
            }
            return new TES5CustomType(memberByValue, TES4Prefix, basicType);
        }
    }
}