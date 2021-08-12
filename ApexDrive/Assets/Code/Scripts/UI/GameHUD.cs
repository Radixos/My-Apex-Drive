using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Animator m_TimerAnimator;
    [SerializeField] private Animator m_FaderAnimator;
    [SerializeField] private Animator m_WinnerBanner;
    [SerializeField] private Image m_BannerBorder;
    [SerializeField] private TMPro.TMP_Text m_WinnerText;
    [SerializeField] private Image[] m_Stars;
    [SerializeField] private GameObject[] m_Avatars; // stupid way to handle this but it will do for now
    [SerializeField] private Animator m_AvatarAnimator;

    // [SerializeField] private CanvasGroup[] m_PortraitGroups;
    // [SerializeField] private Image[] m_PowerMeters;

    private void OnEnable()
    {
        RaceManager.OnRaceSceneLoaded += FadeInScene;
        RaceManager.CountdownStart += Countdown;
        Player.OnRoundWin += RoundTransition;
        RaceManager.OnGameEnd += FadeOutScene;
    }

    private void OnDisable()
    {
        RaceManager.OnRaceSceneLoaded -= FadeInScene;
        RaceManager.CountdownStart -= Countdown;
        Player.OnRoundWin -= RoundTransition;
        RaceManager.OnGameEnd -= FadeOutScene;
    }

    private void FadeInScene()
    {
        m_FaderAnimator.SetBool("Visible", false);
    }

    private void FadeOutScene()
    {
        m_FaderAnimator.SetBool("Visible", true);
    }

    private void Countdown()
    {
        m_TimerAnimator.SetTrigger("Countdown");
    }

    private void RoundTransition(Player winner)
    {

        foreach(GameObject avatar in m_Avatars) avatar.SetActive(false);
        m_Avatars[winner.PlayerID].SetActive(true);
        m_AvatarAnimator.SetTrigger("Celebrate");
        m_AvatarAnimator.SetInteger("Celebration", Random.Range(0,2));
        m_BannerBorder.color = winner.PlayerColor;
        m_WinnerText.text = "Winner - Player " + winner.PlayerReadableID;
        for(int i = 0; i < m_Stars.Length; i++)
        { 
            m_Stars[i].color = winner.RoundWins > i ? Color.white : new Color(1.0f, 1.0f, 1.0f, 0.05f);
        }
        m_WinnerBanner.SetTrigger("Slide");
    }
}
