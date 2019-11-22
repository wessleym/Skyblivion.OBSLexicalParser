using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    /**
     * TES5 signature parameter
     * Similar to a local variable, except that they can have 
     * a "meaning" ( a generic marker one can use to get a variable
     * it doesn't know name for ) and predefined declaration type
     */
    class TES5SignatureParameter : ITES5VariableOrProperty
    {
        public string Name { get; private set; }
        public ITES5Type TES5Type { get; set; }
        public TES5LocalVariableParameterMeaning[] Meanings { get; private set; }

        private readonly bool hasFixedDeclaration;
        private readonly ITES5Type declarationType;

        public TES5SignatureParameter(string nameWithSuffix, 
                                 TES5BasicType type, 
                                 bool hasFixedDeclaration,
                                 TES5LocalVariableParameterMeaning[]? meanings = null
                                )
        {

            Name = nameWithSuffix;
            TES5Type = type;
            declarationType = type;
            this.hasFixedDeclaration = hasFixedDeclaration;
            if (meanings == null) { meanings = new TES5LocalVariableParameterMeaning[] { }; }
            this.Meanings = meanings;
        }

        public ITES5Type TES5DeclaredType
        {
            get
            {
                if(hasFixedDeclaration)
                {
                    return declarationType;
                }
                else
                {
                    return TES5Type;
                }
            }
        }

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

        public IEnumerable<string> Output => new string[] { this.TES5DeclaredType.Value + " " + this.Name };    }
}
