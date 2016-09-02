
using System;

namespace nialscorva.FCEEasyMods
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FCEItemActionAttribute : System.Attribute
    {
        public string[] itemNames { get; }

        public FCEItemActionAttribute(params string[] itemNames)
        {
            this.itemNames = itemNames;
        }

    }
}