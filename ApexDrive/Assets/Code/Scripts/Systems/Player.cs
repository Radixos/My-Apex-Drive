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
    public CarController CharacterController;
    public int RoundWins = 0;
    public int GameWins = 0;

    public int PlayerID { get { return m_PlayerID; } }
    public int PlayerReadableID { get { return m_PlayerID + 1; } }
    public int ControllerID { get { return m_ControllerID; } }
    public bool IsConnected { get { return m_IsConnected; } }
    public Color PlayerColor { get { return m_PlayerColor; } }


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
}
