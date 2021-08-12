// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(50)]
public class LobbyMenu : MonoBehaviour
{
    private enum MenuState {Closed, Home, Settings, Controls, Loading}
    private MenuState m_State = MenuState.Closed;

    [SerializeField] private Animator m_FaderAnimator;
    [SerializeField] private Animator m_ControlsAnimator;
    [SerializeField] private Animator m_MenuAnimator;
    [SerializeField] private Animator[] m_PlayerPortraits;
    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private Animator[] m_CarAnimators;

    [SerializeField] private CanvasGroup m_MenuCanvasGroup;
    [SerializeField] private Image[] m_ControllerReadyCursors;

    [SerializeField] private TMPro.TMP_Text m_LoadingMessage;

    private bool[] m_PlayersReady;
    private Coroutine m_LoadGameroutine;

    private FMOD.Studio.EventInstance[] m_LobbyPlayerSFX = new FMOD.Studio.EventInstance[GameManager.MaxPlayers];
    private bool m_CanCancelLoading = true;
    private ControllerType[] m_Controllers;

    private List<LevelInfo> m_LevelVotes = new List<LevelInfo>();

    private LevelInfo m_SelectedLevel;
    private AsyncOperation m_AsynchronousSceneLoad;

    //VFX
    [SerializeField] private ParticleSystem m_BoostVFX;

    private void Awake()
    {
        for(int i = 0; i < m_LobbyPlayerSFX.Length; i++)
        {
            m_LobbyPlayerSFX[i] = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Player Idle");
        }

        string[] controllerNames = Input.GetJoystickNames();
        m_Controllers = new ControllerType[controllerNames.Length];
        for(int i = 0; i < controllerNames.Length; i++)
        {
            if(controllerNames[i].ToLower().Contains("xbox")) m_Controllers[i] = ControllerType.Xbox;
            else m_Controllers[i] = ControllerType.Playstation;
        }
        if(GameManager.Instance.PlayerCount > 0) m_State = MenuState.Home;
        else m_State = MenuState.Closed;
    }

    private void Start()
    {
        StartCoroutine(Co_InitialiseMenu());
    }

    private void OnEnable()
    {
        GameManager.OnPlayerConnected += OnPlayerConnected;
        GameManager.OnPlayerDisconnected += OnPlayerDisconnected;

        m_PlayersReady = new bool[GameManager.MaxPlayers];
        for(int i = 0; i < m_PlayersReady.Length; i++) m_PlayersReady[i] = false;

        m_FaderAnimator.SetBool("Visible", false);
        m_CanCancelLoading = true;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerConnected -= OnPlayerConnected;
        GameManager.OnPlayerDisconnected -= OnPlayerDisconnected;
    }

    public void Update()
    {
        string[] controllerNames = Input.GetJoystickNames();
        if(m_Controllers.Length != controllerNames.Length)
        {
            m_Controllers = new ControllerType[controllerNames.Length];
            for(int i = 0; i < controllerNames.Length; i++)
            {
                if(controllerNames[i].ToLower().Contains("xbox")) m_Controllers[i] = ControllerType.Xbox;
                else m_Controllers[i] = ControllerType.Playstation;
            }
        }

        // Connect new players by controller input if the menu is closed or on the home page
        if(m_State == MenuState.Closed || m_State == MenuState.Home)
        {
            for (int i = 1; i <= GameManager.MaxPlayers; i++)
            {
                if(i > m_Controllers.Length) break;

                if(GameManager.Instance.GetPlayerByController(i) == null)
                {
                    if(InputManager.GetButtonDown(m_Controllers[i-1], InputAction.Button_Face_1, i))
                    {
                        Player player = GameManager.Instance.AddPlayer(i, m_Controllers[i-1]);
                        MultiplayerEventSystem.Current.AddPlayer(player.PlayerID);
                    }
                }
            }
        }

        // Ready check for all players
        if(m_State == MenuState.Controls)
        {
            for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
            {
                if(!m_PlayersReady[i])
                {
                    Player player = GameManager.Instance.ConnectedPlayers[i];
                    if(InputManager.GetButtonDown(player.ControllerType, InputAction.Button_Face_1, player.ControllerID)) 
                    {
                        m_PlayersReady[i] = true;

                        int readyPlayerCount = 0;

                        for(int j = 0; j < GameManager.Instance.PlayerCount; j++)
                        {
                            if(m_PlayersReady[j])
                            {
                                readyPlayerCount++;
                                m_ControllerReadyCursors[j].fillAmount = (float)readyPlayerCount / (float) GameManager.Instance.PlayerCount;
                            }
                        }

                        if(readyPlayerCount == GameManager.Instance.PlayerCount)
                        {
                            m_State = MenuState.Loading;
                            StartCoroutine(Co_DismissControlsPage());
                        }
                    }
                }
            }
        }
    }

