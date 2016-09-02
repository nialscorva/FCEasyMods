using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FCEDoors
{
    class GameObjectDumper
    {

        System.IO.StreamWriter file;
        public HashSet<Component> seen = new HashSet<Component>();
        
        public IEnumerator<SpawnableObjectEnum> spawnableIter=((IEnumerable<SpawnableObjectEnum>)Enum.GetValues(typeof(SpawnableObjectEnum))).GetEnumerator();

        public GameObjectDumper()
        {
            file = new System.IO.StreamWriter(@"GameObjectTree.txt");
        }

        public string DumpNext()
        {
            if(spawnableIter.MoveNext())
            {
                Dump(spawnableIter.Current);
                return spawnableIter.Current.ToString();
            }
            file.Close();
            file.Dispose();
            return null;
        }

        protected void Dump(SpawnableObjectEnum spawnable)
        {
            if((int)spawnable >= SpawnableObjectManagerScript.instance.maSpawnableObjects.Length)
            {
                return;
            }
            GameObject go = SpawnableObjectManagerScript.instance.maSpawnableObjects[(int)spawnable];
            if(go!=null)
            {
                file.WriteLine($"== {spawnable} ==");
                DumpGameObject(go, "    ");
                file.WriteLine("\n");
            }
        }
        
        private void DumpGameObject(GameObject go,string prefix)
        {
            //List<Component> components = new List<Component>();
            //go.GetComponents<Component>(components);
            //foreach (Component c in components)
            //{
            //    DumpComponent(c, "    ");
            //}
            file.WriteLine("{| class=\"article-table sortable\"\n"
                + "!Name\n!Type\n!OtherInfo"
                );
            foreach(Component c in go.GetComponentsInChildren<Component>())
            {
                file.Write("|-\n"
                    +$"| {c.name}\n"
                    +$"| {c.GetType().Name}\n"
                    +"|");
                if (c is Animation)
                {
                    Animation a = (Animation)c;
                    file.Write("States = ");
                    foreach (AnimationState state in a)
                    {
                        file.Write(state.name + ",");
                    }
                }
                file.WriteLine();
            }
            file.WriteLine("|}");
        }
    }
}
