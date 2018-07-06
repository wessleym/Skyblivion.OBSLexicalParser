using System;

namespace Dissect.Parser
{
    /*
     * Represents a rule in a context-free grammar.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class Rule
    {
        /*
        * Returns the number of this rule.
        */
        public int Number { get; protected set; }
        /*
        * Returns the name of this rule.
        */
        public string Name { get; protected set; }
        /*
        * Returns the components of this rule.
        */
        public string[] Components { get; protected set; }
        /*
        * Gets or sets the callback (the semantic value) of the rule.
        */
        public Func<object[], object> Callback = null;
        public Nullable<int> Precedence = null;
        /*
        * Constructor.
        */
        public Rule(int number, string name, string[] components)
        {
            this.Number = number;
            this.Name = name;
            this.Components = components;
        }

        /*
        * Returns a component at index index or null
         * if index is out of range.
        */
        public string GetComponent(int index)
        {
            if (index > Components.Length - 1)
            {
                return null;
            }
            return this.Components[index];
        }
    }
}