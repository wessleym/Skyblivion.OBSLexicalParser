using Dissect.Extensions;
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
        /*
        * Returns all states in this FSA.
         *
         *  The states of this FSA.
        */
        public Dictionary<int, State> States { get; protected set; } = new Dictionary<int, State>();
        /*
        * Returns the transition table for this automaton.
         *
         *  The transition table.
        */
        public Dictionary<int, Dictionary<string, int>> TransitionTable { get; protected set; } = new Dictionary<int, Dictionary<string, int>>();
        /*
        * Adds a new automaton state.
         *
         *  The new state.
        */
        public void AddState(State state)
        {
            this.States.Add(state.Number, state);
        }

        /*
        * Adds a new transition in the FSA.
         *
         *  The number of the origin state.
         *  The symbol that triggers this transition.
         *  The destination state number.
        */
        public void AddTransition(int origin, string label, int dest)
        {
            Dictionary<string, int> transition = TransitionTable.GetOrAdd(origin, () => new Dictionary<string, int>());
            transition.Add(label, dest);
        }

        /*
        * Returns a state by its number.
         *
         *  The state number.
         *
         *  The requested state.
        */
        public State GetState(int number)
        {
            return this.States[number];
        }

        /*
        * Does this automaton have a state identified by number?
        */
        public bool HasState(int number)
        {
            return this.States.ContainsKey(number);
        }
    }
}