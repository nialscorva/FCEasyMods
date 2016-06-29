# FCEasyMods

Make a bunch of things declarative in creation of the mod.  The sample mod shows some reference info.

## Goals

### Attribute-driven discovery and dispatch
Information about a new machine is spread across sthe if/switch boilerplate in the FortressCraftMod class, XML files, and implementing class.  This library uses annotations to localize that information to the implementing class.

* Automatic registration of SegmentEntities
* In-work: Automatic registration of item actions
* TODO: Automatic generation of the XML files.  

### Entity State Machines & Update Coroutines

LowFreqencyUpdate() and UnityUpdate() are often a mess of conditions for state management and co-routines.  FCEasyMods lets you define states as co-routines, simplifying the logic dramatically.

* Game logic states with coroutines in LowFrequencyUpdate()
* Animation state coroutine for UnityUpdate()
* General purpose coroutines for both!

### Machine Behaviors
Pre-canned base classes for specific behaviors:

* ConveyorLikeEntity - things that act like conveyors
* HopperLikeEntity - Things that act like storage hoppers
* ManufacturorEntity - Somewhere between conveyors and hoppers, take items in and spit new items out.

### Input and Output ports
Input and output ports allow fine control of items with the machine behaviors and entity state machine.  A port:

* A location relative to the entity (e.g. "adjacent in front", "<64m left", "all immediately adjacent").
* Types of entities that can be connected to that port (e.g. SegmentEntity, ItemConsumerInterface, ConveyorEntity)
* Flow control (e.g. "Accept 4 iron ore, then close", "Accept till inventory full")

## Example: Zipper Merge
Ideally, a Zipper Merge could be implemented like this:

```
   [FCESegmentEntity("FCEasyModsSamples.zipperMerge1")]
    [FCETerrainData(
        Name="Sample Zipper Merge",
		IconName="Zipper Merge",
	    Description="Replicates Zipper Merge Functionality"
    )]
    class ZipperMerge : ConveyorLikeMachine, ItemConsumerInterface, ItemSupplierInterface
    {
        protected FaceInputPort leftPort;
        protected FaceInputPort rightPort;
        protected FaceOutputPort outputPort;

        protected float totalCarryTime = 1f;

        public ZipperMerge(ModCreateSegmentEntityParameters parameters) :
            base(parameters, SpawnableObjectEnum.Zipper_Merge)
        {
            leftPort = AddInputPort(new FaceInputPort(this, Vector3.left));
            rightPort = AddInputPort(new FaceInputPort(this, Vector3.right));
            outputPort = AddOutputPort(new FaceOutputPort(this, Vector3.forward));

            InitialState(this.AcceptLeftState());
            InitialVisualState(this.DefaultVisualState());            
        }

        protected StateCoroutine DefaultVisualState()
        {
						// Make the body of the zipper merge green
						primaryGO.GetComponent<MeshRenderer>("ZipperObject")?.material.SetColor("_Color", Color.green);
            yield break;
        }

        protected StateCoroutine AcceptLeftState()
        {
            leftPort.AcceptOne();
            while (leftPort.IsActive())
            {
                yield return 0;
            }

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
						// TODO: start the conveyor animation state
            while (carryTimer < totalCarryTime)
            {
                carryTimer += LowFrequencyThread.mrPreviousUpdateTimeStep;
                yield return 0;
            }
            // TODO: end the conveyor animation state
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
```

## License

Apache License 2.0