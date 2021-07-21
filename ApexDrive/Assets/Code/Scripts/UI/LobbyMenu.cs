// Alec Gamble

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(50)]
public class LobbyMenu : MonoBehaviour
{
    // private Animator m_Animator;
    [SerializeField] private Animator m_TimerAnimator;
    [SerializeField] private Animator m_FaderAnimator;
    [SerializeField] private Animator m_MenuAnimator;
    [SerializeField] private Animator[] m_PlayerPortraits;
    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private Animator[] m_CarAnimators;

    [SerializeField] private bool[] m_PlayersReady;
    private Coroutine m_LoadGameroutine;

    private FMOD.Studio.EventInstance[] m_LobbyPlayerSFX = new FMOD.Studio.EventInstance[GameManager.MaxPlayers];

    private bool m_MenuIsVisible = false;
    private bool m_CanCancelLoading = true;



    private void Awake()
    {
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

        m_FaderAnimator.SetTrigger("FadeIn");
        if(GameManager.Instance.PlayerCount > 0)
        {
            if(m_MenuIsVisible)
            {
                m_MenuAnimator.SetBool("IsOpen", true);
                // m_MenuIsVisible = true;
            }
            foreach(Player player in GameManager.Instance.ConnectedPlayers)
            {
                // instantiate cursor
            }
        }

        m_CanCancelLoading = true;
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
            m_TimerAnimator.SetTrigger("Countdown");
            m_LoadGameroutine = StartCoroutine(Co_LoadGameScene(3.5f));
        }
    }

    public void CancelReady(MultiplayerEventData data)
    {
        if(!m_CanCancelLoading || m_LoadGameroutine == null) return;
        m_PlayersReady[data.Player.PlayerID] = false;
        m_TimerAnimator.SetTrigger("CancelCountdown");
        if(m_LoadGameroutine != null) StopCoroutine(m_LoadGameroutine);
    }

    private void OnPlayerConnected(Player player)
    {
        if(!m_MenuIsVisible)
        {
            m_MenuAnimator.SetBool("IsOpen", true);
            m_MenuIsVisible = true;
        }
        if(player != null && m_PlayerPortraits[player.PlayerID] != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", true);
        if(m_CarAnimators[player.PlayerID] != null)
        {
            m_CarAnimators[player.PlayerID].SetBool("IsActive", true);
            m_CarAnimators[player.PlayerID].SetFloat("Blend", player.PlayerID);
        }
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Player Join");
        m_LobbyPlayerSFX[player.PlayerID].start();
    }

    private void OnPlayerDisconnected(Player player)
    {
        if(GameManager.Instance.PlayerCount == 0) 
        {
            m_MenuAnimator.SetBool("IsOpen", false);
            m_MenuIsVisible = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Player Leave");
        }
        m_PlayersReady[player.PlayerID] = false;
        if(player != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", false);
        if(m_CarAnimators[player.PlayerID] != null)
        {
            m_CarAnimators[player.PlayerID].SetBool("IsActive", false);
        }
        m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator Co_LoadGameScene(float delay)
    {
        yield return new WaitForSeconds(delay - 0.5f);
        m_CanCancelLoading = false;
        foreach(Player player in GameManager.Instance.ConnectedPlayers) m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        m_FaderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Scene_Demo_RoadGenerator");
    }
}
