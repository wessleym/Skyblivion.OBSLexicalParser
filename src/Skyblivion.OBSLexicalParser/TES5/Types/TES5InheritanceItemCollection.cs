using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceItemCollection : IEnumerable<TES5InheritanceItem>
    {
        private List<TES5InheritanceItem> list;
        public TES5InheritanceItemCollection(IEnumerable<TES5InheritanceItem> items)
        {
            list = new List<TES5InheritanceItem>(items);
        }
        public TES5InheritanceItemCollection()
            : this(new TES5InheritanceItem[] { })
        { }

        public void Add(TES5InheritanceItem item)
        {
            list.Add(item);
        }

        public void Add(string name, IEnumerable<TES5InheritanceItem> items)
        {
            Add(new TES5InheritanceItem(name, items));
        }

        public void Add(string name)
        {
            Add(new TES5InheritanceItem(name));
        }

        public IEnumerator<TES5InheritanceItem> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public TES5InheritanceItemCollection ItemsWithoutSubItems => new TES5InheritanceItemCollection(list.Where(i => !i.AnyItems));
    }
}
