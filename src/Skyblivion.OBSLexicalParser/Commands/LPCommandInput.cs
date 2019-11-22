using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandInput
    {
        protected List<LPCommandArgument> arguments = new List<LPCommandArgument>();
        protected List<LPCommandOption> options = new List<LPCommandOption>();
        public void AddArgument(LPCommandArgument argument)
        {
            arguments.Add(argument);
        }
        public void AddOption(LPCommandOption option)
        {
            options.Add(option);
        }
        private string GetArgumentOrOptionValue(IEnumerable<LPCommandArgumentOrOption> collection, string name)
        {
            return LPCommandArgumentOrOption.GetValue(collection, name);
        }
        public string GetArgumentValue(string name)
        {
            return GetArgumentOrOptionValue(arguments, name);
        }
        public string GetOptionValue(string name)
        {
            return GetArgumentOrOptionValue(options, name);
        }
        public bool GetOptionBoolean(string name)
        {
            return Convert.ToBoolean(GetOptionValue(name));
        }
    }
}
