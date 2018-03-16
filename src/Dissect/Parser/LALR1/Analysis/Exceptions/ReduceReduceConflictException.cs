using Dissect.Parser.LALR1.Analysis;
using Dissect.Parser;
using System.Linq;

namespace Dissect.Parser.LALR1.Analysis.Exceptions
{
    /*
     * Thrown when a grammar is not LALR(1) and exhibits
     * a reduce/reduce conflict.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class ReduceReduceConflictException : ConflictException
    {
        /*
        * The exception message template.
        */
        const string MESSAGE =
@"The grammar exhibits a reduce/reduce conflict on rules:

  %d. %s -> %s

vs:

  %d. %s -> %s

(on lookahead ""%s"" in state %d). Restructure your grammar or choose a conflict resolution mode.";
        protected Rule firstRule;
        protected Rule secondRule;
        protected string lookahead;
        /*
        * Constructor.
         *
         *  The number of the inadequate state.
         *  The first conflicting grammar rule.
         *  The second conflicting grammar rule.
         *  The conflicting lookahead.
         *  The faulty automaton.
        */
        public ReduceReduceConflictException(int state, Rule firstRule, Rule secondRule, string lookahead, Automaton automaton)
            : base(GetMessage(state, firstRule, secondRule, lookahead), state, automaton)
        {
            this.firstRule = firstRule;
            this.secondRule = secondRule;
            this.lookahead = lookahead;
        }

        private static string GetMessage(int state, Rule firstRule, Rule secondRule, string lookahead)
        {
            string[] components1 = firstRule.getComponents();
            string[] components2 = secondRule.getComponents();
            return string.Format(MESSAGE, firstRule.getNumber(), firstRule.getName(), !components1.Any() ? "/* empty */" : string.Join(" ", components1), secondRule.getNumber(), secondRule.getName(), !components2.Any() ? "/* empty */" : string.Join(" ", components2), lookahead, state);
        }

        /*
        * Returns the first conflicting rule.
         *
         *  The first conflicting rule.
        */
        public Dissect.Parser.Rule getFirstRule()
        {
            return this.firstRule;
        }

        /*
        * Returns the second conflicting rule.
         *
         *  The second conflicting rule.
        */
        public Dissect.Parser.Rule getSecondRule()
        {
            return this.secondRule;
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