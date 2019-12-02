using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MessageFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public MessageFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            string messageString = functionArguments[0].StringValue;
            MatchCollection messageMatches = Regex.Matches(messageString, @"%([ +-0]*[1-9]*\.[0-9]+[ef]|g)");
            if (messageMatches.Cast<Match>().Any(m=>m.Success))
            {
                //Pack the printf syntax
                //TODO - Perhaps we can use sprintf?
                ITES4StringValue arg0 = functionArguments.Pop(0);
                if (!(arg0 is TES4String))
                { //hacky
                    throw new ConversionException("Cannot transform printf like syntax to concat on string loaded dynamically");
                }

                int i = 0;
                int caret = 0;
                //Example call: You have %.2f apples and %g boxes in your inventory, applesCount, boxesCount
                ITES5Value[] variablesArray = functionArguments.Select(a => this.valueFactory.CreateValue(a, codeScope, globalScope, multipleScriptsScope)).ToArray();

                List<TES5String> stringsList = new List<TES5String>();//Target: "You have ", " apples and ", " boxes in your inventory"
                bool startWithVariable = false; //Pretty ugly. Basically, if we start with a vairable, it should be pushed first from the variable stack and then string comes, instead of string , variable , and so on [...]
                while (caret < messageString.Length)
                {
                    int stringBeforeStart = caret; //Set the start on the caret.
                    Match? match = i < messageMatches.Count ? messageMatches[i] : null;
                    if (match != null)
                    {
                        int stringBeforeEnd = match.Index;
                        int length = stringBeforeEnd - stringBeforeStart;
                        if (caret == 0 && length == 0)
                        {
                            startWithVariable = true;
                        }

                        if (length > 0)
                        {
                            stringsList.Add(new TES5String(messageString.Substring(stringBeforeStart, length)));
                            caret += length;
                        }

                        caret += match.Length;
                    }
                    else
                    {
                        stringsList.Add(new TES5String(messageString.Substring(stringBeforeStart)));
                        caret = messageString.Length;
                    }

                    ++i;
                }

                List<ITES5Value> combinedValues = new List<ITES5Value>();
                Stack<TES5String> stringsStack = new Stack<TES5String>(stringsList.Select(kvp=>kvp).Reverse());
                Stack<ITES5Value> variablesStack = new Stack<ITES5Value>(variablesArray.Select(kvp=>kvp).Reverse());
                if (startWithVariable)
                {
                    if (variablesStack.Any())
                    {
                        combinedValues.Add(variablesStack.Pop());
                    }
                }

                while (stringsStack.Any())
                {
                    combinedValues.Add(stringsStack.Pop());
                    if (variablesStack.Any())
                    {
                        combinedValues.Add(variablesStack.Pop());
                    }
                }

                calledOn = staticReferenceFactory.Debug;
                TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
                {
                    TES5PrimitiveValueFactory.CreateConcatenatedValue(combinedValues)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "Notification", arguments);
            }
            else
            {
                calledOn = staticReferenceFactory.Debug;
                TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
                {
                    this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "Notification", arguments);
            }
        }
    }
}