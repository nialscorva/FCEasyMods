using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{ 

/*    public class InventoryItem
    {
        public ItemBase item;
        public ushort cubeType;
        public ushort cubeValue;

        public int Decrement()
        {
            if(item == null)
            {
                return 0;
            }
            ItemCubeStack ics = item as ItemCubeStack;
            if(ics !=null)
            {
                return --ics.mnAmount;
            }
            ItemStack stack = item as ItemStack;
            if (stack != null)
            {
                return --stack.mnAmount;
            }
            return 0;
        }
    }*/

    public class InventoryMachine : ModMachineEntity, ItemConsumerInterface, ItemSupplierInterface, StorageConsumerInterface, StorageSupplierInterface
    {
        //=======================================================
        // Inventory
        //=======================================================
        ItemBase[] inventory = new ItemBase[1];
        List<InputPort> inputPorts = new List<InputPort>();
        List<OutputPort> outputPorts = new List<OutputPort>();

        public InventoryMachine(ModCreateSegmentEntityParameters parameters, SpawnableObjectEnum spawnable) 
            : base(parameters,spawnable) {
        }

        public InventoryMachine(ModCreateSegmentEntityParameters parameters, SpawnableObjectEnum spawnable, eSegmentEntity segEntity) 
            : base(parameters, spawnable, segEntity)
        {

        }

        public T AddInputPort<T>(T port) where T : InputPort
        {
            return port;
        }

        public T AddOutputPort<T>(T port) where T : OutputPort
        {
            return port;
        }



        // sourceEntity wants to DELIVER the item to us
        public bool TryDeliverItem(StorageUserInterface storageUserInterface, ItemBase item, ushort cubeType, ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            SegmentEntity sourceEntity = storageUserInterface as SegmentEntity;
            if (sourceEntity == null)
            {
                return false;
            }
            if(item==null)
            {
                item = ItemManager.SpawnCubeStack(cubeType, cubeValue, 1);
            }
            Vector3 relativePosition = SegmentCustomRenderer.GetRotationQuaternion(mFlags) * new Vector3(sourceEntity.mnX - mnZ, sourceEntity.mnY - mnZ, sourceEntity.mnZ - mnZ);

            InputPort port = inputPorts.First(p => p.Allow(sourceEntity, relativePosition,item));
            if (port != null)
            {
                port.Take(item);
            }
            return false;
        }
        
        public void ProcessStorageConsumer(StorageMachineInterface storage)
        {
            SegmentEntity sourceEntity = storage as SegmentEntity;
            if (sourceEntity == null) return;
            Vector3 relativePosition = SegmentCustomRenderer.GetRotationQuaternion(mFlags) * new Vector3(sourceEntity.mnX - mnZ, sourceEntity.mnY - mnZ, sourceEntity.mnZ - mnZ);

            InputPort port = inputPorts.First(p => p.Allow(sourceEntity, relativePosition));

            if (port != null)
            {
                port.Take(storage);
            }
        }



        // sourceEntity wants to look at our goods
        public bool PeekItem(StorageUserInterface sourceEntity, out ItemBase item, out ushort cubeType, out ushort cubeValue)
        {
            throw new NotImplementedException();
        }

        protected ItemBase FindItemToDeliver(SegmentEntity sourceEntity)
        {
            if (sourceEntity == null) return null;

            Vector3 relativePosition = SegmentCustomRenderer.GetRotationQuaternion(mFlags) * new Vector3(sourceEntity.mnX - mnZ, sourceEntity.mnY - mnZ, sourceEntity.mnZ - mnZ);
            foreach (OutputPort port in outputPorts)
            {
                if (port.AllowedEntity(sourceEntity, relativePosition))
                {
                    ItemBase item=port.ChooseItem(inventory);

                    int givenItem = Array.FindIndex(inventory, port.AllowedItem);
                    if (givenItem > 0)
                    {
                        ItemBase item = ItemManager.CloneItem(inventory[givenItem], 1);
                        int count = ItemManager.GetCurrentStackSize(item) - 1;
                        if (count < 1)
                        {
                            inventory[givenItem] = null;
                        }
                        else
                        {
                            ItemManager.SetItemCount(item, count);
                        }
                        return item;
                    }
                }
            }
            return null;
        }

        // sourceEntity wants to TAKE an item from us
        public bool TryTakeItem(StorageUserInterface storageUserInterface, out ItemBase item, out ushort cubeType, out ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            // we don't deal with cubeType & value.  Everything gets converted to an ItemBase type when inserted.
            cubeType = cubeValue = 0;

            item = FindItemToDeliver(storageUserInterface as SegmentEntity);
            return item != null;
        }


        public void ProcessStorageSupplier(StorageMachineInterface storage)
        {
            SegmentEntity sourceEntity = storage as SegmentEntity;
            if (sourceEntity == null) return;

            ItemBase item = FindItemToDeliver(storage as SegmentEntity);

            if(item !=null && !storage.TryInsert(this, item))
            {
                // failed to insert
                // TODO: add it back to us
                UnityEngine.Debug.Log("Failed to insert the object, losing " + item.ToString());
            }
        }


    }
}
