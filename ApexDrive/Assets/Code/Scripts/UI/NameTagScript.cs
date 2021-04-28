using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameTagScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas UICanvas;
    private RaceManager carSystem;
    private TextMeshPro[] nameTags;
    private GameObject[] tagChildren;
    private Camera mainCamera;
    [SerializeField] private GameObject raceManager;
    private int numberOfCars;
    private Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
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
            nameTags[i] = tempAddText;
        }
    }

    // Update is called once per frame
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
                tagChildren[i].transform.position = viewPosition;
            }
        }
    }
}
