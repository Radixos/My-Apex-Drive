using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameTagScript : MonoBehaviour
{
    private Canvas UICanvas;
    private RaceManager carSystem;
    private TextMeshPro[] nameTags;
    private GameObject[] tagChildren;
    private Camera mainCamera;
    [SerializeField] private GameObject raceManager;
    private int numberOfCars;
    private Vector3 offset = new Vector3(0.0f, 1.5f, 8.0f);
    //9.65f, -1.5f, 0.0f for Rad's scene

    void Start()
    {
        mainCamera = Camera.main;
        UICanvas = this.GetComponent<Canvas>();
        carSystem = raceManager.GetComponent<RaceManager>();
        numberOfCars = carSystem.raceCars.Count;
        nameTags = new TextMeshPro[numberOfCars];
        tagChildren = new GameObject[numberOfCars];

        //for (int i = 0; i < numberOfCars; i++)
        //{
        //    PositionUpdate processedCar = carSystem.raceCars[i];

        //    if (processedCar.eliminated == false)
        //    {
        //        //couldn't find a way to create TextMeshPro objects similar to others, 
        //        //so each in list added via AddComponent
        //        nameTags[i] = tagChildren[i].AddComponent<TextMeshPro>();
        //        tagChildren[i].transform.parent = processedCar.gameObject.transform;
        //    }
        //}

        for (int i = 0; i < numberOfCars; i++)
        {
            PositionUpdate processedCar = carSystem.raceCars[i];
            tagChildren[i] = new GameObject(processedCar.name + " NAMETAG");
            tagChildren[i].transform.parent = UICanvas.gameObject.transform;
            TextMeshPro tempAddText = tagChildren[i].AddComponent<TextMeshPro>();
            tempAddText.text = processedCar.name;
            tempAddText.fontSize = 10;
            tempAddText.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f); // leveldesign POV
            tempAddText.outlineColor = Color.black;
            tempAddText.outlineWidth = 0.5f;
            //tempAddText.font = Resources.Load("Fonts & Materials/Apex SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
            tempAddText.font.material.shader = Shader.Find("TextMeshPro/Distance Field");
            nameTags[i] = tempAddText;
        }
    }

    void Update()
    {
        //Camera.WorldToScreenPoint

        for (int i = 0; i < numberOfCars; i++)
        {
            PositionUpdate processedCar = carSystem.raceCars[i];
            
            if (processedCar.eliminated == false)
            {
                Vector3 desiredPosition = processedCar.transform.position + offset;
                Vector3 desiredTagPosition = mainCamera.WorldToScreenPoint(desiredPosition);
                Vector3 viewPosition = mainCamera.WorldToScreenPoint(desiredPosition);
                //tagChildren[i].transform.position = viewPosition;
                tagChildren[i].transform.position = desiredPosition;
                //tagChildren[i].transform.eulerAngles = new Vector3(0.0f, processedCar.transform.eulerAngles.y - 90.0f, 0.0f);
            }
        }
    }
}
