using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CoreCarModule car = other.GetComponent<CoreCarModule>();
        if(car != null)
        {
            RaceManager.Instance.EliminatePlayerImmediately(car.Player);
        }
    }
}
