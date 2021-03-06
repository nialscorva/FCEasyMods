﻿
using System;

namespace nialscorva.FCEEasyMods
{
    /// <summary>
    /// Marks the class as a SegmentEntity that should be registered when the mod loads.
    /// </summary>
    ///
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class FCESegmentEntityAttribute : System.Attribute
    {
        public string key;
        public SpawnableObjectEnum objectType;

        public FCESegmentEntityAttribute(string key,SpawnableObjectEnum objectType)
        {
            this.key = key;
            this.objectType = objectType;
        }
    }

    /// <summary>
    /// Marks a static property or field as the container for the cube type of this segment entity.
    /// </summary>
    /// <example>
    /// [CubeType] public static int CUBE_TYPE { get; protected set; }
    /// </example>
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class FCECubeType : System.Attribute
    {}

    /// <summary>
    /// Marks a static property or field as the container for the cube type of this segment entity.
    /// </summary>
    /// <example>
    /// [CubeValue] public static int CUBE_VALUE { get; protected set; }
    /// </example>
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class FCECubeValue : System.Attribute
    { }
}