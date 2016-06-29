
using System;

namespace nialsorva.FCEEasyMods
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FCEItemAction : System.Attribute
    {
        public string[] itemNames { get; }

        public FCEItemAction(params string[] itemNames)
        {
            this.itemNames = itemNames;
        }

    }
}