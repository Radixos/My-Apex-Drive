using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodMenuMusic : MonoBehaviour
{
    [SerializeField]
    [EventRef]
    private string menuMusicString = null;
    private FMOD.Studio.EventInstance musicMenu;



    private void OnEnable()
    {
        musicMenu = RuntimeManager.CreateInstance("event:/Music/Menu");
        musicMenu.start();
        musicMenu.setParameterByName("lobby", 0f);

        GameManager.OnPlayerConnected += OnPlayerConnected;
        GameManager.OnPlayerDisconnected += OnPlayerDisconnected;
    }

    private void OnPlayerConnected(Player player)
    {
        musicMenu.setParameterByName("lobby", 1f);
    }

    private void OnPlayerDisconnected(Player player)
    {
        if (GameManager.Instance.PlayerCount == 0)
        {
            musicMenu.setParameterByName("lobby", 0f);
        }
    }

    private void OnDisable()
    {
        musicMenu.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicMenu.release();
        GameManager.OnPlayerConnected -= OnPlayerConnected;
        GameManager.OnPlayerDisconnected -= OnPlayerDisconnected;
    }
}
