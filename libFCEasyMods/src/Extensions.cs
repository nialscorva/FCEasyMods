using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static T GetComponent<T>(this GameObject target, string name) where T : Component, new()
    {
        foreach (T t in target.GetComponentsInChildren<T>() ?? new T[] { })
        {
            if (string.CompareOrdinal(name, t.name) == 0)
            {
                return t;
            }
        }
        return null;
    }

    public static GameObject GetDescendantGameObject(this GameObject target, string name)
    {
        return target?.transform?.Search(name)?.gameObject;
    }

    public static CubeCoord ToNormalizedCoordinates(this SegmentEntity target)
    {
        return new CubeCoord(target.mnX - 4611686017890516992L, target.mnY - 4611686017890516992L, target.mnZ - 4611686017890516992L);
    }
}