// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplayerCursor : MonoBehaviour
{
    private Image m_Image;
    private RectTransform m_RectTransform;
    public bool IsLocked = false;
    public bool IsActive = false;
    private Color m_Color;
    [SerializeField] private Color m_LockedColor;
    [SerializeField] private float m_TransitionDuration = 0.1f;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void MoveTo(Vector2 position)
    {
        m_RectTransform.position = position;
    }

    public void SetColor(Color color)
    {
        m_Color = color;
        if(m_Image != null) m_Image.color = color;
    }

    public void Lock()
    {
        if(m_Image != null) StartCoroutine(Co_LerpColor(m_Color, m_Color * m_LockedColor));

    }

    public void Unlock()
    {
        if(m_Image != null) StartCoroutine(Co_LerpColor(m_Color * m_LockedColor, m_Color));
    }

    private IEnumerator Co_LerpColor(Color from, Color to)
    {
        float elapsed = 0.0f;
        while(elapsed < m_TransitionDuration)
        {
            elapsed += Time.deltaTime;
            m_Image.color = Color.Lerp(from, to, elapsed / m_TransitionDuration);
            yield return null;
        }
    }
}
