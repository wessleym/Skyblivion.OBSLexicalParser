using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptHeader
    {
        public string OriginalScriptName { get; }
        public string EscapedScriptName { get; }
        public ITES5Type ScriptType { get; }
        private readonly string scriptNamePrefix;
        public bool IsHidden { get; }
        public string EDID { get; }
        public TES5ScriptHeader(string scriptName, ITES5Type type, string scriptNamePrefix, bool isHidden)
        {
            this.OriginalScriptName = scriptName;
            this.EscapedScriptName = NameTransformer.Limit(scriptName, scriptNamePrefix);
            this.EDID = scriptName;
            this.scriptNamePrefix = scriptNamePrefix;
            this.ScriptType = type;
            this.IsHidden = isHidden;
        }

        public string PrefixedScriptName => this.scriptNamePrefix + this.EscapedScriptName;

        public string GetOutput(bool anyConditionalProperties)
        {
            return "ScriptName " + PrefixedScriptName + " Extends " + this.ScriptType.NativeType.Name + (this.IsHidden ? " Hidden" : (anyConditionalProperties ? " Conditional" : ""));
        }

        /*
             * @throws ConversionException
        */
        public void SetNativeType(ITES5Type scriptType, bool ensureNewTypeExtendsCurrentType = true)
        {
            if (ensureNewTypeExtendsCurrentType && !TES5InheritanceGraphAnalyzer.IsExtending(scriptType, this.ScriptType.NativeType))
            {
                throw new ConversionException("Cannot set script type to non-extending type - current native type " + this.ScriptType.NativeType.Value + ", new type " + scriptType.Value);
            }
            this.ScriptType.NativeType = scriptType.NativeType;
        }
    }
}