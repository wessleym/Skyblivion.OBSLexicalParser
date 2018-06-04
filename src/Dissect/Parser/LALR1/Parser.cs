using Dissect.Extensions.StackExtensions;
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
        protected Grammar grammar;
        protected ActionAndGoTo parseTable;
        /*
        * Constructor.
         *
         *  The grammar.
         *  If given, the parser doesn"t have to analyze the grammar.
        */
        public Parser(Grammar grammar, ActionAndGoTo parseTable = null)
        {
            this.grammar = grammar;
            if (parseTable != null)
            {
                this.parseTable = parseTable;
            }
            else
            {
                Analyzer analyzer = new Analyzer();
                this.parseTable = analyzer.analyze(grammar).getParseTable();
            }
        }

        protected override object parse(ITokenStream stream)
        {
            int currentState = 0;
            Stack<int> stateStack = new Stack<int>();
            stateStack.Push(currentState);
            Stack<object> args = new Stack<object>();
            foreach (CommonToken token in stream)
            {
                while (true)
                {
                    string type = token.getType();
                    Dictionary<string, int> typeToAction = this.parseTable.Action[currentState];
                    int action;
                    if(!typeToAction.TryGetValue(type, out action))
                    {// unexpected token
                        throw new UnexpectedTokenException(token, this.parseTable.Action[currentState].Select(kvp => kvp.Key).ToArray());
                    }
                    if (action > 0)
                    {
                        // shift
                        args.Push(token);
                        currentState = action;
                        stateStack.Push(currentState);
                        break;
                    }
                    else if(action < 0)
                    {
                        // reduce
                        Rule rule = this.grammar.getRule(-action);
                        int popCount = rule.getComponents().Length;
                        stateStack.Pop(popCount).ToArray();
                        object[] newArgs = args.Pop(popCount).Reverse().ToArray();
                        var callback = rule.getCallback();
                        if (callback != null)
                        {
                            object newToken = callback.Invoke(newArgs);
                            args.Push(newToken);
                        }
                        else
                        {
                            args.Push(newArgs[0]);
                        }

                        int state = stateStack.Peek();
                        currentState = this.parseTable.GoTo[state][rule.getName()];
                        stateStack.Push(currentState);
                    }
                    else 
                    {
                        // accept
                        return args.Last();
                    }
                }
            }
            //WTM:  Note:  In PHP, this function did not return here, so I'm throwing an exception to complete the function.  I don't think this line is ever invoked
            throw new InvalidOperationException();
        }
    }
}