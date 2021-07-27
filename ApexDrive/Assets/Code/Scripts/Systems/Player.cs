// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public enum PlayerState {Menu, Racing, Eliminated}
    public PlayerState State = PlayerState.Menu;


    [SerializeField] private int m_PlayerID;
    [SerializeField] private int m_ControllerID = -1;
    [SerializeField] private ControllerType m_ControllerType = ControllerType.None;
    private bool m_IsConnected;
    private Color m_PlayerColor;
    public int RoundWins { get; private set; }
    public int GameWins { get; private set; }
    public string Name { get; private set; }
    public ControllerType ControllerType { get { return m_ControllerType; } }

    public CoreCarModule Car;

    public int PlayerID { get { return m_PlayerID; } }
    public int PlayerReadableID { get { return m_PlayerID + 1; } }
    public int ControllerID { get { return m_ControllerID; } }
    public bool IsConnected { get { return m_IsConnected; } }
    public Color PlayerColor { get { return m_PlayerColor; } }

    public delegate void PlayerEvent(Player player);
    public static PlayerEvent OnRoundWin;
    public static PlayerEvent OnGameWin;
    public static PlayerEvent OnGameScoreChange;


    // Race Time Variables
    public float TrackProgress = 0.0f;
    public int Position = 0;
    public int Laps = 0;


    public Player(int playerID, Color playerColor)
    {
        m_PlayerID = playerID;
        m_PlayerColor = playerColor;
    }

    public void AssignController(int controllerID, ControllerType controllerType)
    {
        m_ControllerID = controllerID;
        m_ControllerType = controllerType;
        m_IsConnected = true;
    }

    public void ReleaseController()
    {
        m_ControllerID = -1;
        m_IsConnected = false;
    }

    public void WinRound()
    {
        RoundWins ++;
        if(OnRoundWin != null) OnRoundWin(this);
        if(RoundWins >= GameManager.Rounds && OnGameWin != null) OnGameWin(this);
    }

    public void WinGame()
    {
        GameWins ++;
        if(OnGameWin != null) OnGameWin(this);
    }

    public void ResetScore()
    {
        RoundWins = 0;
        GameWins = 0;
    }

    public void ResetRoundScore()
    {
        RoundWins = 0;
    }
}
