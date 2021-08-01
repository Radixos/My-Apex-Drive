using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MultiplayerButton))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private LevelInfo m_Level;
    [SerializeField] private Image m_Preview;
    [SerializeField] private TMPro.TMP_Text m_Title;
    private MultiplayerButton m_MultiplayerButton;
    public LevelInfo Level { get { return m_Level; } }

    private void Awake()
    {
        m_MultiplayerButton = GetComponent<MultiplayerButton>();
        SetLevelInfo(m_Level);
    }

    public void SetLevelInfo(LevelInfo info)
    {
        m_Level = info;
        m_Preview.sprite = info.Preview;
        m_Title.text = info.Name;
    }
}
