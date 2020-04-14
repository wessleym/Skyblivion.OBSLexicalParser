using Skyblivion.ESReader.PHP;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    static class NameTransformer
    {
        private const int maxLength = 38;//Cannot have more than 38 characters
        public static string Limit(string originalName, string prefix)
        {
            string fullName = prefix + originalName;
            if (fullName.Length > maxLength)
            {
                return GetEscapedName(originalName, prefix, false);
            }
            else
            {
                return originalName;
            }
        }

        public static string GetEscapedName(string name, string prefix, bool includePrefix)
        {
            string newName = PHPFunction.MD5(name.ToLower());//Some names are normal, and others are lowercase.  ToLower() homogenizes them.
            string fullName = prefix + newName;
            if (fullName.Length > maxLength)
            {
                int newNameLength = newName.Length - (fullName.Length - maxLength);
                newName = newName.Substring(0, newNameLength);
                fullName = prefix + newName;
            }
            return includePrefix ? fullName : newName;
        }
    }
}