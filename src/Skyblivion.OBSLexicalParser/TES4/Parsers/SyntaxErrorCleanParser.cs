using Dissect.Lexer;
using Dissect.Lexer.TokenStream;
using Dissect.Parser;
using Dissect.Parser.Exceptions;
using Dissect.Parser.LALR1.Analysis;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Parsers
{
    class SyntaxErrorCleanParser : Dissect.Parser.LALR1.Parser
    {
        public SyntaxErrorCleanParser(Grammar grammar, ActionAndGoTo parseTable = null)
            : base(grammar, parseTable)
        { }

        public object ParseWithFixLogic(ITokenStream stream)
        {
            //WTM:  Change:  If the script is just a comment, resulting in only an EOF token, parser.ParseWithFixLogic fails.
            //The below check works around that.
            IToken[] firstTwoTokens = stream.Take(2).ToArray();
            if (firstTwoTokens.Length == 1 && firstTwoTokens[0].getType() == EOF_TOKEN_TYPE) { throw new EOFOnlyException(); }
            try
            {
                return base.parse(stream);
            }
            catch (UnexpectedTokenException ex) when (ex.getToken().getValue()=="endif")
            {
                bool isFixed = false;
                int nesting = 0;
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
                object newAST = this.parse(newTokenStream);
                return newAST;
            }
        }
    }
}