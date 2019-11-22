using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceItem
    {
        public string? Name { get; private set; }
        public TES5InheritanceItemCollection Items { get; private set; }
        public TES5InheritanceItem(string? name, IEnumerable<TES5InheritanceItem> items)
        {
            this.Name = name;
            this.Items = new TES5InheritanceItemCollection(items);
        }
        public TES5InheritanceItem(string name)
            : this(name, new TES5InheritanceItem[] { })
        { }

        public bool AnyItems => Items.Any();
        public TES5InheritanceItemCollection ItemsWithoutSubItems => Items.ItemsWithoutSubItems;
    }
}
