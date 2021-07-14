using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostBarScript : MonoBehaviour
{
    [SerializeField] private GameObject barCollection; //overarching use variables
    private int vehicleNum;

    private GameObject[] boostBarObjects;
    private Image[] boostBarMeters;
    private TextMeshProUGUI[] boostBarText;

    void Start()
    {
        //vehicleNum = GameManager.Instance.PlayerCount;
        vehicleNum = 4; //temporary!!!
        InitBarObjects();
    }

    private void InitBarObjects()
    {
        boostBarObjects = new GameObject[vehicleNum]; //initialise objects and their children
        boostBarMeters = new Image[vehicleNum];
        boostBarText = new TextMeshProUGUI[vehicleNum];

        for (int i = 0; i < vehicleNum; i++)
        {
            boostBarObjects[i] = barCollection.transform.GetChild(i).gameObject;
            boostBarObjects[i].SetActive(true);
            boostBarMeters[i] = boostBarObjects[i].transform.GetChild(0).GetComponent<Image>();
            boostBarText[i] = boostBarObjects[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            CoreCarModule currentVehicle = GameManager.Instance.ConnectedPlayers[i].Car;
            SetSliders(currentVehicle, i);
            ApplyText(currentVehicle, i);
        }
       
    }
    void SetSliders(CoreCarModule vehicle, int index)
    {
        CarStats vehicleStats = vehicle.gameObject.GetComponent<CarStats>();
        boostBarMeters[index].fillAmount = vehicleStats.PowerAmount;
        boostBarMeters[index].color = Color.Lerp(Color.yellow, Color.red, boostBarMeters[index].fillAmount);
    }

    private void ApplyText(CoreCarModule vehicle, int index)
    {
            int pos = vehicle.Player.Position;
            switch (pos)
            {
                case 1:
                    boostBarText[index].text = "1st";
                    break;
                case 2:
                    boostBarText[index].text = "2nd";
                    break;
                case 3:
                    boostBarText[index].text = "3rd";
                    break;
                case 4:
                    boostBarText[index].text = "4th";
                    break;
            }
    }
}