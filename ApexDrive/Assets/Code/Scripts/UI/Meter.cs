using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    [SerializeField] private Image m_Bar;
    ///<summary>
    /// Update the meter using a value between 0 and 1 to show how full it should be
    /// Value is clamped between 0 and 1
    ///</summary>
    public void SetValue(float v)
    {
        v = Mathf.Clamp01(v);
        if(m_Bar != null)
        {
            m_Bar.rectTransform.anchoredPosition = new Vector2((v-1.0f)*m_Bar.rectTransform.sizeDelta.x, m_Bar.rectTransform.anchoredPosition.y);
        }
    }
}
