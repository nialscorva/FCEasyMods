using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialsorva.FCEEasyMods
{
    /// <summary>
    /// Marks the class as a SegmentEntity that should be registered when the mod loads.
    /// </summary>
    ///
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class FCETerrainData : System.Attribute
    {
        public string Name;
        public string IconName;
        public string Description; 
        public string LayerType  = "PrimaryTerrain";
        public int TopTexture  = 192;
        public int SideTexture  = 192;
        public int BottomTexture  = 192;
        public int GUITexture  = 192;
        public bool isSolid  = false;
        public bool isTransparent  = false;
        public bool isHollow  = false;
        public bool isGlass  = false;
        public bool isPassable  = false;
        public bool hasObject  = true;
        public bool hasEntity  = true;
        public string AudioWalkType  = "Metal";
        public string AudioBuildType  = "Metal";
        public string AudioDestroyType  = "None";
        public string[] Category = new string[] { "Machine" };
        public int MaxStack  = 100;
    }
    public class FCETerrainDataValue : System.Attribute
    {
        public string Name;
        public string Parent;
        public string IconName;
        public string Description; 
    }
}
