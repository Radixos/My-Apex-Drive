using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleColours : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            CoreCarModule processedCar = GameManager.Instance.ConnectedPlayers[i].Car;
            GameObject rickshawTop = processedCar.transform.GetChild(0).GetChild(27).gameObject;
            Material rickshawColour = rickshawTop.GetComponent<Material>();

            switch (i)
            {
                case 0:
                    rickshawColour.SetColor("_BaseColor", Color.red);
                    break;

                case 1:
                    rickshawColour.SetColor("_BaseColor", Color.green);
                    break;

                case 2:
                    rickshawColour.SetColor("_BaseColor", Color.yellow);
                    break;

                case 3:
                    rickshawColour.SetColor("_BaseColor", Color.blue);
                    break;
            }
        }
    }
}