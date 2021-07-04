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
    private Slider[] boostBarSliders; //object children
    private TextMeshProUGUI[] boostBarText;
    private Image[] boostBarImages;

    void Start()
    {
        vehicleNum = GameManager.Instance.PlayerCount;
        InitBarObjects();
    }

    private void InitBarObjects()
    {
        boostBarObjects = new GameObject[vehicleNum]; //initialise objects and their children
        boostBarSliders = new Slider[vehicleNum];
        boostBarText = new TextMeshProUGUI[vehicleNum];
        boostBarImages = new Image[vehicleNum];

        for (int i = 0; i < vehicleNum; i++)
        {
            boostBarObjects[i] = barCollection.transform.GetChild(i).gameObject;
            boostBarObjects[i].SetActive(true);
            boostBarSliders[i] = boostBarObjects[i].GetComponent<Slider>();
            boostBarText[i] = boostBarObjects[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            boostBarImages[i] = boostBarObjects[i].transform.GetChild(0).GetComponent<Image>();
        }
    }

    void Update()
    {
        SetSliders();
        //ApplyText();
    }
    void SetSliders()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            CoreCarModule currentVehicle = GameManager.Instance.ConnectedPlayers[i].Car;
            CarStats vehicleStats = currentVehicle.gameObject.GetComponent<CarStats>();
            boostBarSliders[i].value = vehicleStats.PowerAmount;
            boostBarImages[i].color = Color.Lerp(Color.yellow, Color.red, boostBarSliders[i].value);
        }
    }

    //private void ApplyText()
    //{
    //    for (int i = 0; i < vehicleNum; i++)
    //    {
    //        CoreCarModule currentVehicle = GameManager.Instance.ConnectedPlayers[i].Car;
    //        // mani's update
    //        int pos = currentVehicle.GetPosition();
    //        switch (pos)
    //        {
    //            case 1:
    //                boostBarText[i].text = "1st";
    //                break;
    //            case 2:
    //                boostBarText[i].text = "2nd";
    //                break;
    //            case 3:
    //                boostBarText[i].text = "3rd";
    //                break;
    //            case 4:
    //                boostBarText[i].text = "4th";
    //                break;
    //        }
    //    }
    //}
}