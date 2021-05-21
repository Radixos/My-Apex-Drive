// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(50)]
[RequireComponent(typeof(Animator))]
public class LobbyMenu : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private Animator[] m_PlayerPortraits;
    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private MultiplayerButton m_StartGameButton;

    [SerializeField] private bool[] m_PlayersReady;

    private bool m_MenuIsVisible = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerConnected += OnPlayerConnected;
        GameManager.OnPlayerDisconnected += OnPlayerDisconnected;

        m_PlayersReady = new bool[GameManager.MaxPlayers];
        for(int i = 0; i < m_PlayersReady.Length; i++)
        {
            m_PlayersReady[i] = false;
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerConnected -= OnPlayerConnected;
        GameManager.OnPlayerDisconnected -= OnPlayerDisconnected;
    }

    public void Update()
    {
        for (int i = 1; i <= GameManager.MaxPlayers; i++)
        {
            if(GameManager.Instance.GetPlayerByController(i) == null)
            {
                if(Input.GetButtonDown("Submit " + (i)))
                {
                    Player player = GameManager.Instance.AddPlayer(i);
                }
            }
        }
    }

    public void DisconnectPlayer(MultiplayerEventData data)
    {
        m_PlayersReady[data.Player.PlayerID] = false;
        GameManager.Instance.RemovePlayerByID(data.Player.PlayerID);
    }

    public void Ready(MultiplayerEventData data)
    {
        m_PlayersReady[data.Player.PlayerID] = true;
        int x = 0;
        for(int i = 0; i < GameManager.MaxPlayers; i++)
        {
            if(m_PlayersReady[i]) x++;
        }
        if(x >= 2 && x >= GameManager.Instance.PlayerCount) 
        {
            m_Animator.SetBool("StartGame", true);
            Debug.Log("Load game scene in 3.5 seconds");
        }
    }

    public void CancelReady(MultiplayerEventData data)
    {
        m_PlayersReady[data.Player.PlayerID] = false;
        m_Animator.SetBool("StartGame", false);
    }

    private void OnPlayerConnected(Player player)
    {
        if(!m_MenuIsVisible)
        {
            if(m_Animator != null) m_Animator.SetBool("MenuOpen", true);
            m_MenuIsVisible = true;
        }
        if(GameManager.Instance.PlayerCount < 2) m_StartGameButton.interactable = false;
        else m_StartGameButton.interactable = true;
        if(player != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", true);
    }

    private void OnPlayerDisconnected(Player player)
    {
        if(GameManager.Instance.PlayerCount == 0) 
        {
            if(m_Animator != null) m_Animator.SetBool("MenuOpen", false);
            m_MenuIsVisible = false;
        }
        m_PlayersReady[player.PlayerID] = false;
        if(GameManager.Instance.PlayerCount < 2) m_StartGameButton.interactable = false;
        else m_StartGameButton.interactable = true;
        if(player != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", false);
    }
}
