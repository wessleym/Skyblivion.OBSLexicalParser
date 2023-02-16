using Dissect.Parser;
using System;
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
        /*
        * Returns the rule of this item.
         *
         *  The rule.
        */
        public Rule Rule { get; protected set; }
        /*
        * Returns the dot index of this item.
         *
         *  The dot index.
        */
        public int DotIndex { get; protected set; }
        /*
        * Returns the computed lookahead for this item.
         *
         *  The lookahead symbols.
        */
        public List<string> Lookahead { get; protected set; } = new List<string>();
        protected List<Item> Connected = new List<Item>();
        /*
        * Constructor.
         *
         *  The rule of this item.
         *  The index of the dot in this item.
        */
        public Item(Rule rule, int dotIndex)
        {
            this.Rule = rule;
            this.DotIndex = dotIndex;
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
        public string ActiveComponent
        {
            get
            {
                string? component = this.Rule.GetComponent(this.DotIndex);
                if (component == null) { throw new InvalidOperationException(nameof(component) + " was null when " + nameof(DotIndex) + " was " + DotIndex + "."); }
                return component;
            }
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
        public bool IsReduceItem => this.DotIndex == this.Rule.Components.Length;

        /*
        * Connects two items with a lookahead pumping channel.
         *
         *  The item.
        */
        public void Connect(Item i)
        {
            this.Connected.Add(i);
        }

        /*
        * Pumps a lookahead token to this item and all items connected
         * to it.
         *
         *  The lookahead token name.
        */
        public void Pump(string lookahead)
        {
            if (!this.Lookahead.Contains(lookahead))
            {
                this.Lookahead.Add(lookahead);
                foreach (var item in this.Connected)
                {
                    item.Pump(lookahead);
                }
            }
        }

        /*
        * Pumps several lookahead tokens.
         *
         *  The lookahead tokens.
        */
        public void PumpAll(IEnumerable<string> lookahead)
        {
            foreach (var l in lookahead)
            {
                this.Pump(l);
            }
        }

        /*
        * Returns all components that haven't been recognized
         * so far.
         *
         *  The unrecognized components.
        */
        public IEnumerable<string> UnrecognizedComponents => this.Rule.Components.Skip(this.DotIndex + 1);
    }
}