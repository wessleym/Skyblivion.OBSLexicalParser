using Dissect.Parser;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class ArithGrammar : Grammar
    {
        [Obsolete("For Testing")]
        public void token(string val)
        {
        }

        [Obsolete("For Testing")]
        public void createSampleLexer()
        {
            this.token("0");
            this.token("1");
        }

        public void createSampleParser()
        {
            __invoke("Digit")._is("0")._is("1");
            __invoke("Number")._is("Digit")._is("Number", "Digit");
        }

        public ArithGrammar()
        {
            __invoke("Additive")._is("Additive", "+", "Multiplicative")._is("Multiplicative");
            __invoke("Multiplicative")._is("Multiplicative", "*", "Power")._is("Power");
            __invoke("Power")._is("Primary", "**", "Power")._is("Primary");
            __invoke("Primary")._is("INT")._is("(", "Additive", ")");
            this.start("Additive");
        }
    }
}