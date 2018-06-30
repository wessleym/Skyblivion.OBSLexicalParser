using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptHeader : ITES5Outputtable
    {
        public string OriginalScriptName { get; private set; }
        public string EscapedScriptName { get; private set; }
        public ITES5Type ScriptType { get; private set; }
        /*
        * The basic script type this script header was constructed
         * Used for resolving implicit references.
        */
        public ITES5Type BasicScriptType { get; private set; }
        private string scriptNamePrefix;
        private bool isHidden;
        public string EDID { get; private set; }
        public TES5ScriptHeader(string scriptName, ITES5Type scriptType, string scriptNamePrefix, bool isHidden = false)
        {
            this.OriginalScriptName = scriptName;
            this.EscapedScriptName = NameTransformer.Limit(scriptName, scriptNamePrefix);
            this.EDID = scriptName;
            this.scriptNamePrefix = scriptNamePrefix;
            this.ScriptType = TES5TypeFactory.MemberByValue(scriptName, scriptType);
            this.BasicScriptType = scriptType;
            this.isHidden = isHidden;
        }

        public IEnumerable<string> Output => new List<string>() { "ScriptName " + this.scriptNamePrefix + this.EscapedScriptName + " extends " + this.ScriptType.NativeType.Output.Single() + " " + (this.isHidden ? "Hidden" : "Conditional") };

        /*
             * @throws ConversionException
        */
        public void setNativeType(ITES5Type scriptType)
        {
            if (!TES5InheritanceGraphAnalyzer.isExtending(scriptType, this.ScriptType.NativeType))
            {
                throw new ConversionException("Cannot set script type to non-extending type - current native type " + this.ScriptType.NativeType.Value+ ", new type " + scriptType.Value);
            }

            this.ScriptType.NativeType = scriptType.NativeType;
        }
    }
}