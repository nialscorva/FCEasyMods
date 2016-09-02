using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialscorva.FCEEasyMods.Behaviors
{
    public class MassStorageInventoryBehavior : IEnumerable<KeyValuePair<string,int>>
    {
        public MassStorageCrate Crate;
        public ItemFilter Filter;

        public SortedDictionary<string, int> Items { get; } = new SortedDictionary<string, int>();
        public int CubeCount { get; protected set; }
        public int ItemCount { get { return Items.Count; } }

        public MassStorageInventoryBehavior() : this(ItemFilters.ALL) { }
        public MassStorageInventoryBehavior(ItemFilter filter)
        {
            Filter = filter;
        }

        public MassStorageInventoryBehavior(MassStorageCrate crate) : this(crate,ItemFilters.ALL) { }
        public MassStorageInventoryBehavior(MassStorageCrate crate,ItemFilter filter)
        {
            this.Crate = crate;
            this.Filter = filter;
        }
        
        public IDictionary<string, int> Update()
        {
            CubeCount = 0;
            Items.Clear();
            MassStorageCrate center = Crate?.mConnectedCenter;
            if(center == null)
            {
                return Items;
            }
            foreach (MassStorageCrate crate in center.mConnectedCrates)
            {
                ItemBase pickeditem = null;
                CubeCount++;
                pickeditem = crate.mItem;
                if (pickeditem != null && Filter.Check(pickeditem))
                {
                    string itemname = ItemManager.GetItemName(pickeditem);

                    if (!Items.ContainsKey(itemname))
                    {
                        Items.Add(itemname, ItemManager.GetCurrentStackSize(pickeditem));
                    }
                    else
                    {
                        Items[itemname] = (int)Items[itemname] + ItemManager.GetCurrentStackSize(pickeditem);
                    }
                    pickeditem = null;
                }
            }
            return Items;
        }

        public IEnumerator<System.Collections.Generic.KeyValuePair<string,int>> GetEnumerator()
        {
            return Update().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Update().GetEnumerator();
        }

        public bool IsValid()
        {
            if(Crate == null || Crate.mbDelete)
            {
                Crate = null;
                return false;
            }
            return true;
        }
        public bool IsReady()
        {
            return Crate?.mConnectedCenter != null;
        }
    }
}
