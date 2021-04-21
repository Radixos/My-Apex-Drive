using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    private Vector3 carCameraPos;
    private RaceManager carManager;
    private Camera mainCamera;
    private float waitTimer = 2.5f;

    void Start()
    {
        mainCamera = Camera.main;
        carManager = this.GetComponent<RaceManager>();
    }

    void Update()
    {
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCar = carManager.raceCars[i];
            carCameraPos = mainCamera.WorldToViewportPoint(currentCar.transform.position);
            bool boundaryCheck = checkBoundaries(carCameraPos);

            if (boundaryCheck == true)
            {
                if (currentCar.offScreenTimer < waitTimer)
                {
                    currentCar.offScreenTimer += Time.deltaTime;
                }
                
                if (currentCar.offScreenTimer >= waitTimer)
                {
                    currentCar.gameObject.SetActive(false);
                }
            }

            else
            {
                currentCar.offScreenTimer = 0;
            }
         }
    }

    private bool checkBoundaries(Vector3 positionToCheck)
    {
        if ((positionToCheck.x > 1.0f || positionToCheck.x < 0.0f) || (positionToCheck.y > 1.0f || positionToCheck.y < 0.0f))
        {
            return true;
        }

        else {return false;}
    }
}