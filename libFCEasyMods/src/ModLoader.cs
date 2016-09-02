using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialscorva.FCEEasyMods
{
    using System.Linq.Expressions;
    using System.Reflection;
    using MachineKey = System.Tuple<int?, int?>;

    class MachineRegistration
    {
        public Type type;
        public SpawnableObjectEnum objectType;
        internal ushort PlacementBlock;
        internal FCEMultiblockMachineAttribute MultiblockAttributes;

        public MachineRegistration(Type type, SpawnableObjectEnum objectType)
        {
            this.type = type;
            this.objectType = objectType;
        }
    }

    public class ModLoader
    {
        private const string LOGGER_PREFIX = "[nialscorva.FCEasyMods] ";
        private const bool DEBUG = true;
        public static void log(String msg, params object[] args)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log(LOGGER_PREFIX + ": " + String.Format(msg, args));
            }
        }
        Dictionary<MachineKey, MachineRegistration> machineById = new Dictionary<MachineKey, MachineRegistration>();
        Dictionary<string, MachineRegistration> machineByKey = new Dictionary<string, MachineRegistration>();

        // Placement block is always 600, distinguished by 
        Dictionary<ushort, MachineRegistration> machineByPlacement = new Dictionary<ushort, MachineRegistration>();
        Dictionary<string, Func<ModItemActionParameters, ModItemActionResults>> itemActionByName = new Dictionary<string, Func<ModItemActionParameters, ModItemActionResults>>();



        public void registerMachine(ModRegistrationData mrd, Type type)
        {
            object[] segEntityAttr = type.GetCustomAttributes(typeof(FCESegmentEntityAttribute), false);
            if (segEntityAttr.Length == 0)
            {
                return;
            }
            FCESegmentEntityAttribute segEntity = segEntityAttr[0] as FCESegmentEntityAttribute;
            
            log("Registering {0} to {1} ...", segEntity.key,type.FullName);
            if(machineByKey.ContainsKey(segEntity.key))
            {
                log("--- Key {0} is already registered to {1}", segEntity.key,machineByKey[segEntity.key].type.FullName);
                return;
            }
            // TODO: confirm that type it has a proper constructor
            mrd.RegisterEntityHandler(segEntity.key);

            // find the cubeType and CubeValue that was assigned
            TerrainDataEntry terrainDataEntry;
            TerrainDataValueEntry terrainDataValueEntry;
            TerrainData.GetCubeByKey(segEntity.key, out terrainDataEntry, out terrainDataValueEntry);
            ushort? cubeType = terrainDataEntry?.CubeType;
            ushort cubeValue = terrainDataValueEntry?.Value ?? 0;

            // add it to our list so that we can create it later
            MachineRegistration mr = new MachineRegistration(type, segEntity.objectType);
            machineById.Add(new MachineKey(cubeType, cubeValue), mr);
            machineByKey.Add(segEntity.key, mr);


            // Check the class to see if it has has global static fields to store the cube and value type in
            MemberInfo[] props = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MemberInfo p in props)
            {
                if (p.GetCustomAttributes(typeof(FCECubeType), false).Length > 0)
                {
                    (p as PropertyInfo)?.SetValue(null, cubeType, null);
                    (p as FieldInfo)?.SetValue(null, cubeType);
                }
                if (p.GetCustomAttributes(typeof(FCECubeValue), false).Length > 0)
                {
                    (p as PropertyInfo)?.SetValue(null, cubeValue, null);
                    (p as FieldInfo)?.SetValue(null, cubeValue);
                }
            }
            
            log("  Registered {0} to {1} with key={2}, value={3}, objectType={4}", segEntity.key, type.Name, cubeType, cubeValue,segEntity.objectType);
        }

       
        public void registerItemActions(ModRegistrationData mrd, Type type)
        {
            MethodInfo[] props = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo p in props)
            {
                object[] itemActions = type.GetCustomAttributes(typeof(FCEItemActionAttribute), false);
                if (itemActions.Length > 0)
                {
                    // Make sure it's the right type of method
                    if(p.ReturnType != typeof(ModItemActionResults))
                    {
                        log("ERROR: FCEItemAction requires that {0}.{1} return ModItemActionResults instead of {2}", type.FullName, p.Name, p.ReturnType.Name);
                        continue;
                    }
                    ParameterInfo[] args=p.GetParameters();
                    if(args.Length != 1 || args[0].ParameterType != typeof(ModItemActionParameters))
                    {
                        log("ERROR: FCEItemAction requires that {0}.{1} take a single ModItemActionParameters argument", type.FullName, p.Name);
                        continue;
                    }

                    // Think this through a bit more...
                    // Is there a point to doing an item action on a whole set of item types?
                    // More than one item action for a given item name?
                    FCEItemActionAttribute itemAction = itemActions[0] as FCEItemActionAttribute;
                    foreach (string itemName in itemAction.itemNames)
                    {
                        itemActionByName.Add(itemName, (Func<ModItemActionParameters,ModItemActionResults>)Delegate.CreateDelegate(typeof(Func<ModItemActionParameters, ModItemActionResults>),type,p));
                    }
                }
            }
                
        }


        public ModRegistrationData ScanAssembly(Assembly assembly)
        {
            return ScanAssembly(assembly, new ModRegistrationData());
        }

        public ModRegistrationData ScanAssembly(Assembly assembly, ModRegistrationData mrd)
        {
            Type[] types = assembly.GetExportedTypes();

            foreach (Type t in types)
            {
                registerMachine(mrd, t);
                registerItemActions(mrd, t);
            }

            log("Successfully registered {0} machines", machineById.Count);
            return mrd;
        }
        private MachineRegistration RegistrationForCubeValue(int cube) { return RegistrationForCubeValue(cube, -1); }

        private MachineRegistration RegistrationForCubeValue(int cube, int value)
        {
            MachineRegistration machineRegistration=null;
            if (machineById.TryGetValue(new MachineKey(cube, value), out machineRegistration))
            {
                return machineRegistration;
            }
            machineById.TryGetValue(new MachineKey(cube,-1),out machineRegistration);
            return machineRegistration;

        }
        public  ModCreateSegmentEntityResults CreateSegmentEntity(ModCreateSegmentEntityParameters parameters)
        {
/*            log("Creating with params: " +
                "X = " + parameters.X + ", " +
                "Y = " + parameters.Y + ", " +
                "Z = " + parameters.Z + ", " +
                "Cube = " + parameters.Cube + ", " +
                "Value = " + parameters.Value + ", " +
                "Segment = " + parameters.Segment + ", " +
                "Type = " + parameters.Type + ", " +
                "Flags = " + parameters.Flags + ", " +
                "toString = \"" + parameters.ToString() + "\""
                );
                */
            ModCreateSegmentEntityResults res = new ModCreateSegmentEntityResults();

            MachineRegistration machineRegistration= RegistrationForCubeValue(parameters.Cube,parameters.Value);
            if (machineRegistration != null)
            {
                parameters.ObjectType = machineRegistration.objectType;
                parameters.Type = eSegmentEntity.Mod;
                try
                {
                    res.Entity = (SegmentEntity)Activator.CreateInstance(machineRegistration.type, new Object[] { parameters });
                }
                catch(MissingMethodException e)
                {
                    log("No constructor for {1} that takes a ModCreateSegmentEntityParameters: {2}", machineRegistration.type.Name, e);
                }
            }
            else
            {
                log("Requested an unknown Segment Entity: " +
                    "X = " + parameters.X + ", " +
                    "Y = " + parameters.Y + ", " +
                    "Z = " + parameters.Z + ", " +
                    "Cube = " + parameters.Cube + ", " +
                    "Value = " + parameters.Value + ", " +
                    "Segment = " + parameters.Segment + ", " +
                    "Type = " + parameters.Type + ", " +
                    "Flags = " + parameters.Flags + ", " +
                    "toString = \"" + parameters.ToString() + "\""
                );
            }

            return res;
        }
        public ModItemActionResults PerformItemAction(ModItemActionParameters parameters)
        {
            Func<ModItemActionParameters, ModItemActionResults> p;
            if(itemActionByName.TryGetValue(ItemManager.GetItemName(parameters.ItemToUse), out p))
            {
                return p(parameters);
            }
            return null;
        }

        private bool IsPlacement(WorldFrustrum f, long x, long y, long z, ushort placementValue)
        {
            Segment segment = f.GetSegment(x,y,z);
            if (segment == null || !segment.mbInitialGenerationComplete || segment.mbDestroyed)
            {
                return false;
            }
            return segment.GetCube(x, y, z) == eCubeTypes.MachinePlacementBlock && segment.GetCubeData(x,y,z).mValue == placementValue;
        }

        public void CheckForCompletedMachine(ModCheckForCompletedMachineParameters parameters)
        {
            MachineRegistration machineRegistration = null;
            FCEMultiblockMachineAttribute specs = machineRegistration.MultiblockAttributes;

            if (!machineByPlacement.TryGetValue(parameters.CubeValue, out machineRegistration))
            {
                return;
            }

            // Find the negative-most corner
            long x = parameters.X;
            long y = parameters.Y;
            long z = parameters.Z;

            long minY = parameters.Y, minX = parameters.X, minZ = parameters.Z;

            while (IsPlacement(parameters.Frustrum, minX - 1, minY, minZ,machineRegistration.PlacementBlock))
            {
                minX--;
            }
            while (IsPlacement(parameters.Frustrum, minX , minY-1, minZ, machineRegistration.PlacementBlock))
            {
                minY--;
            }
            while (IsPlacement(parameters.Frustrum, minX, minY, minZ-1, machineRegistration.PlacementBlock))
            {
                minZ--;
            }


            long maxY = minY + specs.Length, maxX =0, maxZ =0;
            if(IsPlacement(parameters.Frustrum, minX+specs.Length, maxY, maxZ+specs.Width, machineRegistration.PlacementBlock))
            {
                maxX = minX + specs.Length;
                maxZ = minZ + specs.Width;
            } else if (IsPlacement(parameters.Frustrum, minX + specs.Width, maxY, maxZ + specs.Length, machineRegistration.PlacementBlock))
            {
                maxX = minX + specs.Width;
                maxZ = minZ + specs.Length;
            } else
            {
                // Couldn't find opposite corner, so just throw it out
                return;
            }



        }

    }



}

