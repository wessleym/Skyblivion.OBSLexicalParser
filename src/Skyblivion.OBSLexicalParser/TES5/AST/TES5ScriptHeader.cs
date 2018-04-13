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
        private ITES5Type scriptType;
        /*
        * The basic script type this script header was constructed
         * Used for resolving implicit references.
        */
        private ITES5Type basicScriptType;
        private TES5InheritanceGraphAnalyzer inheritanceAnalyzer;
        private string scriptNamePrefix;
        private bool isHidden;
        public string Edid { get; private set; }
        public TES5ScriptHeader(string scriptName, ITES5Type scriptType, string scriptNamePrefix, bool isHidden = false)
        {
            this.OriginalScriptName = scriptName;
            this.EscapedScriptName = NameTransformer.Limit(scriptName, scriptNamePrefix);
            this.Edid = scriptName;
            this.scriptNamePrefix = scriptNamePrefix;
            this.scriptType = TES5TypeFactory.memberByValue(scriptName, scriptType);
            this.basicScriptType = scriptType;
            this.isHidden = isHidden;
            this.inheritanceAnalyzer = new TES5InheritanceGraphAnalyzer();
        }

        public IEnumerable<string> Output => new List<string>() { "ScriptName " + this.scriptNamePrefix + this.EscapedScriptName + " extends " + this.scriptType.NativeType.Output.Single() + " " + (this.isHidden ? "Hidden" : "Conditional") };

        /*
             * @throws ConversionException
        */
        public void setNativeType(ITES5Type scriptType)
        {
            if (!TES5InheritanceGraphAnalyzer.isExtending(scriptType, this.scriptType.NativeType))
            {
                throw new ConversionException("Cannot set script type to non-extending type - current native type " + this.scriptType.NativeType.Value+ ", new type " + scriptType.Value);
            }

            this.scriptType.NativeType = scriptType.NativeType;
        }

        public ITES5Type getScriptType()
        {
            return this.scriptType;
        }

        public ITES5Type getBasicScriptType()
        {
            return this.basicScriptType;
        }
    }
}