using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptHeader : ITES5Outputtable
    {
        public string OriginalScriptName { get; }
        public string EscapedScriptName { get; }
        public ITES5Type ScriptType { get; }
        private readonly string scriptNamePrefix;
        private readonly bool isHidden;
        public string EDID { get; }
        public TES5ScriptHeader(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            this.OriginalScriptName = scriptName;
            this.EscapedScriptName = NameTransformer.Limit(scriptName, scriptNamePrefix);
            this.EDID = scriptName;
            this.scriptNamePrefix = scriptNamePrefix;
            this.ScriptType = type;
            this.isHidden = isHidden;
        }

        public IEnumerable<string> Output => new string[] { "ScriptName " + this.scriptNamePrefix + this.EscapedScriptName + " extends " + this.ScriptType.NativeType.Output.Single() + " " + (this.isHidden ? "Hidden" : "Conditional") };

        /*
             * @throws ConversionException
        */
        public void SetNativeType(ITES5Type scriptType, bool ensureNewTypeExtendsCurrentType = true)
        {
            if (ensureNewTypeExtendsCurrentType && !TES5InheritanceGraphAnalyzer.IsExtending(scriptType, this.ScriptType.NativeType))
            {
                throw new ConversionException("Cannot set script type to non-extending type - current native type " + this.ScriptType.NativeType.Value+ ", new type " + scriptType.Value);
            }
            this.ScriptType.NativeType = scriptType.NativeType;
        }
    }
}