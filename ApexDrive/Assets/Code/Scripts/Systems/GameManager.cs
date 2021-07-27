// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : GameSystem
{
    public static GameManager Instance;
    public const int MaxPlayers = 4;
    public const int Rounds = 3;

    [SerializeField] private Player[] m_Players;
    [SerializeField] private List<Player> m_ConnectedPlayers;
    public Player[] ConnectedPlayers { get { return m_ConnectedPlayers.ToArray(); } }
    public Player[] Players { get { return m_Players; } }
    public Color[] PlayerColors;
    public int PlayerCount { get { return m_ConnectedPlayers.Count(); } }

    public delegate void PlayerEvent(Player player);
    public static PlayerEvent OnPlayerConnected;
    public static PlayerEvent OnPlayerDisconnected;

    public void Awake()
    {
        if(Instance == null) Instance = this;
        m_Players = new Player[MaxPlayers];
        m_ConnectedPlayers = new List<Player>();
        for(int i = 0; i < MaxPlayers ; i++)
        {
            m_Players[i] = new Player(i, PlayerColors[i]);
        }
    }

    private void OnEnable()
    {
        Player.OnGameWin += ResetRoundScores;
    }

    private void OnDisable()
    {
        Player.OnGameWin = ResetRoundScores;
    }

    ///<returns>Returns the PlayerID of the newly created player. Returns null if controller is already in use.</returns>
    public Player AddPlayer(int controllerID, ControllerType controllerType)
    {
        if(m_Players.Where(x => x.ControllerID == controllerID).FirstOrDefault() != null) return null; // controller already in use
        Player player = m_Players.Where(x => !x.IsConnected).FirstOrDefault();
        player.AssignController(controllerID, controllerType);   
        m_ConnectedPlayers.Add(player);
        Debug.Log("Player " + player.PlayerReadableID + " joined the lobby using controller " + player.ControllerID + " (" + controllerType.ToString() + ")");
        if(OnPlayerConnected != null) OnPlayerConnected(player);
        return player;
    }

    public void RemovePlayerByID(int playerID)
    {
        Player player = m_Players[playerID];
        GameSystemsManager.Instance.StartCoroutine(RemovePlayerAtEndOfFrame(player));
    }

    ///<returns>Returns the PlayerID of the removed player. Returns null if no player was found.</returns>
    public Player RemovePlayerByControllerID(int controllerID)
    {
        Player player = m_Players.Where(x => x.ControllerID == controllerID).FirstOrDefault();
        GameSystemsManager.Instance.StartCoroutine(RemovePlayerAtEndOfFrame(player));
        return player;
    }

    public Player GetPlayerByController(int controllerID)
    {
        return m_Players.Where(x => x.ControllerID == controllerID).FirstOrDefault();
    }

    private IEnumerator RemovePlayerAtEndOfFrame(Player player)
    {
        yield return null;
        if(player != null)
        {
            player.ReleaseController();
            m_ConnectedPlayers.Remove(player);
            if(OnPlayerDisconnected != null) OnPlayerDisconnected(player);
        }
    }

    private void ResetRoundScores(Player winner)
    {
        foreach(Player player in m_Players) player.ResetRoundScore();
    }

    private void OnControllerConnected()
    {

    }

    private void OnControllerDisconnected()
    {
        
    }
}
