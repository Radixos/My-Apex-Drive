using UnityEngine;
using UnityEngine.UI;

public class BoostBarScript : MonoBehaviour
{
    private GameObject[] boostBarObjects;
    [SerializeField] private Image[] m_Meters;
    [SerializeField] private GameObject[] m_HUDs;

    [SerializeField] private Image[] m_BoostButtonIndicators;
    [SerializeField] private Animator[] m_BoostButtonAnimators;
    [SerializeField] private Sprite m_BoostButtonIconXbox;
    [SerializeField] private Sprite m_BoostButtonIconPS4;

    private void OnEnable()
    {

        for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            if(i < GameManager.Instance.PlayerCount) 
            {
                m_HUDs[i].SetActive(true);
                m_BoostButtonIndicators[i].sprite = GameManager.Instance.ConnectedPlayers[i].ControllerType == ControllerType.Xbox ? m_BoostButtonIconXbox : m_BoostButtonIconPS4;
            }
            else m_HUDs[i].SetActive(false);
        }

        foreach(Image meter in m_Meters) meter.fillAmount = 0.0f;

    }

    private void Update()
    {
        for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            Player player = GameManager.Instance.ConnectedPlayers[i];
            if(player.Car != null)
            {
                if(GameManager.Instance.ConnectedPlayers[i].Car != null) m_Meters[i].fillAmount = player.Car.Stats.PowerAmount;
                if(player.Car.Stats.PowerAmount > player.Car.Stats.BoostCost) 
                {
                    m_BoostButtonAnimators[i].SetBool("CanBoost", true);
                    m_Meters[i].color = GameManager.Instance.PlayerColors[i];
                }
                else 
                {
                    m_BoostButtonAnimators[i].SetBool("CanBoost", false);
                    m_Meters[i].color = Color.grey;
                }
            }
        }
    }
}