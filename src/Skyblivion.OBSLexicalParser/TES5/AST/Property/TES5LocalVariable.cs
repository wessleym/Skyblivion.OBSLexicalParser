using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5LocalVariable : ITES5Variable
    {
        private string variableName;
        private ITES5Type type;
        private TES5LocalVariableParameterMeaning[] meanings;
        public TES5LocalVariable(string variableName, TES5BasicType type, TES5LocalVariableParameterMeaning[] meanings = null)
        {
            if (meanings == null) { meanings = new TES5LocalVariableParameterMeaning[] { }; }
            this.variableName = variableName;
            this.type = type;
            this.meanings = meanings;
        }

        public IEnumerable<string> output()
        {
            return new string[] { this.type.value() + " " + this.variableName };
        }

        public TES5LocalVariableParameterMeaning[] getMeanings()
        {
            return this.meanings;
        }

        public ITES5Type getPropertyType()
        {
            return this.type;
        }

        public string getPropertyName()
        {
            return this.variableName;
        }

        public void setPropertyType(ITES5Type type)
        {
            this.type = type;
        }

        /*
        * Todo - following two methods should not be in this interface but TES5Property interface
         * This can be easily done, as the logic is the same as in TES5Property - Set the tracked script,
         * inference information if any available
         * @throws ConversionException
        */
        public string getReferenceEdid()
        {
            throw new ConversionException("Local variables have no EDID references.");
        }

        public void trackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Local variables cannot track remote scripts.");
        }
    }
}