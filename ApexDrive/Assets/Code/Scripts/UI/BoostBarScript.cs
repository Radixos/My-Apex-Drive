using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostBarScript : MonoBehaviour
{
    private Canvas UICanvas;
    [SerializeField] private RaceManager vehicleManager;
    private GameObject[] boostBarObjects;

    private Slider[] boostBarSliders;
    private Image[] boostBarImages;
    private TextMeshProUGUI[] boostBarText;
    private Gradient barGradient;
    public Gradient testGradient;
    private float colourTimer, alphaTimer;

    GradientColorKey[] colourKeys;
    GradientAlphaKey[] alphaKeys;

    void Start()
    {
        //set gradients
        //testDriveScript[i].powerAmount
        UICanvas = GetComponent<Canvas>();
        int vehicleNum = vehicleManager.raceCars.Count;
        boostBarObjects = new GameObject[vehicleNum];
        boostBarSliders = new Slider[vehicleNum];
        boostBarImages = new Image[vehicleNum];
        boostBarText = new TextMeshProUGUI[vehicleNum];

        for (int i = 0; i < vehicleNum; i++)
        {
            boostBarObjects[i] = UICanvas.transform.GetChild(i).gameObject;
            boostBarObjects[i].SetActive(true);
            boostBarSliders[i] = boostBarObjects[i].GetComponent<Slider>();
            boostBarImages[i] = boostBarObjects[i].transform.GetChild(0).GetComponent<Image>();
            boostBarText[i] = boostBarObjects[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        InitGradient();
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
        SetText();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            vehicleManager.raceCars[0].gameObject.GetComponent<AbilityCollision>().carAbilities.powerAmount -= 0.15f * Time.deltaTime;
        }
    }
    void SetSliders()
    {
        for (int i = 0; i < vehicleManager.raceCars.Count; i++)
        {
            PositionUpdate currentVehicle = vehicleManager.raceCars[i];
            AbilityCollision abilities = currentVehicle.gameObject.GetComponent<AbilityCollision>();
            boostBarSliders[i].value = abilities.carAbilities.powerAmount;
            boostBarImages[i].color = barGradient.Evaluate(boostBarSliders[i].value);
        }
    }
    void SetText()
    {
        for (int i = 0; i < vehicleManager.raceCars.Count; i++)
        {
            PositionUpdate currentvehicle = vehicleManager.raceCars[i];
        }
    }
}