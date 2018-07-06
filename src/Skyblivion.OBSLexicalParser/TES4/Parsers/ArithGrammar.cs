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