
using System;

namespace nialsorva.FCEEasyMods
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class MachineActionButton : System.Attribute
    {
        public string ButtonName { get; }
        public int ModMask { get; }

        public MachineActionButton(string buttonName) : this(buttonName, 0) { }
        public MachineActionButton(string buttonName, int modMask)
        {
            this.ButtonName = buttonName;
            this.ModMask = modMask;
        }
    }
}