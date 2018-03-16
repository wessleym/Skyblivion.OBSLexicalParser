using Dissect.Parser;
using System.Collections.Generic;
using System.Linq;

namespace Dissect.Parser.LALR1.Analysis
{
    /*
     * A LALR(1) item.
     *
     * An item represents a state where a part of
     * a grammar rule has been recognized. The current
     * position is marked by a dot:
     *
     * <pre>
     * A . a . b c
     * </pre>
     *
     * This means that within this item, a has been recognized
     * and b is expected. If the dot is at the very end of the
     * rule:
     *
     * <pre>
     * A . a b c .
     * </pre>
     *
     * it means that the whole rule has been recognized and
     * can be reduced.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class Item
    {
        protected Rule rule;
        protected int dotIndex;
        protected List<string> lookahead = new List<string>();
        protected List<Item> connected = new List<Item>();
        /*
        * Constructor.
         *
         *  The rule of this item.
         *  The index of the dot in this item.
        */
        public Item(Rule rule, int dotIndex)
        {
            this.rule = rule;
            this.dotIndex = dotIndex;
        }

        /*
        * Returns the dot index of this item.
         *
         *  The dot index.
        */
        public int getDotIndex()
        {
            return this.dotIndex;
        }

        /*
        * Returns the currently expected component.
         *
         * If the item is:
         *
         * <pre>
         * A . a . b c
         * </pre>
         *
         * then this method returns the component "b".
         *
         *  The component.
        */
        public string getActiveComponent()
        {
            return this.rule.getComponent(this.dotIndex);
        }

        /*
        * Returns the rule of this item.
         *
         *  The rule.
        */
        public Dissect.Parser.Rule getRule()
        {
            return this.rule;
        }

        /*
        * Determines whether this item is a reduce item.
         *
         * An item is a reduce item if the dot is at the very end:
         *
         * <pre>
         * A . a b c .
         * </pre>
         *
         *  Whether this item is a reduce item.
        */
        public bool isReduceItem()
        {
            return this.dotIndex == this.rule.getComponents().Length;
        }

        /*
        * Connects two items with a lookahead pumping channel.
         *
         *  The item.
        */
        public void connect(Item i)
        {
            this.connected.Add(i);
        }

        /*
        * Pumps a lookahead token to this item and all items connected
         * to it.
         *
         *  The lookahead token name.
        */
        public void pump(string lookahead)
        {
            if (!this.lookahead.Contains(lookahead))
            {
                this.lookahead.Add(lookahead);
                foreach (var item in this.connected)
                {
                    item.pump(lookahead);
                }
            }
        }

        /*
        * Pumps several lookahead tokens.
         *
         *  The lookahead tokens.
        */
        public void pumpAll(IEnumerable<string> lookahead)
        {
            foreach (var l in lookahead)
            {
                this.pump(l);
            }
        }

        /*
        * Returns the computed lookahead for this item.
         *
         *  The lookahead symbols.
        */
        public List<string> getLookahead()
        {
            return this.lookahead;
        }

        /*
        * Returns all components that haven"t been recognized
         * so far.
         *
         *  The unrecognized components.
        */
        public IEnumerable<string> getUnrecognizedComponents()
        {
            return this.rule.getComponents().Skip(this.dotIndex + 1);
        }
    }
}