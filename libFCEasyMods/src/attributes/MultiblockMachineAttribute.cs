
using System;

namespace nialscorva.FCEEasyMods
{
    /// <summary>
    /// Marks the class as a SegmentEntity that should be registered when the mod loads.
    /// </summary>
    ///
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class FCEMultiblockMachineAttribute : System.Attribute
    {
        public int Length;
        public int Width;
        public int Height;
        public String PlacementBlock;

        public FCEMultiblockMachineAttribute(string placementBlock,int length,int width, int height)
        {
            Length = length;
            Width = width;
            Height = height;
            PlacementBlock = placementBlock;
        }

    }
}