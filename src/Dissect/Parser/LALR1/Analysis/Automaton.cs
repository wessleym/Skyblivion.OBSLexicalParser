using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    /*
     * A finite-state automaton for recognizing
     * grammar productions.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class Automaton
    {
        protected Dictionary<int, State> states = new Dictionary<int, State>();
        protected Dictionary<int, Dictionary<string, int>> transitionTable = new Dictionary<int, Dictionary<string, int>>();
        /*
        * Adds a new automaton state.
         *
         *  The new state.
        */
        public void addState(State state)
        {
            this.states[state.getNumber()] = state;
        }

        /*
        * Adds a new transition in the FSA.
         *
         *  The number of the origin state.
         *  The symbol that triggers this transition.
         *  The destination state number.
        */
        public void addTransition(int origin, string label, int dest)
        {
            if(!transitionTable.ContainsKey(origin))
            {
                transitionTable.Add(origin, new Dictionary<string, int>());
            }
            this.transitionTable[origin][label] = dest;
        }

        /*
        * Returns a state by its number.
         *
         *  The state number.
         *
         *  The requested state.
        */
        public State getState(int number)
        {
            return this.states[number];
        }

        /*
        * Does this automaton have a state identified by number?
        */
        public bool hasState(int number)
        {
            return this.states.ContainsKey(number);
        }

        /*
        * Returns all states in this FSA.
         *
         *  The states of this FSA.
        */
        public Dictionary<int, State> getStates()
        {
            return this.states;
        }

        /*
        * Returns the transition table for this automaton.
         *
         *  The transition table.
        */
        public Dictionary<int, Dictionary<string, int>> getTransitionTable()
        {
            return this.transitionTable;
        }
    }
}