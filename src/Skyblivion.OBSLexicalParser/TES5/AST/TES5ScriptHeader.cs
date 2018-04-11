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
        private string edid;
        public TES5ScriptHeader(string scriptName, ITES5Type scriptType, string scriptNamePrefix, bool isHidden = false)
        {
            this.OriginalScriptName = scriptName;
            this.EscapedScriptName = NameTransformer.Limit(scriptName, scriptNamePrefix);
            this.edid = scriptName;
            this.scriptNamePrefix = scriptNamePrefix;
            this.scriptType = TES5TypeFactory.memberByValue(scriptName, scriptType);
            this.basicScriptType = scriptType;
            this.isHidden = isHidden;
            this.inheritanceAnalyzer = new TES5InheritanceGraphAnalyzer();
        }

        /*
        * Gets the EDID of this script as it was in oblivion.
         * Script name may be obfuscated with md5 if the name is too long
        */
        public string getEdid()
        {
            return this.edid;
        }

        public IEnumerable<string> Output => new List<string>() { "ScriptName " + this.scriptNamePrefix + this.EscapedScriptName + " extends " + this.scriptType.getNativeType().Output.Single() + " " + (this.isHidden ? "Hidden" : "Conditional") };

        /*
             * @throws ConversionException
        */
        public void setNativeType(ITES5Type scriptType)
        {
            if (!TES5InheritanceGraphAnalyzer.isExtending(scriptType, this.scriptType.getNativeType()))
            {
                throw new ConversionException("Cannot set script type to non-extending type - current native type " + this.scriptType.getNativeType().value() + ", new type " + scriptType.value());
            }

            this.scriptType.setNativeType(scriptType.getNativeType());
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