using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private TMPro.TMP_Text m_Text;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void UpdateCombo(string comboName, float points)
    {
        m_Text.text = comboName + " +"+Mathf.RoundToInt(points * 100.0f);
    }

    public void Disappear()
    {
        m_Animator.SetTrigger("Disappear");
    }
}
