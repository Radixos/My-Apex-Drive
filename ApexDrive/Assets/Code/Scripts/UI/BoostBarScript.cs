using UnityEngine;
using UnityEngine.UI;

public class BoostBarScript : MonoBehaviour
{
    private GameObject[] boostBarObjects;
    [SerializeField] private Image[] m_Meters;
    [SerializeField] private GameObject[] m_HUDs;

    private void Start()
    {
        UpdateUIElements(null);
    }

    private void OnEnable()
    {
        GameManager.OnPlayerConnected += UpdateUIElements;
        GameManager.OnPlayerDisconnected += UpdateUIElements;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerConnected -= UpdateUIElements;
        GameManager.OnPlayerDisconnected -= UpdateUIElements;
    }

    private void UpdateUIElements(Player player)
    {
        for(int i = 0; i < 4; i++)
        {
            if(i < GameManager.Instance.PlayerCount) m_HUDs[i].SetActive(true);
            else m_HUDs[i].SetActive(false);
        }
    }

    private void Update()
    {
        for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            if(GameManager.Instance.ConnectedPlayers[i].Car != null) m_Meters[i].fillAmount = GameManager.Instance.ConnectedPlayers[i].Car.Stats.PowerAmount;
        }
    }
}