    public void DisconnectPlayer(MultiplayerEventData data)
    {
        Player player = data.Player;
        m_PlayersReady[player.PlayerID] = false;

        MultiplayerEventSystem.Current.RemovePlayer(player.PlayerID);
        m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", false);

        StartCoroutine(Co_DisconnectPlayer(player));
    }

    public void Ready(MultiplayerEventData data)
    {
        if(m_State == MenuState.Home)
        {
            m_PlayersReady[data.Player.PlayerID] = true;

            int x = 0;

            for(int i = 0; i < GameManager.MaxPlayers; i++)
            {
                if(m_PlayersReady[i]) x++;
            }

            if(x >= 2 && x >= GameManager.Instance.PlayerCount) 
            {
                m_State = MenuState.Loading;
                m_MenuCanvasGroup.interactable = false;
                StartCoroutine(Co_LoadControlsPage());
                for(int i = 0; i < m_PlayersReady.Length; i++) m_PlayersReady[i] = false;
                foreach(Player player in GameManager.Instance.ConnectedPlayers) m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    public void SubmitVoteForLevel(LevelInfo level)
    {
        m_LevelVotes.Add(level);
    }

    public void RemoveVoteForLevel(LevelInfo level)
    {
        m_LevelVotes.Remove(level);
    }

    public void CancelReady(MultiplayerEventData data)
    {
        if(m_State == MenuState.Home)
        {
            if(!m_CanCancelLoading || m_LoadGameroutine == null) return;
            m_PlayersReady[data.Player.PlayerID] = false;
            if(m_LoadGameroutine != null) StopCoroutine(m_LoadGameroutine);
        }
    }

    private void OnPlayerConnected(Player player)
    {
        if(m_State == MenuState.Closed)
        {
            m_State = MenuState.Home;
            StartCoroutine(Co_OpenMenu());
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
            m_State = MenuState.Closed;
            m_MenuAnimator.SetBool("IsOpen", false);
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Player Leave");
        }
        m_PlayersReady[player.PlayerID] = false;
        if(m_CarAnimators[player.PlayerID] != null)
        {
            m_CarAnimators[player.PlayerID].SetBool("IsActive", false);
        }
        m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator Co_LoadControlsPage()
    {
        m_LoadingMessage.text = "Loading...";
        m_FaderAnimator.SetBool("Visible", true);
        yield return new WaitForSeconds(1.0f);


        m_ControlsAnimator.SetBool("Visible", true);
        yield return new WaitForSeconds (1.0f);

        m_SelectedLevel = m_LevelVotes[Random.Range(0, m_LevelVotes.Count)];

        GameManager.Instance.CurrentGameInfo = new GameInfo(m_SelectedLevel);

        m_AsynchronousSceneLoad = SceneManager.LoadSceneAsync(m_SelectedLevel.SceneName);
        m_AsynchronousSceneLoad.allowSceneActivation = false;   

        while(m_AsynchronousSceneLoad.progress < 0.9f)
        {
            yield return null;
        }

        m_LoadingMessage.text = "Ready";

        m_State = MenuState.Controls;

    }

    private IEnumerator Co_DismissControlsPage()
    {
        m_ControlsAnimator.SetBool("Visible", false);
        yield return new WaitForSeconds(0.5f);

        // trigger scene load now
        while(m_AsynchronousSceneLoad.progress < 0.9f)
        {
            yield return null;
        }
        m_AsynchronousSceneLoad.allowSceneActivation = true;
    }

    private IEnumerator Co_InitialiseMenu()
    {
        if(m_State == MenuState.Closed) m_MenuAnimator.SetBool("IsOpen", false);
        if(m_State == MenuState.Home) m_MenuAnimator.SetBool("IsOpen", true);

        for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            Player player = GameManager.Instance.ConnectedPlayers[i];
            if(player != null)
            {
                if(m_PlayerPortraits[player.PlayerID] != null) m_PlayerPortraits[player.PlayerID].SetBool("IsVisible", true);
                MultiplayerEventSystem.Current.AddPlayer(player.PlayerID);
            }
        }
        yield return null;
        MultiplayerEventSystem.Current.UpdateCursorPositions();

        yield return new WaitForSeconds(0.5f);
        m_MenuCanvasGroup.interactable = true;
    }

    private IEnumerator Co_DisconnectPlayer(Player player)
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.RemovePlayerByID(player.PlayerID);
    }

    private IEnumerator Co_OpenMenu()
    {
        m_MenuAnimator.SetBool("IsOpen", true);
        yield return null;
        MultiplayerEventSystem.Current.UpdateCursorPositions();

        yield return new WaitForSeconds(0.5f);
        m_MenuCanvasGroup.interactable = true;
    }

    private IEnumerator Co_CloseMenu()
    {
        m_MenuAnimator.SetBool("IsOpen", false);
        m_MenuCanvasGroup.interactable = false;
        yield return new WaitForSeconds(0.5f);
    }
}
