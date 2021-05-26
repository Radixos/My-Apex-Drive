// Alec Gamble

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(50)]
[RequireComponent(typeof(Animator))]
public class LobbyMenu : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private Animator[] m_PlayerPortraits;
    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private bool[] m_PlayersReady;

    private Coroutine m_LoadGameRoutine;

    private FMOD.Studio.EventInstance[] m_LobbyPlayerSFX = new FMOD.Studio.EventInstance[GameManager.MaxPlayers];

    private bool m_MenuIsVisible = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        for(int i = 0; i < m_LobbyPlayerSFX.Length; i++)
        {
            m_LobbyPlayerSFX[i] = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Player Idle");
        }
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

        if(GameManager.Instance.PlayerCount > 0)
        {
            if(!m_MenuIsVisible)
            {
                if(m_Animator != null) m_Animator.SetBool("MenuOpen", true);
                m_MenuIsVisible = true;
            }
            foreach(Player player in GameManager.Instance.ConnectedPlayers)
            {

            }
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
            m_LoadGameRoutine = StartCoroutine(Co_LoadGameScene(3.5f));
        }
    }

    public void CancelReady(MultiplayerEventData data)
    {
        m_PlayersReady[data.Player.PlayerID] = false;
        m_Animator.SetBool("StartGame", false);
        StopCoroutine(m_LoadGameRoutine);
    }

    private void OnPlayerConnected(Player player)
    {
        if(!m_MenuIsVisible)
        {
            if(m_Animator != null) m_Animator.SetBool("MenuOpen", true);
            m_MenuIsVisible = true;
        }
        if(player != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Player Join");
        m_LobbyPlayerSFX[player.PlayerID].start();
    }

    private void OnPlayerDisconnected(Player player)
    {
        if(GameManager.Instance.PlayerCount == 0) 
        {
            if(m_Animator != null) m_Animator.SetBool("MenuOpen", false);
            m_MenuIsVisible = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Player Leave");
        }
        m_PlayersReady[player.PlayerID] = false;
        if(player != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", false);
        m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator Co_LoadGameScene(float delay)
    {
        float elapsed = 0.0f;
        while(elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("LevelDesignScene");
    }
}
