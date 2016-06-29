
using System;

namespace nialsorva.FCEEasyMods
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FCEInputPort : System.Attribute
    {
        public Type EntityType { get; }
        public Neighborhood Neighborhood{ get; }
        public FCEInputPort(Type entityType,Neighborhood neighborhood)
        {
            this.EntityType = entityType;
            this.Neighborhood = neighborhood;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FCEVisualState : System.Attribute
    {
        public string Name {get;}
        public FCEVisualState(string name)
        {
            this.Name = name;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class FCEItemAction : System.Attribute
    {
        public string[] itemNames { get; }
        public ItemType? itemType { get; }

        public FCEItemAction(params string[] itemNames)
        {
            this.itemNames = itemNames;
            this.itemType = null;
        }
        public FCEItemAction(ItemType itemType)
        {
            this.itemNames = null;
            this.itemType = itemType;
        }
    }
}