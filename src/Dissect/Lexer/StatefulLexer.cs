using Dissect.Lexer.Recognizer;
using Dissect.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dissect.Lexer
{
    /*
     * The StatefulLexer works like SimpleLexer,
     * but internally keeps notion of current lexer state.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class StatefulLexer : AbstractLexer
    {
        //DissectChange:
        protected class State
        {
            public Dictionary<string, object> Actions = new Dictionary<string, object>();
            public Dictionary<string, IRecognizer> Recognizers=new Dictionary<string, IRecognizer>();
            public string[] SkipTokens = new string[] { };
        }

        protected Dictionary<string, State> states = new Dictionary<string, State>();
        protected Stack<string> stateStack = new Stack<string>();
        protected string stateBeingBuilt =  null;
        protected string typeBeingBuilt =  null;
        /*
        * Signifies that no action should be taken on encountering a token.
        */
        const int NO_ACTION = 0;
        /*
        * Indicates that a state should be popped of the state stack on
        * encountering a token.
        */
        public const int POP_STATE = 1;
        /*
        * Adds a new token definition. If given only one argument,
        * it assumes that the token type and recognized value are
        * identical.
        */
        public StatefulLexer token(string type, string value = null)
        {
            if (this.stateBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state first.");
            }

            if (value == null)
            {
                value = type;
            }

            this.states[this.stateBeingBuilt].Recognizers[type] = new SimpleRecognizer(value);
            this.states[this.stateBeingBuilt].Actions[type] = NO_ACTION;
            this.typeBeingBuilt = type;
            return this;
        }

        /*
        * Adds a new regex token definition.
        */
        public StatefulLexer regex(string type, Regex regex)
        {
            if (this.stateBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state first.");
            }

            this.states[this.stateBeingBuilt].Recognizers[type] = new RegexRecognizer(regex);
            this.states[this.stateBeingBuilt].Actions[type] = NO_ACTION;
            this.typeBeingBuilt = type;
            return this;
        }

        /*
        * Marks the token types given as arguments to be skipped.
        */
        public StatefulLexer skip(params string[] args)
        {
            if (this.stateBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state first.");
            }

            this.states[this.stateBeingBuilt].SkipTokens = args;
            return this;
        }

        /*
        * Registers a new lexer state.
        */
        //DissectChange:
        public StatefulLexer _state(string state)
        {
            this.stateBeingBuilt = state;
            this.states[state] = new State();
            return this;
        }

        /*
        * Sets the starting state for the lexer.
        */
        public StatefulLexer start(string state)
        {
            this.stateStack.Push(state);
            return this;
        }

        /*
        * Sets an action for the token type that is currently being built.
        */
        private StatefulLexer action(object actionObject)
        {
            if (this.stateBeingBuilt == null || this.typeBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state and type first.");
            }

            this.states[this.stateBeingBuilt].Actions[this.typeBeingBuilt] = actionObject;
            return this;
        }
        public StatefulLexer action(string actionString)
        {
            return action((object)actionString);
        }
        public StatefulLexer action(int actionInt)
        {
            return action((object)actionInt);
        }

        protected override bool shouldSkipToken(IToken token)
        {
            var state = this.states[this.stateStack.Last()];
            return state.SkipTokens.Contains(token.getType());
        }

        protected override IToken extractToken(string str)
        {
            if (!this.stateStack.Any())
            {
                throw new InvalidOperationException("You must set a starting state before lexing.");
            }

            string value = null;
            string type = null;
            object action = null;
            var state = this.states[this.stateStack.Last()];
            foreach (var kvp in state.Recognizers)
            {
                IRecognizer recognizer = kvp.Value;
                string v;
                if (recognizer.match(str, out v))
                {
                    if (value == null || Dissect.Util.Util.stringLength(v) > Dissect.Util.Util.stringLength(value))
                    {
                        value = v;
                        type = kvp.Key;
                        action = state.Actions[type];
                    }
                }
            }

            if (type == null)
            {
                string actionString = action as string;
                if (actionString!=null)
                { // enter new state
                    this.stateStack.Push(actionString);
                }
                else if((int)action == POP_STATE)
                {
                    this.stateStack.Pop();
                }

                return new CommonToken(type, value, this.getCurrentLine());
            }

            return null;
        }
    }
}