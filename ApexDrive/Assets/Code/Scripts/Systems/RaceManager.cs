using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public delegate void RaceEvent();
    public static RaceEvent OnRaceSceneLoaded;
    public static RaceEvent OnGameStart;
    public static RaceEvent OnGameEnd;
    public static RaceEvent PreRoundStart;
    public static RaceEvent CountdownStart;
    public static RaceEvent CountdownEnd;
    public static RaceEvent OnRoundStart;
    public static RaceEvent OnRoundEnd;

    private void Start()
    {
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

    public void SpawnPlayers(Player[] players)
    {
        for(int i = 0; i < players.Length; i++)
        {
            OrientedPoint op = ActiveTrack.Evaluate(m_TrackProgress);
            Vector3 offset = Vector3.left * (players.Length - i) * 1.5f + Vector3.right * players.Length / 2.0f * 1.5f;
            Vector3 spawnPoint = op.pos + op.rot * offset + Vector3.up * 1.05f;

            CoreCarModule car = m_CarPrefabs[players[i].PlayerID];
            car.gameObject.SetActive(true);
            car.transform.position = spawnPoint;
            car.transform.rotation = op.rot;
            car.Player.Laps = 0;

            if(players[i].ControllerID <= 0 || players[i].ControllerID >= 4) players[i].AssignController(i+1); // this shouldn't be the case except for debugging in the race scene
            car.SetPlayer(players[i]);
            players[i].Car = car;
        }
    }

    private void EndRound(Player winner)
    {
        State = RaceState.PostRace;
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = false;
        m_TrackProgress = winner.TrackProgress;
        if(OnRoundEnd != null) OnRoundEnd();

        if(winner.RoundWins >= GameManager.Rounds)
        {
            // end the game
            StartCoroutine(Co_EndGame());
        }
        else
        {
            StartCoroutine(Co_StartRound(3.0f));
        }
    }

    private IEnumerator Co_StartGame()
    {
        yield return new WaitForSeconds(0.5f); // wait whilst fading in
        if(OnGameStart != null) OnGameStart();
        StartCoroutine(Co_StartRound(0.0f));
    }

    private IEnumerator Co_StartRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPlayers(GameManager.Instance.ConnectedPlayers);
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = false;
        if(PreRoundStart != null) PreRoundStart();
        yield return StartCoroutine(Co_Countdown());

        State = RaceState.Racing;
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = true;
        if(OnRoundStart != null) OnRoundStart();
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
            if(y.Laps.CompareTo(x.Laps) != 0) return y.Laps.CompareTo(x.Laps);
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

    private IEnumerator Co_Countdown()
    {
        if(CountdownStart != null) CountdownStart();
        yield return new WaitForSeconds(3.25f);
        if(CountdownEnd != null) CountdownEnd();
    }

    private IEnumerator Co_EndGame()
    {
        yield return new WaitForSeconds(4.0f);
        if(OnGameEnd != null) OnGameEnd();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Scene_Menu");   
    }
}