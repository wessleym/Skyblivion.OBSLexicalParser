using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    static class TES5ScriptHeaderFactory
    {
        private static readonly Dictionary<string, TES5ScriptHeader> cache = new Dictionary<string, TES5ScriptHeader>(StringComparer.OrdinalIgnoreCase);
        private static TES5ScriptHeader Construct(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            return new TES5ScriptHeader(scriptName, type, scriptNamePrefix, isHidden);
        }

        public static TES5ScriptHeader? TryGetScriptHeader(string scriptName)
        {
            TES5ScriptHeader? header;
            if (cache.TryGetValue(scriptName, out header)) { return header; }
            return null;
        }

        public static TES5ScriptHeader GetFromCacheOrConstructByBasicType(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            TES5ScriptHeader? header = TryGetScriptHeader(scriptName);
            if (header != null) { return header; }
            TES5BasicType? basicType = type as TES5BasicType;
            if (basicType != null) { type = TES5TypeFactory.MemberByValue(scriptName, basicType); }
            header = Construct(scriptName, type, scriptNamePrefix, isHidden);
            cache.Add(scriptName, header);
            return header;
        }
    }
}
