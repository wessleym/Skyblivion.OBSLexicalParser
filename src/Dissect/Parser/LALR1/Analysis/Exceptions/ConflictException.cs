using System;

namespace Dissect.Parser.LALR1.Analysis.Exceptions
{
    /*
     * A base class for exception thrown when encountering
     * inadequate states during parse table construction.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class ConflictException : Exception
    {
        protected int state;
        protected Automaton automaton;
        public ConflictException(string message, int state, Automaton automaton)
            : base(message)
        {
            this.state = state;
            this.automaton = automaton;
        }

        /*
            * Returns the number of the inadequate state.
            */
        public int getStateNumber()
        {
            return this.state;
        }

        /*
            * Returns the faulty automaton.
            */
        public Automaton getAutomaton()
        {
            return this.automaton;
        }
    }
}
