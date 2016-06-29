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
        private const string LOGGER_PREFIX = "[nialscorva.smartConveyors] ";
        private const bool DEBUG = true;
        public static void log(String msg, params object[] args)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log(LOGGER_PREFIX + ": " + String.Format(msg, args));
            }
        }
        Dictionary<MachineKey, System.Type> machineById = new Dictionary<MachineKey, Type>();

        public void registerMachine(ModRegistrationData mrd, String id, Type type)
        {
            mrd.RegisterEntityHandler(id);

            TerrainDataEntry terrainDataEntry;
            TerrainDataValueEntry terrainDataValueEntry;
            TerrainData.GetCubeByKey(id, out terrainDataEntry, out terrainDataValueEntry);
            int? cubeType = terrainDataEntry?.CubeType;
            int? cubeValue = terrainDataValueEntry?.Value;


            MemberInfo[] props = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            Action<MemberInfo, Object> setValue = (MemberInfo p, Object v) =>
            {
                if (p.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)p).SetValue(null, v, null);
                }
                else if (p.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)p).SetValue(null, v);
                }
            };
            foreach (MemberInfo p in props)
            {
                if (p.GetCustomAttributes(typeof(FCECubeType), false).Length > 0)
                {
                    setValue(p, cubeType);
                }
                if (p.GetCustomAttributes(typeof(FCECubeValue), false).Length > 0)
                {
                    setValue(p, cubeValue);
                }
            }

            machineById.Add(new MachineKey(cubeType, cubeValue), type);

            log("Registered {0} to {1} with key={2}, value={3}", id, type.Name, cubeType, cubeValue);

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
                object[] attrs = t.GetCustomAttributes(typeof(FCESegmentEntity), true);
                if (attrs.Length > 0)
                {
                    FCESegmentEntity seg = (FCESegmentEntity)attrs[0];
                    registerMachine(mrd, seg.key, t);
                }
            }

            log("Successfully registered {0} machines", machineById.Count);
            return mrd;
        }

/*        public void RegisterWindowHandler()
        {
            GenericMachineManager machineManager = (GenericMachineManager)typeof(GenericMachinePanelScript).GetField("manager", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GenericMachinePanelScript.instance);

            if (machineManager != null)
            {
                machineManager.AddWindowType(eSegmentEntity.DoodadFactory, new ModWindow());
            }
        }
        */
        public  ModCreateSegmentEntityResults CreateSegmentEntity(ModCreateSegmentEntityParameters parameters)
        {
            log("Creating with params: " +
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
            ModCreateSegmentEntityResults res = new ModCreateSegmentEntityResults();

            Type type = machineById[new MachineKey(parameters.Cube, parameters.Value)];
            if (type != null)
            {
                res.Entity = (SegmentEntity)Activator.CreateInstance(type, new Object[] { parameters });
            }

            return res;
        }
    }
}

