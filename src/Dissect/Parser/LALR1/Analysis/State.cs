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
        protected List<Item> items = new List<Item>();
        protected Dictionary<int, Dictionary<int, Item>> itemMap = new Dictionary<int, Dictionary<int, Item>>();
        protected int number;
        /*
        * Constructor.
         *
         *  The number identifying this state.
         *  The initial items of this state.
        */
        public State(int number, Item[] items)
        {
            this.number = number;
            foreach (var item in items)
            {
                this.add(item);
            }
        }

        /*
        * Adds a new item to this state.
         *
         *  The new item.
        */
        public void add(Item item)
        {
            this.items.Add(item);
            this.itemMap[item.getRule().getNumber()][item.getDotIndex()] = item;
        }

        /*
        * Returns an item by its rule number and dot index.
         *
         *  The number of the rule of the desired item.
         *  The dot index of the desired item.
         *
         *  The item.
        */
        public Item get(int ruleNumber, int dotIndex)
        {
            return this.itemMap[ruleNumber][dotIndex];
        }

        /*
        * Returns the number identifying this state.
        */
        public int getNumber()
        {
            return this.number;
        }

        /*
        * Returns an array of items constituting this state.
         *
         *  The items.
        */
        public List<Item> getItems()
        {
            return this.items;
        }
    }
}