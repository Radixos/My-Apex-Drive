using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class RaceManager : Singleton<RaceManager>
{
    public enum RaceState {PreRace, Racing, PostRace, None}
    public static RaceState State = RaceState.None;

    public RoadChain ActiveTrack;
    [SerializeField] private CoreCarModule[] m_CarPrefabs;
    [SerializeField]private float m_TrackProgress = 0.01f;
    public float TrackProgress { get{ return m_TrackProgress; } }
    public Player FirstPlayer
    {
        get 
        {
            for(int i = 0; i < GameManager.Instance.PlayerCount; i++) if(GameManager.Instance.ConnectedPlayers[i].Position == 1) return GameManager.Instance.ConnectedPlayers[i];
            return null;
        }
    }

    private float grace = 0.0f;

    private Coroutine[] m_GraceRoutines = new Coroutine[4];
    private List<Player> m_ActivePlayers = new List<Player>();
    private List<Player> m_OffscreenPlayers = new List<Player>();

    public delegate void EliminationEvent(Player[] activePlayers);
    public static EliminationEvent OnPlayerEliminated;

    public delegate void RaceEvent();
    public static RaceEvent OnRaceSceneLoaded;
    public static RaceEvent OnGameStart;
    public static RaceEvent OnGameEnd;
    public static RaceEvent PreRoundStart;
    public static RaceEvent CountdownStart;
    public static RaceEvent CountdownEnd;
    public static RaceEvent OnRoundStart;
    public static RaceEvent OnRoundEnd;

    [SerializeField] private ParticleSystem[] m_PlayerSpawnVFX = new ParticleSystem[4];


    protected override void Awake()
    {
        base.Awake();
        if(GameManager.Instance.PlayerCount > 0) State = RaceState.PreRace;
    }

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

    private IEnumerator Co_SpawnPlayers(Player[] players)
    {
        OrientedPoint op = ActiveTrack.Evaluate(m_TrackProgress);
        for(int i = 0; i < players.Length; i++)
        {
            Vector3 offset = Vector3.left * (players.Length - i) * 1.5f + Vector3.right * players.Length / 2.0f * 1.5f;
            Vector3 spawnPoint = op.pos + op.rot * offset + Vector3.up * 1.05f;

            Destroy(GameObject.Instantiate(m_PlayerSpawnVFX[i], spawnPoint, Quaternion.identity), 5.0f);
            yield return new WaitForSeconds(1.0f);

            CoreCarModule car = m_CarPrefabs[players[i].PlayerID];
            car.gameObject.SetActive(true);
            car.transform.position = spawnPoint;
            car.transform.rotation = op.rot;

            car.SetPlayer(players[i]);
            players[i].Car = car;
        }
        yield return new WaitForSeconds(1.0f);
    }

    private void EndRound(Player winner)
    {
        StartCoroutine(Co_EndRound(winner));
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
        yield return StartCoroutine(Co_SpawnPlayers(GameManager.Instance.ConnectedPlayers));
        foreach(Player player in GameManager.Instance.ConnectedPlayers) 
        {
            player.Car.Stats.CanDrive = false;
        }
        if(PreRoundStart != null) PreRoundStart();
        yield return StartCoroutine(Co_Countdown());

        State = RaceState.Racing;
        foreach(Player player in GameManager.Instance.ConnectedPlayers) player.Car.Stats.CanDrive = true;

        m_ActivePlayers.Clear();
        m_OffscreenPlayers.Clear();
        foreach(Player player in GameManager.Instance.ConnectedPlayers) m_ActivePlayers.Add(player);
        StartCoroutine(Co_CheckEliminations());

        if(OnRoundStart != null) OnRoundStart();
    }

    public void UpdatePlayerRaceInfo()
    {
        foreach(Player player in GameManager.Instance.ConnectedPlayers)
        {
            float trackProgress = ActiveTrack.GetNearestTimeOnSpline(player.Car.Position, 10, 5);
            if(Vector3.Distance(ActiveTrack.Evaluate(trackProgress).pos, player.Car.Position) < 5.0f)
            player.TrackProgress = trackProgress;
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
        if(GameManager.Instance.PlayerCount >= GameManager.MaxPlayers) return;

        string[] controllerNames = Input.GetJoystickNames();
        if(GameManager.Instance.PlayerCount >= controllerNames.Length) return;

        ControllerType controllerType = ControllerType.Playstation;
        if(controllerNames[GameManager.Instance.PlayerCount].ToLower().Contains("xbox")) controllerType = ControllerType.Xbox;

		Player player  = GameManager.Instance.AddPlayer(GameManager.Instance.PlayerCount + 1, controllerType);
	}

    public void SpawnPlayer(Player player, bool canDrive = false)
    {
        OrientedPoint op = ActiveTrack.Evaluate(m_TrackProgress);
        Vector3 spawnPoint = op.pos + Vector3.up * 1.05f;
        CoreCarModule car = Instantiate(m_CarPrefabs[player.PlayerID], spawnPoint, op.rot);
        car.Stats.CanDrive = canDrive;
        // if(player.ControllerID < 0 || player.ControllerID >= 4) player.AssignController(1); // this shouldn't be the case except for debugging in the race scene
        car.SetPlayer(player);
        player.Car = car;
    }

    private IEnumerator Co_Countdown()
    {
        if(CountdownStart != null) CountdownStart();
        yield return new WaitForSeconds(3.25f);
        if(CountdownEnd != null) CountdownEnd();
    }

    private IEnumerator Co_EndRound(Player winner)
    {
        State = RaceState.PostRace;

        foreach(Player player in GameManager.Instance.ConnectedPlayers) 
        {
            player.Car.Stats.CanDrive = false;
        }
        m_TrackProgress = winner.TrackProgress;

        m_ActivePlayers.Clear();
        m_OffscreenPlayers.Clear();

        if(OnRoundEnd != null) OnRoundEnd();

        yield return new WaitForSeconds(2.0f);

        winner.Car.Controller.PlayVictoryVFX();
        foreach(Player player in GameManager.Instance.ConnectedPlayers) 
        {
            player.Car.Stats.CurrSpeed = 0.0f;
            player.Car.Rigidbody.velocity = Vector3.zero;
            player.Car.Player.Laps = 0;
        }
        yield return new WaitForSeconds(0.25f);

        winner.Car.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.75f);

        if(winner.RoundWins >= GameManager.Rounds)
        {
            // end the game
            if(winner.RoundWins >= GameManager.Rounds && Player.OnGameWin != null) Player.OnGameWin(winner);
            StartCoroutine(Co_EndGame());
        }
        else
        {
            StartCoroutine(Co_StartRound(0.0f));
        }

    }

    private IEnumerator Co_EndGame()
    {
        if(OnGameEnd != null) OnGameEnd();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Scene_Menu");   
    }

    private IEnumerator Co_CheckEliminations()
    {
        while(RaceManager.State == RaceManager.RaceState.Racing)
        {
            List<Player> noLongerVisiblePlayers = new List<Player>();

            foreach(Player player in m_ActivePlayers)
            {
                bool visiblePlayer = IsPointInsideCameraFrustum(player.Car.Position);
                if (!visiblePlayer && !m_OffscreenPlayers.Contains(player)) noLongerVisiblePlayers.Add(player);
            }
            foreach (Player player in noLongerVisiblePlayers) StartCoroutine(Co_Eliminate(player.Car));
            if(m_ActivePlayers.Count == 1) m_ActivePlayers[0].WinRound();
            yield return null;
        }
        
    }

    public void EliminatePlayerImmediately(Player player)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/TukTuk/Elimination");
        m_ActivePlayers.Remove(player);
        player.Car.Controller.Eliminate();
        player.Car.gameObject.SetActive(false);

        if (OnPlayerEliminated != null)
        {
            OnPlayerEliminated(m_ActivePlayers.ToArray());
        }
    }

    private bool IsPointInsideCameraFrustum(Vector3 point)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(point);
        if ((viewportPosition.x > 1.0f || viewportPosition.x < 0.0f) || (viewportPosition.y > 1.0f || viewportPosition.y < 0.0f)) return false;
        return true;
    }

    private IEnumerator Co_Eliminate(CoreCarModule car)
    {
        float elapsed = 0.0f;

        while(elapsed < grace)
        {
            if(IsPointInsideCameraFrustum(car.Position))
            {
                // save player
            }
            yield return null;
        }

        car.gameObject.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/TukTuk/Elimination");
        m_ActivePlayers.Remove(car.Player);
        car.Controller.Eliminate();
        if (OnPlayerEliminated != null)
        {
            OnPlayerEliminated(m_ActivePlayers.ToArray());
        }
    }
}