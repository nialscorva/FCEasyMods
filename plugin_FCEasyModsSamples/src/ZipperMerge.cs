using nialsorva.FCEEasyMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using StateCoroutine = System.Collections.Generic.IEnumerable<nialsorva.FCEEasyMods.ModMachineEntity.StateReturn>;

namespace nialscorva.FCEasyMods.Samples
{
    [FCESegmentEntity("FCEasyModsSamples.zipperMerge1")]
    [FCETerrainData(
        Name="Sample Zipper Merge",
		IconName="Zipper Merge",
	    Description="Replicates Zipper Merge Functionality"
    )]
    class ZipperMerge : InventoryMachine, ItemConsumerInterface, ItemSupplierInterface
    {
        [FCECubeType]
        public static int CUBE_TYPE { get; protected set; }

        [FCECubeValue]
        public static int CUBE_VALUE { get; protected set; }

        protected FaceInputPort leftPort;
        protected FaceInputPort rightPort;
        protected FaceOutputPort outputPort;

        protected float totalCarryTime = 1f;
        protected GameObject mCarriedObjectParent;



        public ZipperMerge(ModCreateSegmentEntityParameters parameters) :
            base(parameters, SpawnableObjectEnum.Zipper_Merge)
        {
            mbNeedsLowFrequencyUpdate = true;
            mbNeedsUnityUpdate = true;

            leftPort = AddInputPort(new FaceInputPort(this, Vector3.left));
            rightPort = AddInputPort(new FaceInputPort(this, Vector3.right));
            outputPort = AddOutputPort(new FaceOutputPort(this, Vector3.forward));

            InitialState(this.AcceptLeftState());
            InitialVisualState(this.DefaultVisualState());
            
        }


        protected StateCoroutine DefaultVisualState()
        {
            primaryGO.GetComponent<MeshRenderer>("ZipperObject")?.material.SetColor("_Color", Color.green);
            
            // Make the input ports point the wrong way just to be annoying
            primaryGO.GetComponent<MeshRenderer>("Merger_Conveyor_L")?.transform.Rotate(0, 180, 0);
            primaryGO.GetComponent<MeshRenderer>("Merger_Conveyor_R")?.transform.Rotate(0, 180, 0);

              // only works for ConveyorBelts    
            var s = primaryGO.GetComponent<UVScroller_PG>("ConveyorBelt");
            if (s != null) s.scrollSpeed = 1f;


            yield break;
        }

        protected StateCoroutine AcceptLeftState()
        {
            leftPort.AcceptOne();

            while (leftPort.IsActive())
            {
                yield return 0;
            }

            // TODO: StartAnimationCoroutine to animate the item moving across the left input side

            yield return NextState(CarryState(AcceptLeftState()));
        }

        protected StateCoroutine AcceptRightState()
        {
            rightPort.AcceptOne();

            while(rightPort.IsActive())
            {
                yield return 0;
            }
            
            yield return NextState(CarryState(AcceptLeftState()));
        }


        protected StateCoroutine CarryState(StateCoroutine followupState)
        {
            float carryTimer = 0f;
            // TODO: start the conveyor animation...

            while (carryTimer < totalCarryTime)
            {
                carryTimer += LowFrequencyThread.mrPreviousUpdateTimeStep;
                yield return 0;
            }
            // TODO: end the conveyor animation

            yield return NextState(DeliverItemState(followupState));
        }
        
        protected StateCoroutine DeliverItemState(StateCoroutine followupState)
        {
            outputPort.DeliverOne();

            while (outputPort.IsActive())
            {
                yield return 0;
            }

            yield return NextState(followupState);
        }
    }
}
