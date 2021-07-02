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
            GameObject rickshawModel = processedCar.transform.GetChild(0).GetChild(27).gameObject;
            //Material alterMat = rickshawModel.GetComponent<Renderer>().materials[10];
            Material rickshawMat = rickshawModel.GetComponent<Material>();

            switch (i)
            {
                case 0:
                    rickshawMat.SetColor("_BaseColor", Color.red);
                    break;

                case 1:
                    rickshawMat.SetColor("_BaseColor", Color.green);
                    break;

                case 2:
                    rickshawMat.SetColor("_BaseColor", Color.yellow);
                    break;

                case 3:
                    rickshawMat.SetColor("_BaseColor", Color.blue);
                    break;
            }
        }
    }
}