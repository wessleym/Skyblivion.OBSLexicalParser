using Dissect.Extensions;
using Dissect.Lexer;
using Dissect.Lexer.TokenStream;
using Dissect.Parser.Exceptions;
using Dissect.Parser.LALR1.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissect.Parser.LALR1
{
    /*
     * A LR parser.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class Parser : Dissect.Parser.Parser
    {
        protected readonly Grammar Grammar;
        protected readonly ActionAndGoTo ParseTable;
        /*
        * Constructor.
         *
         *  The grammar.
         *  If given, the parser doesn't have to analyze the grammar.
        */
        public Parser(Grammar grammar, ActionAndGoTo? parseTable = null)
        {
            this.Grammar = grammar;
            if (parseTable != null)
            {
                this.ParseTable = parseTable;
            }
            else
            {
                Analyzer analyzer = new Analyzer();
                this.ParseTable = analyzer.Analyze(grammar).ParseTable;
            }
        }

        protected override object Parse(ITokenStream stream)
        {
            int currentState = 0;
            Stack<int> stateStack = new Stack<int>();
            stateStack.Push(currentState);
            Stack<object> args = new Stack<object>();
            foreach (IToken token in stream)
            {
                //Sometimes useful for debugging:  (new Analyzer()).Analyze(Grammar).Automaton.States[currentState] (especially .Items[].Rule)
                string type = token.Type;
                while (true)
                {
                    Dictionary<string, int> typeToAction = this.ParseTable.Action[currentState];
                    int action;
                    if (!typeToAction.TryGetValue(type, out action))
                    {// unexpected token
                        throw new UnexpectedTokenException(token, typeToAction.Select(kvp => kvp.Key).ToArray());
                    }
                    if (action > 0)
                    {
                        // shift
                        args.Push(token);
                        currentState = action;
                        stateStack.Push(currentState);
                        break;
                    }
                    else if (action < 0)
                    {
                        // reduce
                        Rule rule = this.Grammar.GetRule(-action);
                        int popCount = rule.Components.Length;
                        stateStack.Pop(popCount).ToArray();
                        object[] newArgs = args.Pop(popCount).Reverse().ToArray();
                        var callback = rule.Callback;
                        if (callback != null)
                        {
                            object newToken;
                            try//WTM:  Added:  try-catch
                            {
                                newToken = callback.Invoke(newArgs);
                            }
                            catch (Exception ex)
                            {
                                throw new CallbackException(token, ex);
                            }
                            args.Push(newToken);
                        }
                        else
                        {
                            args.Push(newArgs[0]);
                        }

                        int state = stateStack.Peek();
                        currentState = this.ParseTable.GoTo[state][rule.Name];
                        stateStack.Push(currentState);
                    }
                    else
                    {
                        // accept
                        return args.Last();
                    }
                }
            }
            //WTM:  Note:  In PHP, this function did not return here, so I'm throwing an exception to complete the method.
            throw new InvalidOperationException("Ran out of tokens.");
        }
    }
}