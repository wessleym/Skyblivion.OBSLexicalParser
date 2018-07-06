using System.Linq;

namespace Dissect.Parser.LALR1.Analysis.Exceptions
{
    /*
     * Thrown when a grammar is not LALR(1) and exhibits
     * a shift/reduce conflict.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class ShiftReduceConflictException : ConflictException
    {
        /*
        * Returns the conflicting rule.
         *
         *  The conflicting rule.
        */
        public Rule Rule { get; protected set; }
        /*
        * Returns the conflicting lookahead.
         *
         *  The conflicting lookahead.
        */
        public string Lookahead { get; protected set; }
        /*
        * Constructor.
         *
         *  The conflicting grammar rule.
         *  The conflicting lookahead to shift.
         *  The faulty automaton.
        */
        public ShiftReduceConflictException(int state, Rule rule, string lookahead, Automaton automaton)
            : base(GetMessage(rule, state, lookahead), state, automaton)
        {
            this.Rule = rule;
            this.Lookahead = lookahead;
        }

        private static string GetMessage(Rule rule, int state, string lookahead)
        {
            string[] components = rule.Components;
            return
@"The grammar exhibits a shift/reduce conflict on rule:

  " + rule.Number+ @". " + rule.Name+ @" -> " + (!components.Any() ? "/* empty */" : string.Join(" ", components)) + @"

(on lookahead """ + lookahead + @""" in state " + state + @"). Restructure your grammar or choose a conflict resolution mode.";
        }
    }
}