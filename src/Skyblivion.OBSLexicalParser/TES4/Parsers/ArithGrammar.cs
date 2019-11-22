using Dissect.Parser;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class ArithGrammar : Grammar
    {
        [Obsolete("For Testing")]
        public void Token(string val)
        {
        }

        [Obsolete("For Testing")]
        public void CreateSampleLexer()
        {
            this.Token("0");
            this.Token("1");
        }

        public void CreateSampleParser()
        {
            __invoke("Digit").Is("0").Is("1");
            __invoke("Number").Is("Digit").Is("Number", "Digit");
        }

        public ArithGrammar()
        {
            __invoke("Additive").Is("Additive", "+", "Multiplicative").Is("Multiplicative");
            __invoke("Multiplicative").Is("Multiplicative", "*", "Power").Is("Power");
            __invoke("Power").Is("Primary", "**", "Power").Is("Primary");
            __invoke("Primary").Is("INT").Is("(", "Additive", ")");
            this.Start("Additive");
        }
    }
}