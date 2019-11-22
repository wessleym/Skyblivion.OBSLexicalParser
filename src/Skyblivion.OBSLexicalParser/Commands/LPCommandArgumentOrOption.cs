using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandArgumentOrOption
    {
        public readonly string Name;
        private readonly string description;
        private readonly string? defaultValue;
        private readonly string? userValue = null;//WTM:  Note:  Never gets set.  This isn't developed yet and may never be necessary.
        protected LPCommandArgumentOrOption(string name, string description, string? defaultValue = null)
        {
            Name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }

        public string? Value => userValue != null ? userValue : defaultValue;

        public static string GetValue(IEnumerable<LPCommandArgumentOrOption> arguments, string name)
        {
            string? value = arguments.Where(i => i.Name == name).First().Value;
            if (value != null) { return value; }
            throw new InvalidOperationException("Argument " + name + " was null.");
        }
    }
}
