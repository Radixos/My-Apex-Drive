// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private Animator m_FaderAnimator;
    private bool[] m_PlayersReady;
    [SerializeField] private Image[] m_ControllerReadyCursors;

    private void OnEnable()
    {
        m_PlayersReady = new bool[GameManager.MaxPlayers];
    }

    private void Update()
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
                            StartCoroutine(Co_LoadMenuScene());
                        }
                    }
                }
            }
    }

    private IEnumerator Co_LoadMenuScene()
    {
        m_FaderAnimator.SetBool("Visible", true);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Scene_Menu");
    }
}
