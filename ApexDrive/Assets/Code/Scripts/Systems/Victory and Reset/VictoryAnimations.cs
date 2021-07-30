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


    private void Awake()
    {
        Player temporaryPlayer = new Player(Random.Range(0, 4), Color.red);
        SubmitWinner(temporaryPlayer);
        Debug.Log(temporaryPlayer.PlayerID);
        Debug.Log(m_Character1Avatars[temporaryPlayer.PlayerID].name);
    }

    public void SubmitWinner(Player winner)
    {
        foreach (GameObject avatar in m_Character1Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character2Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character3Avatars) avatar.SetActive(false);
        foreach (GameObject avatar in m_Character4Avatars) avatar.SetActive(false);

        m_Character1Avatars[winner.PlayerID].SetActive(true);
        m_Character2Avatars[Random.Range(4, m_Character2Avatars.Length)].SetActive(true);
        m_Character3Avatars[Random.Range(4, m_Character2Avatars.Length)].SetActive(true);
        m_Character4Avatars[Random.Range(4, m_Character2Avatars.Length)].SetActive(true);

        m_Avatars[0].GetComponent<Animator>().SetTrigger("Celebrate");
        m_Avatars[0].GetComponent<Animator>().SetInteger("Celebration", 0);
        for (int i = 1; i < m_Avatars.Length; i++)
        {
            m_Avatars[i].GetComponent<Animator>().SetTrigger("Defeat");
            m_Avatars[i].GetComponent<Animator>().SetInteger("Celebration", Random.Range(0, 2));
        }
    }
}
