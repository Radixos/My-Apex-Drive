// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private Animator m_FaderAnimator;
    [SerializeField] private bool[] m_PlayersReady;
    private Coroutine m_LoadGameroutine;

    private void Awake()
    {
    }

    private void Start()
    {
        GameManager.Instance.AddPlayer(1, ControllerType.Playstation);
        MultiplayerEventSystem.Current.AddPlayer(0);
        StartCoroutine(Co_InitialiseMenu());
    }

    private void OnEnable()
    {
        m_PlayersReady = new bool[GameManager.MaxPlayers];
    }

    private IEnumerator Co_InitialiseMenu()
    {
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            Player player = GameManager.Instance.ConnectedPlayers[i];
            if (player != null)
            {
                MultiplayerEventSystem.Current.AddPlayer(player.PlayerID);
            }
        }
        yield return null;
        MultiplayerEventSystem.Current.UpdateCursorPositions();
    }

    public void Ready(MultiplayerEventData data)
    {
        m_PlayersReady[data.Player.PlayerID] = true;
        int x = 0;
        for (int i = 0; i < GameManager.MaxPlayers; i++)
        {
            if (m_PlayersReady[i]) x++;
        }
        if (x >= 2 && x >= GameManager.Instance.PlayerCount)
        {
            m_LoadGameroutine = StartCoroutine(Co_LoadGameScene(3.5f));
        }
    }

    private IEnumerator Co_LoadGameScene(float delay)
    {
        yield return new WaitForSeconds(delay - 0.5f);
        //m_CanCancelLoading = false;
        // foreach (Player player in GameManager.Instance.ConnectedPlayers) m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        m_FaderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        // SceneManager.LoadScene("Scene_Demo_RoadGenerator");
        SceneManager.LoadScene("Scene_Level_01");
    }

    public void LoadMenuScene(MultiplayerEventData data)
    {
        StartCoroutine(Co_LoadMenuScene(1f));
    }

    private IEnumerator Co_LoadMenuScene(float delay)
    {
        yield return new WaitForSeconds(delay - 0.5f);
        //m_CanCancelLoading = false;
        // foreach (Player player in GameManager.Instance.ConnectedPlayers) m_LobbyPlayerSFX[player.PlayerID].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        m_FaderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        // SceneManager.LoadScene("Scene_Demo_RoadGenerator");
        SceneManager.LoadScene("Scene_Menu");
    }
}
