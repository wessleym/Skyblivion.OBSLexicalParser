using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceItem
    {
        public TES5BasicType? ParentType { get; }
        public TES5InheritanceItemCollection Items { get; }
        public TES5InheritanceItem(TES5BasicType? parentType, IEnumerable<TES5InheritanceItem> items)
        {
            this.ParentType = parentType;
            this.Items = new TES5InheritanceItemCollection(items);
        }
        public TES5InheritanceItem(TES5BasicType parentType)
            : this(parentType, new TES5InheritanceItem[] { })
        { }

        public bool AnyItems => Items.Any();
        public TES5InheritanceItemCollection ItemsWithoutSubItems => Items.ItemsWithoutSubItems;
    }
}
