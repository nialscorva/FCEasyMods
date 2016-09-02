using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialscorva.FCEEasyMods
{ 
    public class StorageHopperMachine : ModMachineEntity, ItemConsumerInterface, StorageMachineInterface
    {
        //=======================================================
        // Inventory
        //=======================================================
        protected ItemBase[] inventory = new ItemBase[1];
        protected int mTotalCapacity = 100;
        protected eHopperPermissions mPermissions = eHopperPermissions.AddAndRemove;
        protected bool operationPermitted = true;

        public int TotalCapacity  { get { return mTotalCapacity; }}
        public int UsedCapacity  {  get { return inventory.Sum((ItemBase e) => ItemManager.GetCurrentStackSize(e)); }   }
        public int RemainingCapacity  { get { return TotalCapacity - UsedCapacity; } }
        public bool InventoryExtractionPermitted  { get { return operationPermitted; } }

        public StorageHopperMachine(ModCreateSegmentEntityParameters parameters) 
            : base(parameters) {
        }

        public eHopperPermissions GetPermissions()
        {
            return mPermissions;
        }

        public bool IsEmpty()
        {
            return UsedCapacity == 0;
        }

        public bool IsFull()
        {
            return RemainingCapacity == 0;
        }

        public bool IsNotEmpty()
        {
            return UsedCapacity != 0;
        }

        public bool IsNotFull()
        {
            return RemainingCapacity != 0;
        }

        public bool TryExtract(eHopperRequestType lType, int exemplarItemId, ushort exemplarCubeType, ushort exemplarCubeValue, bool invertExemplar, int minimumAmount, int maximumAmount, bool knownItemsOnly, bool countOnly, bool trashItems, bool convertCubesToItems, bool ignorePower, out ItemBase returnedItem, out ushort returnedCubeType, out ushort returnedCubeValue, out int returnedAmount)
        {
            throw new NotImplementedException();
        }

        private void TryInsert(ItemBase item, ushort cube, ushort value, int amount, bool allowPartialInsertion, bool v1, bool v2, out int num, out int amountRemaining)
        {
            throw new NotImplementedException();
        }
        
        public int UnloadToList(List<ItemBase> cargoList, int amountToExtract)
        {
            throw new NotImplementedException();
        }

        public void IterateContents(IterateItem itemFunc, object state)
        {
            throw new NotImplementedException();
        }

        public bool TryExtract(InventoryExtractionOptions options, ref InventoryExtractionResults results)
        {
            ItemBase item;
            ushort cube;
            ushort value;
            int amount;
            if (!this.TryExtract(options.RequestType, options.ExemplarItemID, options.ExemplarBlockID, options.ExemplarBlockValue, options.InvertExemplar, options.MinimumAmount, options.MaximumAmount, options.KnownItemsOnly, false, false, options.ConvertToItem, options.IgnorePower, out item, out cube, out value, out amount))
            {
                results.Item = null;
                results.Amount = 0;
                results.Cube = 0;
                results.Value = 0;
                return false;
            }
            results.Cube = cube;
            results.Value = value;
            results.Amount = amount;
            results.Item = item;
            return true;
        }
        public bool TryExtractCubes(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, -1, cube, value, false, amount, amount, false, false, false, false, false, out itemBase, out num, out num2, out num3);
        }

        public bool TryExtractItems(StorageUserInterface sourceEntity, int itemId, int amount, out ItemBase item)
        {
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, itemId, 0, 0, false, amount, amount, false, false, false, true, false, out item, out num, out num2, out num3);
        }

        public bool TryExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount, out ItemBase item)
        {
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, itemId, cube, value, false, amount, amount, false, false, false, true, false, out item, out num, out num2, out num3);
        }

        public bool TryExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, itemId, cube, value, false, amount, amount, false, false, true, false, false, out itemBase, out num, out num2, out num3);
        }

        public bool TryExtractItems(StorageUserInterface sourceEntity, int itemId, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, itemId, 0, 0, false, amount, amount, false, false, true, false, false, out itemBase, out num, out num2, out num3);
        }

        public int TryPartialExtractCubes(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            bool flag = this.TryExtract(eHopperRequestType.eAny, -1, cube, value, false, 1, amount, false, false, false, false, false, out itemBase, out num, out num2, out result);
            return result;
        }

        public int TryPartialExtractItems(StorageUserInterface sourceEntity, int itemId, int amount, out ItemBase item)
        {
            ushort num;
            ushort num2;
            int result;
            this.TryExtract(eHopperRequestType.eAny, itemId, 0, 0, false, 1, amount, false, false, false, true, false, out item, out num, out num2, out result);
            return result;
        }

        public int TryPartialExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount, out ItemBase item)
        {
            ushort num;
            ushort num2;
            int result;
            this.TryExtract(eHopperRequestType.eAny, itemId, cube, value, false, 1, amount, false, false, false, true, false, out item, out num, out num2, out result);
            return result;
        }

        public int TryPartialExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            this.TryExtract(eHopperRequestType.eAny, itemId, cube, value, false, 1, amount, false, false, false, false, false, out itemBase, out num, out num2, out result);
            return result;
        }

        public int TryPartialExtractItems(StorageUserInterface sourceEntity, int itemId, int amount)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            this.TryExtract(eHopperRequestType.eAny, itemId, 0, 0, false, 1, amount, false, false, true, false, false, out itemBase, out num, out num2, out result);
            return result;
        }

        public bool TryExtractAny(StorageUserInterface sourceEntity, int amount, out ItemBase item)
        {
            ushort num;
            ushort num2;
            int num3;
            return this.TryExtract(eHopperRequestType.eAny, -1, 0, 0, false, amount, amount, false, false, false, true, false, out item, out num, out num2, out num3);
        }


        public bool TryInsert(InventoryInsertionOptions options, ref InventoryInsertionResults results)
        {
            int num;
            int amountRemaining;
            this.TryInsert(options.Item, options.Cube, options.Value, options.Amount, options.AllowPartialInsertion, false, false, out num, out amountRemaining);
            results.AmountInserted = num;
            results.AmountRemaining = amountRemaining;
            return num > 0;
        }
        public bool TryInsert(StorageUserInterface sourceEntity, ItemBase item)
        {
            int num;
            int num2;
            this.TryInsert(item, 0, 0, 0, false, false, false, out num, out num2);
            return num > 0;
        }

        public bool TryInsert(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            int num;
            int num2;
            this.TryInsert(null, cube, value, amount, false, false, false, out num, out num2);
            return num > 0;
        }


        public int TryPartialInsert(StorageUserInterface sourceEntity, ref ItemBase item, bool alwaysCloneItem, bool updateSourceItem)
        {
            int result;
            int num;
            this.TryInsert(item, 0, 0, 0, true, false, alwaysCloneItem, out result, out num);
            if (updateSourceItem && (item is ItemStack || item is ItemCubeStack))
            {
                if (num <= 0)
                {
                    item = null;
                }
                else
                {
                    ItemManager.SetItemCount(item, num);
                }
            }
            else if (updateSourceItem)
            {
                item = null;
            }
            return result;
        }

        public int TryPartialInsert(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            int result;
            int num;
            this.TryInsert(null, cube, value, amount, true, false, false, out result, out num);
            return result;
        }

        public int CountItems(InventoryExtractionOptions options)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            if (!this.TryExtract(options.RequestType, options.ExemplarItemID, options.ExemplarBlockID, options.ExemplarBlockValue, options.InvertExemplar, options.MinimumAmount, options.MaximumAmount, options.KnownItemsOnly, true, false, false, options.IgnorePower, out itemBase, out num, out num2, out result))
            {
                return 0;
            }
            return result;
        }

        public int CountItems(int itemId)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            if (!this.TryExtract(eHopperRequestType.eAny, itemId, 0, 0, false, 0, 2147483647, false, true, false, false, false, out itemBase, out num, out num2, out result))
            {
                return 0;
            }
            return result;
        }

        public int CountItems(int itemId, ushort cube, ushort value)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            if (!this.TryExtract(eHopperRequestType.eAny, itemId, cube, value, false, 0, 2147483647, false, true, false, false, false, out itemBase, out num, out num2, out result))
            {
                return 0;
            }
            return result;
        }

        public int CountCubes(ushort cube, ushort value)
        {
            ItemBase itemBase;
            ushort num;
            ushort num2;
            int result;
            if (!this.TryExtract(eHopperRequestType.eAny, -1, cube, value, false, 0, 2147483647, false, true, false, false, false, out itemBase, out num, out num2, out result))
            {
                return 0;
            }
            return result;
        }

        public bool TryDeliverItem(StorageUserInterface sourceEntity, ItemBase item, ushort cubeType, ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            int num;
            int num2;
            this.TryInsert(item, cubeType, cubeValue, 1, false, false, false, out num, out num2);
            return num > 0;
        }


    }
}
