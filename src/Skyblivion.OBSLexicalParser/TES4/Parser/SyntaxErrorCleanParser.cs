using Dissect.Lexer;
using Dissect.Lexer.TokenStream;
using Dissect.Parser.Exceptions;
using Dissect.Parser.LALR1.Analysis;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class SyntaxErrorCleanParser : Dissect.Parser.LALR1.Parser
    {
        public SyntaxErrorCleanParser(Dissect.Parser.Grammar grammar, ActionAndGoTo parseTable = null)
            : base(grammar, parseTable)
        { }

        public IToken ParseWithFixLogic(ITokenStream stream)
        {
            try
            {
                return base.parse(stream);
            }
            catch (UnexpectedTokenException e)
            {
                IToken exToken = e.getToken();
                bool isFixed = false;
                if (exToken.getValue() == "endif")
                {
                    int nesting = 0;
                    /*
                     * @var CommonToken token
                     */
                    List<IToken> tokens = new List<IToken>();
                    foreach (var token in stream)
                    {
                        if (token.getType() == "BranchStartToken")
                        {
                            ++nesting;
                            tokens.Add(token);
                        }
                        else
                        {
                            if (token.getType() == "BranchEndToken")
                            {
                                nesting = nesting - 1;
                                if (nesting > -1)
                                {
                                    tokens.Add(token);
                                }
                                else
                                {
                                    isFixed = true;
                                    nesting = 0; //Clear up the token and nesting will be again 0
                                }
                            }
                            else
                            {
                                tokens.Add(token);
                            }
                        }
                    }

                    if (!isFixed)
                    {
                        throw;
                    }

                    ArrayTokenStream newTokenStream = new ArrayTokenStream(tokens);
                    IToken newAST = this.parse(newTokenStream);
                    return newAST;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}