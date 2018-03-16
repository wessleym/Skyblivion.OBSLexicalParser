using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptHeader : ITES5Outputtable
    {
        private string scriptName;
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
        public TES5ScriptHeader(string scriptName, string edid, ITES5Type scriptType, string scriptNamePrefix, bool isHidden = false)
        {
            this.scriptName = scriptName;
            this.edid = edid;
            this.scriptNamePrefix = scriptNamePrefix;
            this.scriptType = TES5TypeFactory.memberByValue(scriptName, scriptType);
            this.basicScriptType = scriptType;
            this.isHidden = isHidden;
            this.inheritanceAnalyzer = new TES5InheritanceGraphAnalyzer();
        }

        public string getScriptName()
        {
            return this.scriptName;
        }

        /*
        * Gets the EDID of this script as it was in oblivion.
         * Script name may be obfuscated with md5 if the name is too long
        */
        public string getEdid()
        {
            return this.edid;
        }

        public List<string> output()
        {
            if (this.isHidden)
            {
                return new List<string>() { "ScriptName " + this.scriptNamePrefix + this.scriptName + " extends " + this.scriptType.getNativeType().output() + " Hidden" };
            }
            else
            {
                return new List<string>() { "ScriptName " + this.scriptNamePrefix + this.scriptName + " extends " + this.scriptType.getNativeType().output() + " Conditional" };
            }
        }

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