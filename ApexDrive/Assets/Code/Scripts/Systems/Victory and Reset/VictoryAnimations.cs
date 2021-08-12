// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryAnimations : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Avatars;
    [SerializeField] private GameObject[] m_Character1Avatars;
    [SerializeField] private GameObject[] m_Character2Avatars;
    [SerializeField] private GameObject[] m_Character3Avatars;
    [SerializeField] private GameObject[] m_Character4Avatars;
    [SerializeField] private GameObject[] m_SpotLights;
    [SerializeField] private GameObject[] m_Confetti;


    private void OnEnable()
    {
        if(GameManager.Instance.CurrentGameInfo == null || GameManager.Instance.CurrentGameInfo.Winner == null) return;

        List<int> remainingPlayerIDs = new List<int>();

        for(int i = 0; i < GameManager.Instance.PlayerCount; i++) remainingPlayerIDs.Add(GameManager.Instance.ConnectedPlayers[i].PlayerID);

        foreach (GameObject avatar in m_Character1Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character2Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character3Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character4Avatars) avatar.SetActive(false);

        remainingPlayerIDs.Remove(GameManager.Instance.CurrentGameInfo.Winner.PlayerID);
        m_SpotLights[GameManager.Instance.CurrentGameInfo.Winner.PlayerID].SetActive(true);
        m_Character1Avatars[GameManager.Instance.CurrentGameInfo.Winner.PlayerID].SetActive(true);
        m_Confetti[GameManager.Instance.CurrentGameInfo.Winner.PlayerID].SetActive(true);

        int characterID = remainingPlayerIDs[Random.Range(0, remainingPlayerIDs.Count)];
        m_Character2Avatars[characterID].SetActive(true);
        remainingPlayerIDs.Remove(characterID);


        if(GameManager.Instance.PlayerCount > 2)
        {
            characterID = remainingPlayerIDs[Random.Range(0, remainingPlayerIDs.Count)];
            m_Character3Avatars[characterID].SetActive(true);
            remainingPlayerIDs.Remove(characterID);
        }


        if(GameManager.Instance.PlayerCount > 3)
        {
            characterID = remainingPlayerIDs[Random.Range(0, remainingPlayerIDs.Count)];
            m_Character4Avatars[characterID].SetActive(true);
            remainingPlayerIDs.Remove(characterID);
        }

        m_Avatars[0].GetComponent<Animator>().SetTrigger("Celebrate");
        m_Avatars[0].GetComponent<Animator>().SetInteger("Celebration", 0);
        m_Avatars[0].GetComponent<Animator>().SetFloat("AnimationOffset", Random.Range(0.0f, 1.0f));

        for (int i = 1; i < m_Avatars.Length; i++)
        {
            m_Avatars[i].GetComponent<Animator>().SetTrigger("Defeat");
            m_Avatars[i].GetComponent<Animator>().SetInteger("Celebration", Random.Range(0, 2));
            m_Avatars[i].GetComponent<Animator>().SetFloat("AnimationOffset", Random.Range(0.0f, 1.0f));
        }
    }
}
