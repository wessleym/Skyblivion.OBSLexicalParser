using Dissect.Extensions.IDictionaryExtensions;
using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    /*
     * A state in a handle-finding FSA.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class State
    {
        /*
        * Returns an array of items constituting this state.
         *
         *  The items.
        */
        public List<Item> Items { get; protected set; } = new List<Item>();
        protected readonly Dictionary<int, Dictionary<int, Item>> ItemMap = new Dictionary<int, Dictionary<int, Item>>();
        /*
        * Returns the number identifying this state.
        */
        public int Number { get; protected set; }
        /*
        * Constructor.
         *
         *  The number identifying this state.
         *  The initial items of this state.
        */
        public State(int number, Item[] items)
        {
            this.Number = number;
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /*
        * Adds a new item to this state.
         *
         *  The new item.
        */
        public void Add(Item item)
        {
            this.Items.Add(item);
            int ruleNumber = item.Rule.Number;
            Dictionary<int, Item> itemDictionary = this.ItemMap.GetOrAdd(ruleNumber, ()=> new Dictionary<int, Item>());
            itemDictionary.Add(item.DotIndex, item);
        }

        /*
        * Returns an item by its rule number and dot index.
         *
         *  The number of the rule of the desired item.
         *  The dot index of the desired item.
         *
         *  The item.
        */
        public Item Get(int ruleNumber, int dotIndex)
        {
            return this.ItemMap[ruleNumber][dotIndex];
        }
    }
}