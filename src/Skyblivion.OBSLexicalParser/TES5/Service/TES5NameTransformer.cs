using Skyblivion.ESReader.PHP;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    static class TES5NameTransformer
    {
        public static string TransformLongName(string originalName, string prefix = "")
        {
            if ((prefix+originalName).Length > 38)
            { //Cannot have more than 38 characters..
#if PHP_COMPAT
                return PHPFunction.MD5(originalName.ToLower());
#else
                return Math.Abs(originalName.ToLower().GetHashCode()).ToString();
#endif
            }
            else
            {
                return originalName;
            }
        }
    }
}