using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleColours : MonoBehaviour
{
    private RaceManager vehicleManager;
    private int numberOfCars;

    void Start()
    {
        vehicleManager = this.GetComponent<RaceManager>();
        numberOfCars = vehicleManager.raceCars.Count;

        for (int i = 0; i < numberOfCars; i++)
        {
            PositionUpdate processedCar = vehicleManager.raceCars[i];
            Transform sphereCarRoot = processedCar.gameObject.transform.parent;
            GameObject rickshawModel = sphereCarRoot.GetChild(2).gameObject;
            Material alterMat = rickshawModel.GetComponent<Renderer>().materials[10];

            switch (i)
            {
                case 0:
                    alterMat.SetColor("_BaseColor", Color.red);
                    break;

                case 1:
                    alterMat.SetColor("_BaseColor", Color.blue);
                    break;

                case 2:
                    alterMat.SetColor("_BaseColor", Color.green);
                    break;

                case 3:
                    alterMat.SetColor("_BaseColor", Color.yellow);
                    break;
            }
        }
    }
}