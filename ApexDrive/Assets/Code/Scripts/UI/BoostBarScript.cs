using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostBarScript : MonoBehaviour
{
    private Canvas UICanvas; //overarching use variables
    [SerializeField] private RaceManager vehicleManager;
    private int vehicleNum;

    private GameObject[] boostBarObjects; 
    private Slider[] boostBarSliders; //object children
    private TextMeshProUGUI[] boostBarText;
    private Image[] boostBarImages;

    //private Gradient barGradient; //gradient related properties
    //private float colourTimer, alphaTimer;
    //GradientColorKey[] colourKeys;
    //GradientAlphaKey[] alphaKeys;

    void Start()
    {
        UICanvas = GetComponent<Canvas>();
        vehicleNum = vehicleManager.raceCars.Count;
        InitBarObjects();
        //InitGradient();
    }

    private void InitBarObjects()
    {
        boostBarObjects = new GameObject[vehicleNum]; //initialise objects and their children
        boostBarSliders = new Slider[vehicleNum];
        boostBarText = new TextMeshProUGUI[vehicleNum];
        boostBarImages = new Image[vehicleNum];

        for (int i = 0; i < vehicleNum; i++)
        {
            boostBarObjects[i] = UICanvas.transform.GetChild(i).gameObject;
            boostBarObjects[i].SetActive(true);
            boostBarSliders[i] = boostBarObjects[i].GetComponent<Slider>();
            boostBarText[i] = boostBarObjects[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            boostBarImages[i] = boostBarObjects[i].transform.GetChild(0).GetComponent<Image>();
        }
    }

    //private void InitGradient()
    //{
    //    barGradient = new Gradient();
    //    alphaKeys = new GradientAlphaKey[3];
    //    alphaTimer = 0.0f;

    //    for (int j = 0; j < alphaKeys.Length; j++)
    //    {
    //        alphaKeys[j].alpha = 1.0f;
    //        alphaKeys[j].time = alphaTimer;
    //        alphaTimer += 0.5f;
    //    }

    //    colourKeys = new GradientColorKey[3];
    //    colourKeys[0].color = Color.yellow;
    //    colourKeys[1].color = new Color(1.0f, 0.55f, 0.0f);
    //    colourKeys[2].color = Color.red;
    //    colourTimer = 0.0f;

    //    for (int k = 0; k < colourKeys.Length; k++)
    //    {
    //        colourKeys[k].time = colourTimer;
    //        colourTimer += 0.5f;
    //    }
    //    barGradient.SetKeys(colourKeys, alphaKeys);
    //}

    void Update()
    {
        SetSliders();
        ApplyText();
    }
    void SetSliders()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.ogRaceCars[i];
            AbilityCollision abilities = currentVehicle.gameObject.GetComponent<AbilityCollision>();
            boostBarSliders[i].value = abilities.carAbilities.powerAmount;
            //boostBarImages[i].color = barGradient.Evaluate(boostBarSliders[i].value);
            boostBarImages[i].color = Color.Lerp(Color.yellow, Color.red, boostBarSliders[i].value);
        }
    }

    private void ApplyText()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.ogRaceCars[i];

            // MANI'S UPDATE
            int pos = currentVehicle.GetPosition();

            switch (pos)
            {
                case 1:
                    boostBarText[i].text = "1ST";
                    break;
                case 2:
                    boostBarText[i].text = "2ND";
                    break;
                case 3:
                    boostBarText[i].text = "3RD";
                    break;
                case 4:
                    boostBarText[i].text = "4TH";
                    break;
            }
        }
    }
}