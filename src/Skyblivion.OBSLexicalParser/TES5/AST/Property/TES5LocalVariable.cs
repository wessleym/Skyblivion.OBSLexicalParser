using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5LocalVariable : TES5VariableOrProperty
    {
        public TES5LocalVariableParameterMeaning[] Meanings { get; private set; }
        public TES5LocalVariable(string nameWithSuffix, TES5BasicType type, TES5LocalVariableParameterMeaning[] meanings = null)
            : base(nameWithSuffix, type)
        {
            if (meanings == null) { meanings = new TES5LocalVariableParameterMeaning[] { }; }
            this.Meanings = meanings;
        }

        /*
        * Todo - following two methods should not be in this interface but TES5Property interface
         * This can be easily done, as the logic is the same as in TES5Property - Set the tracked script,
         * inference information if any available
         * @throws ConversionException
        */
        public override string ReferenceEDID => throw new ConversionException("Local variables have no EDID references.");

        public override void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Local variables cannot track remote scripts.");
        }
    }
}