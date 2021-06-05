using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostBarScript : MonoBehaviour
{
    private Canvas UICanvas;
    [SerializeField] private RaceManager vehicleManager;
    private int vehicleNum;

    private GameObject[] boostBarObjects;
    private Slider[] boostBarSliders;
    private TextMeshProUGUI[] boostBarText;
    private Image[] boostBarImages;

    private Gradient barGradient;
    private float colourTimer, alphaTimer;
    GradientColorKey[] colourKeys;
    GradientAlphaKey[] alphaKeys;

    void Start()
    {
        UICanvas = GetComponent<Canvas>();
        vehicleNum = vehicleManager.raceCars.Count;
        InitBarObjects();
        InitGradient();
    }

    private void InitBarObjects()
    {
        boostBarObjects = new GameObject[vehicleNum];
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

    private void InitGradient()
    {
        barGradient = new Gradient();
        alphaKeys = new GradientAlphaKey[3];
        alphaTimer = 0.0f;

        for (int j = 0; j < alphaKeys.Length; j++)
        {
            alphaKeys[j].alpha = 1.0f;
            alphaKeys[j].time = alphaTimer;
            alphaTimer += 0.5f;
        }

        colourKeys = new GradientColorKey[3];
        colourKeys[0].color = Color.yellow;
        colourKeys[1].color = new Color(1.0f, 0.55f, 0.0f);
        colourKeys[2].color = Color.red;
        colourTimer = 0.0f;

        for (int k = 0; k < colourKeys.Length; k++)
        {
            colourKeys[k].time = colourTimer;
            colourTimer += 0.5f;
        }
        barGradient.SetKeys(colourKeys, alphaKeys);
    }

    void Update()
    {
        SetSliders();
        DeterminePositions();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            vehicleManager.raceCars[0].gameObject.GetComponent<AbilityCollision>().carAbilities.powerAmount -= 0.15f * Time.deltaTime;
        }
    }
    void SetSliders()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.raceCars[i];
            AbilityCollision abilities = currentVehicle.gameObject.GetComponent<AbilityCollision>();
            boostBarSliders[i].value = abilities.carAbilities.powerAmount;
            boostBarImages[i].color = barGradient.Evaluate(boostBarSliders[i].value);
        }
    }

    void DeterminePositions()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.raceCars[i];
            currentVehicle.aheadOf = 0;

            for (int j = 0; j < vehicleNum; j++)
            {
                if (vehicleManager.raceCars[i].gameObject.GetInstanceID() !=
                   vehicleManager.raceCars[j].gameObject.GetInstanceID())
                {
                    if ((vehicleManager.raceCars[i].collidersHit > vehicleManager.raceCars[j].collidersHit) ||
                        (vehicleManager.raceCars[i].laps > vehicleManager.raceCars[j].laps))
                    {
                        currentVehicle.aheadOf++;
                    }
                }

                Debug.Log(vehicleManager.raceCars[i].gameObject.GetInstanceID());
            }   
        }
        ApplyText();
    }

    private void ApplyText()
    {
        for (int i = 0; i < vehicleNum; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.raceCars[i];
            
            if (currentVehicle.aheadOf == vehicleNum - 1)
            {
                //currentVehicle.ranking = PositionUpdate.VehiclePosition.FIRST;
                boostBarText[i].text = "1ST";
            }

            else if (currentVehicle.aheadOf == vehicleNum - 2)
            {
                boostBarText[i].text = "2ND";
            }

            else if (currentVehicle.aheadOf == vehicleNum - 3)
            {
                boostBarText[i].text = "3RD";
            }

            else
            {
                 boostBarText[i].text = "4TH";
            }
            
            //switch (currentVehicle.aheadOf)
            //{ 
            //    case (vehicleNum):
            //        break;
            //}
        }
    }
}