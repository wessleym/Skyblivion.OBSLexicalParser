using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5TypeFactory
    {
        private static string scriptsPrefix = "TES4";
        public static ITES5Type _void()
        {
            return memberByValue("void");
        }

        /*
        * @param memberByValue string Type to be created.
        * @param basicType TES5BasicType You might override the basic type for this custom type created.
        * @return \Eloquent\Enumeration\ValueMultitonInterface|TES5CustomType|TES5VoidType
        */
        public static ITES5Type memberByValue(string memberByValue, ITES5Type basicType = null)
        {
            if (memberByValue == "void")
            {
                return new TES5VoidType();
            }

            try
            {
                return TES5BasicType.GetFirst(PHPFunction.UCWords(memberByValue));
            }
            catch (Exception)
            {
                //Ugly - todo: REFACTOR THIS TO NON-STATIC CLASS AND MOVE THIS TO DI
                if (basicType == null)
                {
                    ESMAnalyzer analyzer = ESMAnalyzer._instance();
                    basicType = analyzer.getScriptType(memberByValue);
                }

                return new TES5CustomType(TES5NameTransformer.transform(memberByValue, scriptsPrefix), scriptsPrefix, memberByValue, basicType);
            }
        }
    }
}