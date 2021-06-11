using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Image m_Preview;
    [SerializeField] private TMPro.TMP_Text m_Title;
    private LevelInfo m_Level;
    public LevelInfo Level { get { return m_Level; } }

    public void SetLevelInfo(LevelInfo info)
    {
        m_Level = info;
        m_Preview.sprite = info.Preview;
        m_Title.text = info.Name;
    }
}
