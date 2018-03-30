using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5ScriptAsVariable : ITES5Variable
    {
        private TES5ScriptHeader scriptHeader;
        public TES5ScriptAsVariable(TES5ScriptHeader scriptHeader)
        {
            this.scriptHeader = scriptHeader;
        }

        public string getPropertyName()
        {
            return "self";
        }

        public IEnumerable<string> output()
        {
            return new string[] { "self" };
        }

        public ITES5Type getPropertyType()
        {
            return this.scriptHeader.getScriptType();
        }

        public void setPropertyType(ITES5Type type)
        {
            this.scriptHeader.setNativeType(type);
        }

        public string getReferenceEdid()
        {
            return this.scriptHeader.getEdid();
        }

        public void trackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Cannot track TES5ScriptAsVariable as it tracks already.");
        }
    }
}