using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// StorageMachineInterface, StorageInventoryIterationInterface

namespace nialsorva.FCEEasyMods
{ /*
    class Port { }

    public class ConveyorLikeMachine : ModMachineEntity, ItemConsumerInterface, ItemSupplierInterface
    {
        
        public ConveyorLikeMachine(ModCreateSegmentEntityParameters parameters, SpawnableObjectEnum spawnable)
            : base(parameters,spawnable)
        { }
        public ConveyorLikeMachine(ModCreateSegmentEntityParameters parameters, SpawnableObjectEnum spawnable, eSegmentEntity segEntity) :
            base(parameters, spawnable,segEntity)
        {
        }

        // From ItemSupplierInterface
        public bool PeekItem(StorageUserInterface sourceEntity, out ItemBase item, out ushort cubeType, out ushort cubeValue)
        {
            throw new NotImplementedException();
        }

        // From ItemSupplierInterface
        // DELIVER an item to sourceEntity
        public bool TryTakeItem(StorageUserInterface sourceEntity, out ItemBase item, out ushort cubeType, out ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            return false;            
        }

        // From ItemConsumerInterface
        // RECEIVE an item from SourceEntity
        public bool TryDeliverItem(StorageUserInterface sourceEntity, ItemBase item, ushort cubeType, ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            if (outputPort.TryTakeItem(sourceEntity, out item, out cubeType, out cubeValue))
            {
                StartAnimationCoroutine(CarryItem());
                if (sendImmediateNetworkUpdate)
                {
                    this.RequestImmediateNetworkUpdate();
                }
                return true;
            }
            return false;
        }

        float totalCarryTime=1f;
        Vector3 forward;
        GameObject mCarriedObjectParent;
        
        public IEnumerable<int> CarryItem()
        {
            float carryTimer=0f;
            isWorking = true;
            // Animate the carrying
            while (carryTimer < totalCarryTime)
            {
                carryTimer += Time.deltaTime;
                AnimateCarriedItem(carryTimer / totalCarryTime);
                yield return 0;
            }
            isWorking = false;
        }

        protected virtual void AnimateCarriedItem(float completion)
        {
            this.mCarriedObjectParent.transform.localPosition = new Vector3(0f, 0.10025f, completion);
            this.mCarriedObjectParent.transform.forward = this.forward;
        }

        public IEnumerable<int> OffloadCargo()
        {
            while (!outputPort.TryDeliverItem(this, mCarriedItem, mCarriedCube, mCarriedValue))
            {
                yield return 0;
            }
            this.mCarriedItem = null;
            this.mCarriedCube = 0;
            this.mCarriedValue = 0;
            this.RequestImmediateNetworkUpdate();
        }
    } */
}
