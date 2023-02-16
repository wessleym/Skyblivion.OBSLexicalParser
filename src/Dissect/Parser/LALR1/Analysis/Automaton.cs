using Dissect.Extensions;
using Dissect.Parser.LALR1.Analysis.KernelSet;
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
        public Dictionary<Node, State> States { get; } = new Dictionary<Node, State>();
        /*
        * Returns the transition table for this automaton.
         *
         *  The transition table.
        */
        public Dictionary<Node, Dictionary<string, Node>> TransitionTable { get; } = new Dictionary<Node, Dictionary<string, Node>>();
        /*
        * Adds a new automaton state.
         *
         *  The new state.
        */
        public void AddState(State state)
        {
            this.States.Add(state.Node, state);
        }

        /*
        * Adds a new transition in the FSA.
         *
         *  The number of the origin state.
         *  The symbol that triggers this transition.
         *  The destination state number.
        */
        public void AddTransition(Node origin, string label, Node destintation)
        {
            Dictionary<string, Node> transition = TransitionTable.GetOrAdd(origin, () => new Dictionary<string, Node>());
            transition.Add(label, destintation);
        }

        /*
        * Returns a state by its node.
         *
         *  The state node.
         *
         *  The requested state.
        */
        public State GetState(Node node)
        {
            return this.States[node];
        }

        /*
        * Does this automaton have a state identified by node?
        */
        public bool HasState(Node node)
        {
            return this.States.ContainsKey(node);
        }
    }
}