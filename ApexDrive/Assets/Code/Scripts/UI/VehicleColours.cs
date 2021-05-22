using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleColours : MonoBehaviour
{
    private RaceManager carSystem;
    private RaceManager vehicleManager;
    private int numberOfCars;

    // Start is called before the first frame update
    void Start()
    {
        vehicleManager = this.GetComponent<RaceManager>();
        carSystem = vehicleManager.GetComponent<RaceManager>();
        numberOfCars = carSystem.raceCars.Count;

        for (int i = 0; i < numberOfCars; i++)
        {
            PositionUpdate processedCar = carSystem.raceCars[i];
            Transform sphereCarRoot = processedCar.gameObject.transform.parent;
            GameObject rickshawModel = sphereCarRoot.GetChild(2).gameObject;
            Material alterMat = rickshawModel.GetComponent<Renderer>().materials[10];

            switch (i)
            {
                case 0:
                    alterMat.SetColor("_BaseColor", Color.red);
                    break;

                case 1:
                    //alterMat.color = Color.blue;
                    alterMat.SetColor("_BaseColor", Color.blue);
                    break;

                case 2:
                    //alterMat.color = Color.green;
                    alterMat.SetColor("_BaseColor", Color.green);
                    break;

                case 3:
                    //alterMat.color = Color.yellow;
                    alterMat.SetColor("_BaseColor", Color.yellow);
                    break;
            }
        }
    }
}