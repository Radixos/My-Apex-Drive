using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : Singleton<RaceManager>
{
    public enum RaceState {PreRace, Racing, PostRace}
    public static RaceState State;

    public RoadChain ActiveTrack;
    [SerializeField] private CoreCarModule[] m_CarPrefabs;
    public float m_TrackProgress = 0.01f;
    public Player FirstPlayer
    {
        get 
        {
            for(int i = 0; i < GameManager.Instance.PlayerCount; i++) if(GameManager.Instance.ConnectedPlayers[i].Position == 1) return GameManager.Instance.ConnectedPlayers[i];
            return null;
        }
    }

    public List<PositionUpdate> raceCars;
    public List<PositionUpdate> ogRaceCars; // Non-updated list
    public int totalColliders;

    public delegate void RaceEvent();
    public static RaceEvent OnRaceSceneLoaded;
    public static RaceEvent OnGameStart;
    public static RaceEvent OnGameEnd;
    public static RaceEvent PreRoundStart;
    public static RaceEvent OnRoundStart;
    public static RaceEvent OnRoundEnd;

    private void Start()
    {
        Initialise();
        if(OnRaceSceneLoaded != null) OnRaceSceneLoaded();
        StartCoroutine(Co_StartGame());
    }

    private void OnEnable()
    {
        Player.OnRoundWin += EndRound;
    }

    private void OnDisable()
    {
        Player.OnRoundWin -= EndRound;
    }

    private void Update()
    {
        if(State == RaceState.Racing) UpdatePlayerRaceInfo();
    }

    private void Initialise()
    {
        SpawnPlayers(GameManager.Instance.ConnectedPlayers);
    }

    public void SpawnPlayers(Player[] players)
    {
        for(int i = 0; i < players.Length; i++)
        {
            OrientedPoint op = ActiveTrack.Evaluate(m_TrackProgress);
            Vector3 offset = Vector3.left * (players.Length - i) * 1.5f + Vector3.right * players.Length / 2.0f * 1.5f;
            Vector3 spawnPoint = op.pos + op.rot * offset + Vector3.up * 1.05f;
            CoreCarModule car = Instantiate(m_CarPrefabs[players[i].PlayerID], spawnPoint, op.rot);
            car.Stats.CanDrive = false;
            if(players[i].ControllerID <= 0 || players[i].ControllerID >= 4) players[i].AssignController(i+1); // this shouldn't be the case except for debugging in the race scene
            car.SetPlayer(players[i]);
            players[i].Car = car;
        }
    }

    private void EndRound(Player winner)
    {
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = false;
        if(OnRoundEnd != null) OnRoundEnd();
        // get progress for next spawn
    }

    private IEnumerator Co_StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Co_StartRound());
    }

    private IEnumerator Co_StartRound()
    {
        if(PreRoundStart != null) PreRoundStart();
        yield return new WaitForSeconds(3.25f);
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = true;
        State = RaceState.Racing;
    }

    public void UpdatePlayerRaceInfo()
    {
        foreach(Player player in GameManager.Instance.ConnectedPlayers)
        {
            player.TrackProgress = ActiveTrack.GetNearestTimeOnSpline(player.Car.Position, 10, 5);
        }

        Player[] players = GameManager.Instance.ConnectedPlayers;
        Array.Sort(players, (x,y) => 
        {
            if(x.Laps.CompareTo(y.Laps) != 0) return x.Laps.CompareTo(y.Laps);
            return y.TrackProgress.CompareTo(x.TrackProgress);
        });

        for(int i = 0; i < players.Length; i++)
        {
            players[i].Position = i + 1;
        }
    }

    [ContextMenu("Spawn Test Car")]
	private void SpawnTestCar()
	{
        if(GameManager.Instance.PlayerCount >= 4) return;
		Player player  = GameManager.Instance.AddPlayer(GameManager.Instance.PlayerCount + 1);
	}

    public void SpawnPlayer(Player player, bool canDrive = false)
    {
        OrientedPoint op = ActiveTrack.Evaluate(m_TrackProgress);
        Vector3 spawnPoint = op.pos + Vector3.up * 1.05f;
        CoreCarModule car = Instantiate(m_CarPrefabs[player.PlayerID], spawnPoint, op.rot);
        car.Stats.CanDrive = canDrive;
        if(player.ControllerID < 0 || player.ControllerID >= 4) player.AssignController(1); // this shouldn't be the case except for debugging in the race scene
        car.SetPlayer(player);
        player.Car = car;
    }
}