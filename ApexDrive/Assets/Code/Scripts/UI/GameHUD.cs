using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Animator m_TimerAnimator;
    [SerializeField] private Animator m_FaderAnimator;

    private void OnEnable()
    {
        RaceManager.OnRaceSceneLoaded += FadeInScene;
        RaceManager.PreRoundStart += Countdown;
    }

    private void OnDisable()
    {
        RaceManager.OnRaceSceneLoaded -= FadeInScene;
        RaceManager.PreRoundStart -= Countdown;
    }

    private void FadeInScene(Player[] players)
    {
        m_FaderAnimator.SetTrigger("FadeIn");
    }

    private void FadeOutScene(Player[] players)
    {
        m_FaderAnimator.SetTrigger("FadeOut");
    }

    private void Countdown(Player[] players)
    {
        m_TimerAnimator.SetTrigger("Countdown");
    }
}
