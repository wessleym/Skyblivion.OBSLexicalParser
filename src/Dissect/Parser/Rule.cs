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
        protected int number;
        protected string name;
        protected string[] components;
        protected Func<object[], object> callback = null;
        protected Nullable<int> precedence = null;
        /*
        * Constructor.
        */
        public Rule(int number, string name, string[] components)
        {
            this.number = number;
            this.name = name;
            this.components = components;
        }

        /*
        * Returns the number of this rule.
        */
        public int getNumber()
        {
            return this.number;
        }

        /*
        * Returns the name of this rule.
        */
        public string getName()
        {
            return this.name;
        }

        /*
        * Returns the components of this rule.
        */
        public string[] getComponents()
        {
            return this.components;
        }

        /*
        * Returns a component at index index or null
         * if index is out of range.
        */
        public string getComponent(int index)
        {
            if (index > components.Length - 1)
            {
                return null;
            }
            return this.components[index];
        }

        /*
        * Sets the callback (the semantic value) of the rule.
        */
        public void setCallback(Func<object[], object> callback)
        {
            this.callback = callback;
        }

        public Func<object[], object> getCallback()
        {
            return this.callback;
        }

        public Nullable<int> getPrecedence()
        {
            return this.precedence;
        }

        public void setPrecedence(int i)
        {
            this.precedence = i;
        }
    }
}