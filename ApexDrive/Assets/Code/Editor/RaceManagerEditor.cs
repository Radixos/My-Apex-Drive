using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RaceManager))]
public class RaceManagerEditor : Editor
{

    private RaceManager m_RaceManager;

    private void OnEnable()
    {
        m_RaceManager = (RaceManager) target;
    }
    private void OnSceneGUI()
    {
        if(Application.isPlaying)
        {
            Handles.BeginGUI();
            if(GUILayout.Button("Spawn Test Car"))
            {
                if(GameManager.Instance == null || GameManager.Instance.PlayerCount >= 4) return;
		        Player player  = GameManager.Instance.AddPlayer(GameManager.Instance.PlayerCount + 1);
                m_RaceManager.SpawnPlayer(player, true);
            }
            Handles.EndGUI();
        }
    }
}
