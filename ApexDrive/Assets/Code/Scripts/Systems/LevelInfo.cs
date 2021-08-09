using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu()]
public class LevelInfo : ScriptableObject
{
    public string Name;
    public Difficulty Difficulty;
    public Sprite Preview;
    [TextArea] public string Description;
    public string SceneName;
}
