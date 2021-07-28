using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodLevelMusic : MonoBehaviour
{
    [SerializeField]
    [EventRef]
    private string musicPath = null;
    private EventInstance levelMusic;

    private void OnEnable()
    {
        levelMusic = RuntimeManager.CreateInstance(musicPath);
        levelMusic.start();

        RaceManager.PreRoundStart += RoundStart;
        RaceManager.OnRoundEnd += RoundEnd;
        RaceManager.OnGameEnd += MatchEnd;
        EliminationScript.OnPlayerEliminated += OnPlayerEliminated;

    }

    private void OnPlayerEliminated(Player[] activePlayers)
    {
        //sets to final two players music
        levelMusic.setParameterByName("PlayersRemaining", activePlayers.Length);
    }

    private void RoundEnd()
    {
        levelMusic.setParameterByName("RoundEnd", 1);
    }

    private void RoundStart()
    {
        levelMusic.setParameterByName("RoundEnd", 0);
    }

    private void MatchEnd()
    {
        levelMusic.setParameterByName("matchend", 1);
    }

    private void OnDisable()
    {
        levelMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        levelMusic.release();
    }
}
