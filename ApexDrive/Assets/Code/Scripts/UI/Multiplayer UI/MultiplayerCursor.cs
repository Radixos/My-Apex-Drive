// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplayerCursor : MonoBehaviour
{
    [SerializeField] private Image m_Graphic;
    [SerializeField] private Image m_Mask;
    private RectTransform m_RectTransform;
    public bool IsLocked = false;
    public bool IsActive = false;
    private Color m_Color;
    [SerializeField] private Color m_LockedColor;
    [SerializeField] private float m_TransitionDuration = 0.1f;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public virtual void MoveTo(RectTransform target, float maskFill)
    {
        m_RectTransform.position = (Vector2)target.position;
        m_RectTransform.sizeDelta = target.sizeDelta;
        m_RectTransform.rotation = target.rotation;
        m_Mask.fillAmount = maskFill;
    }

    public void SetColor(Color color)
    {
        m_Color = color;
        if(m_Graphic != null) m_Graphic.color = color;
    }

    public void Lock()
    {
        if(m_Graphic != null) StartCoroutine(Co_LerpColor(m_Color, m_Color * m_LockedColor));
        IsLocked = true;
    }

    public void Unlock()
    {
        if(m_Graphic != null) StartCoroutine(Co_LerpColor(m_Color * m_LockedColor, m_Color));
        IsLocked = false;
    }

    private IEnumerator Co_LerpColor(Color from, Color to)
    {
        float elapsed = 0.0f;
        while(elapsed < m_TransitionDuration)
        {
            elapsed += Time.deltaTime;
            m_Graphic.color = Color.Lerp(from, to, elapsed / m_TransitionDuration);
            yield return null;
        }
    }
}
