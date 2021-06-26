using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollision : MonoBehaviour
{
    public CarStats carStats;
    public CarController sphereCarController;

    private void OnCollisionEnter(Collision collision)
    {

        // Only check collision if the car has activated rampage
        if (collision.gameObject.CompareTag("PlayerTuk") && carStats.Rampage.activeSelf)
        {
            Vector3 normal = collision.contacts[0].normal;

            if (collision.gameObject.GetComponent<AbilityCollision>().carStats.Shield.activeSelf)
            {
                sphereCarController.Impact(200, normal, 0.75f);
            }
            else
            {
                collision.gameObject.GetComponent<AbilityCollision>().sphereCarController.Impact(200, -normal, 0.75f);
            }

        }

    }
}
