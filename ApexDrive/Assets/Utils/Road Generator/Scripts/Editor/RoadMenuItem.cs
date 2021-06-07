using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoadMenuItem
{
    [MenuItem("GameObject/Apex Drive/Road")]
    public static void InstantiateRoadPrefab()
    {
        PrefabUtility.InstantiatePrefab(Resources.Load<Object>("Road"));
    }
}
