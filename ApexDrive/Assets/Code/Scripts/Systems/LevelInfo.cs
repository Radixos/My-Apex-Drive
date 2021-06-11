using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu()]
public class LevelInfo : ScriptableObject
{
    public string Name;
    public string ReferenceString;
    [TextArea] public string Description;
    public Sprite Preview;
}
