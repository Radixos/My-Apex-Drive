using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameTagScript : MonoBehaviour
{
    private Canvas UICanvas;
    private TextMeshPro[] nameTags;
    private GameObject[] tagChildren;
    private Camera mainCamera;
    private int numberOfCars;
    private Vector3 offset = new Vector3(0.0f, 1.5f, 7.5f);
    //9.65f, -1.5f, 0.0f for Rad's scene
    //0.0f, 1.5f, 7.5f for LD-Old Scene

    void Start()
    {
        mainCamera = Camera.main;
        UICanvas = this.GetComponent<Canvas>();
        numberOfCars = GameManager.Instance.PlayerCount;
        nameTags = new TextMeshPro[numberOfCars];
        tagChildren = new GameObject[numberOfCars];

        for (int i = 0; i < numberOfCars; i++)
        {
            CoreCarModule processedCar = GameManager.Instance.ConnectedPlayers[i].Car;
            tagChildren[i] = new GameObject(processedCar.gameObject.name + " NAMETAG");
            tagChildren[i].transform.parent = UICanvas.gameObject.transform;
            TextMeshPro tempAddText = tagChildren[i].AddComponent<TextMeshPro>();
            tempAddText.text = processedCar.name;
            tempAddText.fontSize = 15;
            tempAddText.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f); // leveldesign POV
            tempAddText.outlineColor = Color.black;
            tempAddText.outlineWidth = 0.2f;
            nameTags[i] = tempAddText;
        }
    }

    void Update()
    {
        for (int i = 0; i < numberOfCars; i++)
        {
            CoreCarModule processedCar = GameManager.Instance.ConnectedPlayers[i].Car;
            
            if (processedCar.Player.State != Player.PlayerState.Eliminated)
            {
                Vector3 desiredPosition = processedCar.transform.position + offset;
                Vector3 desiredTagPosition = mainCamera.WorldToScreenPoint(desiredPosition);
                Vector3 viewPosition = mainCamera.WorldToScreenPoint(desiredPosition);
                tagChildren[i].transform.position = desiredPosition;
            }

            else
            {
                tagChildren[i].SetActive(false);
            }
        }
    }
}