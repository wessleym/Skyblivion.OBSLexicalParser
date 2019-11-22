using Dissect.Lexer.Recognizer;
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
        protected readonly Dictionary<string, LexerState> States = new Dictionary<string, LexerState>();
        protected readonly Stack<string> StateStack = new Stack<string>();
        protected string? StateBeingBuilt = null;
        protected string? TypeBeingBuilt = null;
        /*
        * Signifies that no action should be taken on encountering a token.
        */
        const int NO_ACTION = 0;
        /*
        * Indicates that a state should be popped of the state stack on
        * encountering a token.
        */
        public const int POP_STATE = 1;

        private void CheckLexerState()
        {
            if (this.StateBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state first.");
            }
        }

        private void AddRecognizer(string type, IRecognizer recognizer)
        {
            CheckLexerState();
            LexerState state = this.States[this.StateBeingBuilt!];
            state.Recognizers.Add(type, recognizer);
            state.Actions.Add(type, NO_ACTION);
            this.TypeBeingBuilt = type;
        }

        /*
        * Adds a new token definition. If given only one argument,
        * it assumes that the token type and recognized value are
        * identical.
        */
        public StatefulLexer Token(string type, string? value = null, bool ignoreCase = false)
        {
            if (value == null) { value = type; }
            AddRecognizer(type, new SimpleRecognizer(value, ignoreCase));
            return this;
        }

        /*
        * Adds a new regex token definition.
        */
        public StatefulLexer Regex(string type, Regex regex)
        {
            if (!regex.Options.HasFlag(RegexOptions.Compiled))
            {
                throw new InvalidOperationException("Regex was not compiled.");
            }
            AddRecognizer(type, new RegexRecognizer(regex));
            return this;
        }
        private StatefulLexer Regex(string type, string pattern, RegexOptions options)
        {
            Regex(type, new Regex(pattern, options));
            return this;
        }
        public StatefulLexer Regex(string type, string pattern)
        {
            Regex(type, pattern, RegexOptions.Compiled);
            return this;
        }
        public StatefulLexer RegexIgnoreCase(string type, string pattern)
        {
            Regex(type, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return this;
        }

        /*
        * Marks the token types given as arguments to be skipped.
        */
        public StatefulLexer Skip(params string[] args)
        {
            CheckLexerState();
            LexerState state = this.States[this.StateBeingBuilt!];
            state.SkipTokens = args;
            return this;
        }

        /*
        * Registers a new lexer state.
        */
        //DissectChange:
        public StatefulLexer State(string state)
        {
            this.StateBeingBuilt = state;
            this.States.Add(state, new LexerState());
            return this;
        }

        /*
        * Sets the starting state for the lexer.
        */
        public StatefulLexer Start(string state)
        {
            this.StateStack.Push(state);
            return this;
        }

        /*
        * Sets an action for the token type that is currently being built.
        */
        private StatefulLexer Action(object actionObject)
        {
            if (this.StateBeingBuilt == null || this.TypeBeingBuilt == null)
            {
                throw new InvalidOperationException("Define a lexer state and type first.");
            }

            LexerState state = this.States[this.StateBeingBuilt];
            state.Actions[this.TypeBeingBuilt] = actionObject;
            return this;
        }
        public StatefulLexer Action(string actionString)
        {
            return Action((object)actionString);
        }
        public StatefulLexer Action(int actionInt)
        {
            return Action((object)actionInt);
        }

        protected override bool ShouldSkipToken(IToken token)
        {
            var state = this.States[this.StateStack.Peek()];
            return state.SkipTokens.Contains(token.Type);
        }

        protected override IToken? ExtractToken(string str)
        {
            if (!this.StateStack.Any())
            {
                throw new InvalidOperationException("You must set a starting state before lexing.");
            }

            string? value = null;
            string? type = null;
            object? action = null;
            var state = this.States[this.StateStack.Peek()];
            foreach (var kvp in state.Recognizers)
            {
                IRecognizer recognizer = kvp.Value;
                string? v;
                if (recognizer.Match(str, out v))
                {
                    if (value == null || Util.Util.StringLength(v) > Util.Util.StringLength(value))
                    {
                        value = v;
                        type = kvp.Key;
                        action = state.Actions[type];
                    }
                }
            }

            if (type != null)
            {
                if (action != null)
                {
                    string? actionString = action as string;
                    if (actionString != null)
                    { // enter new state
                        this.StateStack.Push(actionString);
                    }
                    else if ((int)action == POP_STATE)
                    {
                        this.StateStack.Pop();
                    }
                }

                return new CommonToken(type, value!, this.Line);
            }

            return null;
        }

        protected override void ResetStatesForNewString()
        {
            string lastState = this.StateStack.Last();
            this.StateStack.Clear();
            this.Start(lastState);
        }
    }
}