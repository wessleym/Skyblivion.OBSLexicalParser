using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5LocalVariable : ITES5VariableOrProperty
    {
        public string Name { get; }
        public ITES5Type TES5Type { get; set; }
        public TES5LocalVariable(string nameWithSuffix, TES5BasicType type)
        {
            Name = nameWithSuffix;
            TES5Type = type;
        }

        public ITES5Type TES5DeclaredType => TES5Type;

        /*
        * Todo - following two methods should not be in this interface but TES5Property interface
         * This can be easily done, as the logic is the same as in TES5Property - Set the tracked script,
         * inference information if any available
         * @throws ConversionException
        */
        public string ReferenceEDID => throw new ConversionException("Local variables have no EDID references.");

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new ConversionException("Local variables cannot track remote scripts.");
        }

        public IEnumerable<string> Output => new string[] { this.TES5Type.Value + " " + this.Name };
    }
}