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

    private void FadeInScene()
    {
        m_FaderAnimator.SetTrigger("FadeIn");
    }

    private void FadeOutScene()
    {
        m_FaderAnimator.SetTrigger("FadeOut");
    }

    private void Countdown()
    {
        m_TimerAnimator.SetTrigger("Countdown");
    }
}
