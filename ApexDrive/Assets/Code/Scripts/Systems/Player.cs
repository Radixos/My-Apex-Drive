// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    [SerializeField] private int m_PlayerID;
    [SerializeField] private int m_ControllerID = -1;
    private bool m_IsConnected;
    private Color m_PlayerColor;
    public int RoundWins { get; private set; }
    public int GameWins { get; private set; }
    public string Name { get; private set; }

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

    public void AssignController(int controllerID)
    {
        m_ControllerID = controllerID;
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
        if(OnGameScoreChange != null) OnGameScoreChange(this);
    }

    public void WinGame()
    {
        GameWins ++;
        if(OnGameWin != null) OnGameWin(this);
        if(OnGameScoreChange != null) OnGameScoreChange(this);
    }

    public void ResetScore()
    {
        RoundWins = 0;
        GameWins = 0;
        if(OnGameScoreChange != null) OnGameScoreChange(this);
    }
}
