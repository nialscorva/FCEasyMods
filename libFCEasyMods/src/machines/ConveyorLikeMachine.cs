using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// StorageMachineInterface, StorageInventoryIterationInterface

namespace nialscorva.FCEEasyMods
{
    using StateCoroutine = IEnumerator<ModMachineEntity.StateReturn>;
    using Coroutine = IEnumerator<int>;

    public class ConveyorLikeMachine : ModMachineEntity, ItemConsumerInterface, ItemSupplierInterface
    {
        public static int FRONT = 0;
        public static int BACK = 1;
        public static int LEFT = 2;
        public static int RIGHT = 3;
        public static int TOP = 4;
        public static int BOTTOM = 5;

        protected ItemBase carriedItem = null;

        protected EntityFilter outputLocation;
        protected EntityFilter inputLocation;

        protected int ConveyorState;
        protected float totalCarryTime = 1f;

        GameObject movingItemParentGO;
        GameObject movingItemGO;
        UVScroller_PG scroller;
        
        public ConveyorLikeMachine(ModCreateSegmentEntityParameters parameters)
            : base(parameters)
        {
            outputLocation = new RelativeLocation(this, Vector3.forward);
            inputLocation = new RelativeLocation(this, Vector3.back);

            SetAnimationState(AnimateStill());
        }

        protected override void InitializeGameObjects()
        {
            movingItemParentGO = primaryGO?.GetDescendantGameObject("Moving Item");
            movingItemGO = primaryGO?.GetDescendantGameObject("Moving Item Obj");
            scroller = primaryGO?.GetComponent<UVScroller_PG>();
        }


        // From ItemSupplierInterface
        public bool PeekItem(StorageUserInterface storageUserInterface, out ItemBase item, out ushort cubeType, out ushort cubeValue)
        {
            cubeType = cubeValue = 0;
            SegmentEntity sourceEntity = storageUserInterface as SegmentEntity;

            if (sourceEntity == null || carriedItem == null || !outputLocation.Allow(sourceEntity))
            {
                //                ModLoader.log("PeekItem failed in precheck: sourceEntity == {0}, carriedItem=={1}, inputPort.Allow == {2}", sourceEntity, carriedItem, inputLocation.Allow(sourceEntity));
                item = null;
                return false;
            }

            item = carriedItem;
            return true;
        }

        // From ItemSupplierInterface
        // sourceEntity wants us to DELIVER an item to them
        public bool TryTakeItem(StorageUserInterface storageUserInterface, out ItemBase item, out ushort cubeType, out ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            cubeValue = cubeType = 0;
            SegmentEntity sourceEntity = storageUserInterface as SegmentEntity;


            // check to see if we have carried the item fully!
            if (sourceEntity == null || carriedItem == null || !outputLocation.Allow(sourceEntity))
            {
//                ModLoader.log("TryTakeItem failed in precheck: sourceEntity == {0}, carriedItem=={1}, inputPort.Allow == {2}", sourceEntity, carriedItem, inputLocation.Allow(sourceEntity));
                item = null;
                return false;
            }


            item = carriedItem;
            carriedItem = null;
            if (sendImmediateNetworkUpdate)
            {
                this.RequestImmediateNetworkUpdate();
            }
            return true;
        }

        // From ItemConsumerInterface
        // RECEIVE an item from SourceEntity
        public bool TryDeliverItem(StorageUserInterface storageUserInterface, ItemBase item, ushort cubeType, ushort cubeValue, bool sendImmediateNetworkUpdate)
        {
            SegmentEntity sourceEntity = storageUserInterface as SegmentEntity;

            if (sourceEntity == null || carriedItem != null || !inputLocation.Allow(sourceEntity))
            {
//                ModLoader.log("TryDeliverItem failed in precheck: sourceEntity == {0}, carriedItem=={1}, inputPort.Allow == {2}", sourceEntity, carriedItem, inputLocation.Allow(sourceEntity));
                return false;
            }
            if (item == null)
            {
                item = ItemManager.SpawnCubeStack(cubeType, cubeValue, 1);
            }
            
            carriedItem = item;
            SetAnimationState(AnimateCarryItem());
            if (sendImmediateNetworkUpdate)
            {
                this.RequestImmediateNetworkUpdate();
            }
            return true;
        }

        public Coroutine ScanForHoppers()
        {
            while(true)
            {
                StorageMachineInterface smi;

                smi = GetEntityAt<StorageMachineInterface>(Back);
                yield return 0;

                smi = GetEntityAt<StorageMachineInterface>(Left);
                yield return 0;

                smi = GetEntityAt<StorageMachineInterface>(Right);
                yield return 0;

                smi = GetEntityAt<StorageMachineInterface>(Up);
                yield return 0;

                smi = GetEntityAt<StorageMachineInterface>(Down);
                yield return 0;
            }
        }
        //==============================================================================
        // State & animation coroutines
        //==============================================================================
        public StateCoroutine AnimateStill()
        {
            primaryGO?.GetDescendantGameObject("Moving Item").SetActive(false);
            
            movingItemGO?.SetActive(false);
            movingItemParentGO?.SetActive(false);

            if (scroller != null)
            {
                scroller.scrollSpeed = 0f;
            }
            yield break;
        }

        public StateCoroutine AnimateCarryItem()
        {
            float carryTimer=0f;
                        
            movingItemGO?.SetActive(true);
            movingItemParentGO?.SetActive(true);
            if (scroller != null)
            {
                scroller.scrollSpeed = 1f;
            }

            while (carryTimer < totalCarryTime)
            {
                carryTimer += Time.deltaTime;
                
                movingItemParentGO.transform.localPosition = new Vector3(0f, 0.10025f, carryTimer / totalCarryTime -0.5f);
                movingItemParentGO.transform.forward = this.Forward;
                yield return 0;
            }
            SetState(OffloadCargo());
            yield return NextState(AnimateStill());
        }


        public StateCoroutine OffloadCargo()
        {
            while (carriedItem != null)
            {
                if(GetEntityAt<ItemConsumerInterface>(Forward)?.TryDeliverItem(this, carriedItem, 0, 0, true) == true)
                {
                    carriedItem = null;
                    this.RequestImmediateNetworkUpdate();
                    yield return NextState(ReceiveCargo());
                }

                yield return 0;
            }
        }

        public StateCoroutine ReceiveCargo()
        {
            // do nothing
            yield break;
        }
    }
}
