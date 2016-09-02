using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialscorva.FCEEasyMods
{
    public interface ItemFilter
    {
        string Name { get; }
        bool Check(ItemBase i);
    }

    public static class ItemFilters
    {
        public static bool IsBar(ItemBase i)
        {
            return i.mnItemID == ItemEntries.CopperBar || i.mnItemID == ItemEntries.TinBar || i.mnItemID == ItemEntries.IronBar || i.mnItemID == ItemEntries.LithiumBar
             || i.mnItemID == ItemEntries.TitaniumBar || i.mnItemID == ItemEntries.NickelBar || i.mnItemID == ItemEntries.GoldBar
             || i.mnItemID == ItemEntries.ChromiumBar || i.mnItemID == ItemEntries.MolybdenumBar;
        }
        public static bool IsCrafted(ItemBase i)
        {
            return i is ItemSingle || i is ItemDurability || i is ItemStack;
        }
        public static bool IsHighCalorie(ItemBase i)
        {
            return i is ItemCubeStack && CubeHelper.IsHighCalorie((i as ItemCubeStack).mCubeType);
        }
        public static bool IsGarbage(ItemBase i)
        {
            return i is ItemCubeStack && CubeHelper.IsGarbage((i as ItemCubeStack).mCubeType);
        }
        public static bool IsOre(ItemBase i)
        {
            return i is ItemCubeStack && CubeHelper.IsOre((i as ItemCubeStack).mCubeType);
        }
        public static bool IsCrystal(ItemBase i)
        {
            return i is ItemCubeStack && (i as ItemCubeStack).mCubeType == 152;
        }
        public static bool IsBiomass(ItemBase i)
        {
            return i is ItemCubeStack && (i as ItemCubeStack).mCubeType == 153;
        }

        public static bool IsGems(ItemBase i)
        {
            return i is ItemCubeStack && (i as ItemCubeStack).mCubeType == 162;
        }
        public static bool IsOrganic(ItemBase i)
        {
            return i.mnItemID >= 4000 && i.mnItemID <= 4101;
        }
        public static bool IsMachineBlock(ItemBase i)
        {
            return i is ItemCubeStack && (CubeHelper.IsMultiBlockMachine((i as ItemCubeStack).mCubeType) || CubeHelper.IsMachine((i as ItemCubeStack).mCubeType));
        }
        public static bool IsSmeltable(ItemBase i)
        {
            return i is ItemCubeStack && CubeHelper.IsIngottableOre((i as ItemCubeStack).mCubeType);
        }
        public static bool IsResearchable(ItemBase i)
        {
            if (i is ItemCubeStack)
            {
                TerrainDataEntry terrainDataEntry = global::TerrainData.mEntries[(i as ItemCubeStack).mCubeType];
                return terrainDataEntry != null && terrainDataEntry.DecomposeValue > 0;
            }
            else if (i.mnItemID > 0 && i.mnItemID < ItemEntry.mEntries.Length)
            {
                ItemEntry itemEntry = ItemEntry.mEntries[i.mnItemID];
                return itemEntry != null && itemEntry.DecomposeValue > 0;
            }
            else
            {
                return false;
            }
        }
        public static ItemFilter ALL = new ItemDelegateFilter("All", (i) => true);
        public static ItemFilter NONE = new ItemDelegateFilter("Nothing", (i) => false);
    }

    public class ItemDelegateFilter : ItemFilter
    {
        public string Name { get; protected set; }
        protected Func<ItemBase, bool> checkFunction;

        public bool Check(ItemBase i) { return checkFunction(i); }
        

        public ItemDelegateFilter(string name, Func<ItemBase, bool> check)
        {
            Name = name;
            checkFunction = check;
        }
    }

    public class ItemFilterSet : ItemFilter
    {
        public string Name
        {
            get
            {
                return Filters[Index].Name;
            }
        }

        public string NextName
        {
            get { return Filters[Normalize(Index + 1)].Name; }
        }
        public string PrevName
        {
            get { return Filters[Normalize(Index - 1)].Name; }
        }
        public bool Check(ItemBase i)
        {
            return Filters[Index].Check(i);
        }

        public ItemFilter[] Filters { get; protected set; }
        public int Index { get; protected set; }

        public ItemFilterSet(params ItemFilter[] filters) : this(0, filters) { }
        public ItemFilterSet(int index,params ItemFilter[] filters)
        {
            this.Filters = filters;
            this.Index = index;
        }
        public int Normalize(int i)
        {
            return (i % Filters.Length + Filters.Length) % Filters.Length;
        }
        public void Next()
        {
            Index = Normalize(Index + 1);
        }
        public void Prev()
        {
            Index = Normalize(Index - 1);
        }
    }

    /*
     * Modeled after the eHopperRequestType in order, naming, and effect:

            eAny,
            eHighCalorieOnly,
            eGarbage,
            eOreOnly,
            eBarsOnly,
            eAnyCraftedItem,
            eCrystals,
            eBioMass,
            eNone,
            eGems,
            eOrganic,
            eSmeltable,
            eResearchable,
    */
    public class ConveyorItemFilterSet : ItemFilterSet
    {
        public ConveyorItemFilterSet() : this(0) { }

        public static ItemFilter[] HOPPER_REQUEST_TYPE = new ItemFilter[] {
            ItemFilters.ALL,
            new ItemDelegateFilter("Combustible", ItemFilters.IsHighCalorie),
            new ItemDelegateFilter("Garbage", ItemFilters.IsGarbage),
            new ItemDelegateFilter("Ore", ItemFilters.IsOre),
            new ItemDelegateFilter("Bars Only", ItemFilters.IsBar),
            new ItemDelegateFilter("Crafted Items", ItemFilters.IsCrafted),
            new ItemDelegateFilter("Crystals", ItemFilters.IsCrafted),
            new ItemDelegateFilter("Biomass", ItemFilters.IsBiomass),
            ItemFilters.NONE,
            new ItemDelegateFilter("Crystals", ItemFilters.IsGems),
            new ItemDelegateFilter("Organic Items", ItemFilters.IsOrganic),
            new ItemDelegateFilter("Smeltable", ItemFilters.IsSmeltable),
            new ItemDelegateFilter("Researchable", ItemFilters.IsResearchable)
        };
        public ConveyorItemFilterSet(int index)
            : base(index,HOPPER_REQUEST_TYPE)
        {
        
        }
        public eHopperRequestType ToHopperRequestType()
        {
            return (eHopperRequestType)Index;
        }
    }




}
