using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    static class TES5ScriptHeaderFactory
    {
        private static readonly Dictionary<string, TES5ScriptHeader> cache = new Dictionary<string, TES5ScriptHeader>();
        private static TES5ScriptHeader Construct(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            return new TES5ScriptHeader(scriptName, type, scriptNamePrefix, isHidden);
        }

        public static TES5ScriptHeader GetFromCacheOrConstructByBasicType(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            string scriptNameLower = scriptName.ToLower();
            TES5ScriptHeader header;
            if (cache.TryGetValue(scriptNameLower, out header)) { return header; }
            TES5BasicType? basicType = type as TES5BasicType;
            if (basicType != null) { type = TES5TypeFactory.MemberByValue(scriptName, basicType, basicType.AllowInference); }
            header = Construct(scriptName, type, scriptNamePrefix, isHidden);
            cache.Add(scriptNameLower, header);
            return header;
        }
    }
}
