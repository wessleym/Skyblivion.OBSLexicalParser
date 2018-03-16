using Dissect.Parser.LALR1.Analysis;
using Dissect.Parser;
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
        * The exception message template.
        */
        const string MESSAGE =
@"The grammar exhibits a shift/reduce conflict on rule:

  %d. %s -> %s

(on lookahead ""%s"" in state %d). Restructure your grammar or choose a conflict resolution mode.";
        protected Rule rule;
        protected string lookahead;
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
            this.rule = rule;
            this.lookahead = lookahead;
        }

        private static string GetMessage(Rule rule, int state, string lookahead)
        {
            string[] components = rule.getComponents();
            return string.Format(MESSAGE, rule.getNumber(), rule.getName(), !components.Any() ? "/* empty */" : string.Join(" ", components), lookahead, state);
        }

        /*
        * Returns the conflicting rule.
         *
         *  The conflicting rule.
        */
        public Dissect.Parser.Rule getRule()
        {
            return this.rule;
        }

        /*
        * Returns the conflicting lookahead.
         *
         *  The conflicting lookahead.
        */
        public string getLookahead()
        {
            return this.lookahead;
        }
    }
}