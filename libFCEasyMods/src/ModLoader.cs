using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialsorva.FCEEasyMods
{
    using System.Linq.Expressions;
    using System.Reflection;
    using MachineKey = System.Tuple<int?, int?>;

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
        Dictionary<MachineKey, System.Type> machineById = new Dictionary<MachineKey, Type>();
        Dictionary<string, Func<ModItemActionParameters, ModItemActionResults>> itemActionByName = new Dictionary<string, Func<ModItemActionParameters, ModItemActionResults>>();

        public void registerMachine(ModRegistrationData mrd, Type type)
        {
            object[] segEntityAttr = type.GetCustomAttributes(typeof(FCESegmentEntity), false);
            if (segEntityAttr.Length == 0)
            {
                return;
            }
            FCESegmentEntity segEntity = segEntityAttr[0] as FCESegmentEntity;
            
            // TODO: confirm that type it has a proper constructor
            mrd.RegisterEntityHandler(segEntity.key);

            // find the cubeType and CubeValue that was assigned
            TerrainDataEntry terrainDataEntry;
            TerrainDataValueEntry terrainDataValueEntry;
            TerrainData.GetCubeByKey(segEntity.key, out terrainDataEntry, out terrainDataValueEntry);
            int? cubeType = terrainDataEntry?.CubeType;
            int? cubeValue = terrainDataValueEntry?.Value;

            // add it to our list so that we can create it later
            machineById.Add(new MachineKey(cubeType, cubeValue), type);

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
            
            log("Registered {0} to {1} with key={2}, value={3}", id, type.Name, cubeType, cubeValue);
        }

       
        public void registerItemActions(ModRegistrationData mrd, Type type)
        {
            MethodInfo[] props = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo p in props)
            {
                object[] itemActions = type.GetCustomAttributes(typeof(FCEItemAction), false);
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
                    FCEItemAction itemAction = itemActions[0] as FCEItemAction;
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

        public void RegisterWindowHandler()
        {
            GenericMachineManager machineManager = (GenericMachineManager)typeof(GenericMachinePanelScript).GetField("manager", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GenericMachinePanelScript.instance);

            if (machineManager != null)
            {
                machineManager.AddWindowType(eSegmentEntity.DoodadFactory, new ModWindow());
            }
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

            Type type = machineById[new MachineKey(parameters.Cube, parameters.Value)];
            if (type != null)
            {
                res.Entity = (SegmentEntity)Activator.CreateInstance(type, new Object[] { parameters });
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

    }



}

