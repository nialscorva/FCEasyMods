using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{ 
    public class InventoryMachine : ModMachineEntity, ItemConsumerInterface, StorageMachineInterface
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

        public InventoryMachine(ModCreateSegmentEntityParameters parameters) 
            : base(parameters) {
        }

        // sourceEntity wants to DELIVER the item to us
        public bool TryDeliverItem(StorageUserInterface storageUserInterface, ItemBase item, ushort cubeType, ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            return false;
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

        public bool TryInsert(InventoryInsertionOptions options, ref InventoryInsertionResults results)
        {
            if(results==null)
            {
                results = new InventoryInsertionResults();
            }
            if (options.Item != null)
            {
                int stackSize = ItemManager.GetCurrentStackSize(options.Item);
                if (!options.AllowPartialInsertion && stackSize > RemainingCapacity)
                {
                    return false;
                }
                results.AmountInserted = TryPartialInsert(options.SourceEntity, ref options.Item, false, false);
                results.AmountRemaining = stackSize-results.AmountInserted;
                return true;
            } else if (options.Amount <= RemainingCapacity || options.AllowPartialInsertion)
            {
                results.AmountInserted = TryPartialInsert(options.SourceEntity, options.Cube, options.Value, options.Amount);
                results.AmountRemaining = options.Amount-results.AmountInserted;
                return true;
            }
            return false;
        }

        public bool TryInsert(StorageUserInterface sourceEntity, ItemBase item)
        {
            throw new NotImplementedException();
        }

        public bool TryInsert(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public int TryPartialInsert(StorageUserInterface sourceEntity, ref ItemBase item, bool alwaysCloneItem, bool updateSourceItem)
        {
            throw new NotImplementedException();
        }

        public int TryPartialInsert(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public bool TryExtract(InventoryExtractionOptions options, ref InventoryExtractionResults results)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractCubes(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractItems(StorageUserInterface sourceEntity, int itemId, int amount, out ItemBase item)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractItems(StorageUserInterface sourceEntity, int itemId, int amount)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount, out ItemBase item)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractAny(StorageUserInterface sourceEntity, int amount, out ItemBase item)
        {
            throw new NotImplementedException();
        }

        public int TryPartialExtractCubes(StorageUserInterface sourceEntity, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public int TryPartialExtractItems(StorageUserInterface sourceEntity, int itemId, int amount, out ItemBase item)
        {
            throw new NotImplementedException();
        }

        public int TryPartialExtractItems(StorageUserInterface sourceEntity, int itemId, int amount)
        {
            throw new NotImplementedException();
        }

        public int TryPartialExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount, out ItemBase item)
        {
            throw new NotImplementedException();
        }

        public int TryPartialExtractItemsOrCubes(StorageUserInterface sourceEntity, int itemId, ushort cube, ushort value, int amount)
        {
            throw new NotImplementedException();
        }

        public int CountItems(InventoryExtractionOptions options)
        {
            throw new NotImplementedException();
        }

        public int CountItems(int itemId, ushort cube, ushort value)
        {
            throw new NotImplementedException();
        }

        public int CountItems(int itemId)
        {
            throw new NotImplementedException();
        }

        public int CountCubes(ushort cube, ushort value)
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
    }
}
