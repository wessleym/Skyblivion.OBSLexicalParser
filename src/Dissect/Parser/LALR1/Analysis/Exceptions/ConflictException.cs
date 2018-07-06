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
        /*
        * Returns the number of the inadequate state.
        */
        public int StateNumber { get; protected set; }
        /*
        * Returns the faulty automaton.
        */
        public Automaton Automaton { get; protected set; }
        public ConflictException(string message, int state, Automaton automaton)
            : base(message)
        {
            this.StateNumber = state;
            this.Automaton = automaton;
        }
    }
}
