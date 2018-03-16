using Skyblivion.ESReader.PHP;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class TES5NameTransformer
    {
        public static string transform(string originalName, string prefix = "")
        {
            if ((prefix+originalName).Length > 38)
            { //Cannot have more than 38 characters..
                return PHPFunction.MD5(originalName.ToLower());
            }
            else
            {
                return originalName;
            }
        }
    }
}