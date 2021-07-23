using UnityEngine;
using UnityEditor;
 
public static class RemoveCollider
{
    [MenuItem("Tools/Remove Colliders In Scene")]
    static public void RemoveColliders()
    {
        Collider[] colliders = GameObject.FindObjectsOfType<Collider>();

        foreach(Collider collider in colliders)
        {
            GameObject.DestroyImmediate(collider);
        }
    }
}