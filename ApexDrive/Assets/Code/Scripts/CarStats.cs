
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{
    public CarAttributes carAttributes;
    public Rigidbody sphereCollider;



    void Start()
    {
        //Assign car attributes
        driftSpeedThresholdPercent = carAttributes.driftSpeedThresholdPercent;
        driftSideBoostMultiplier = carAttributes.driftSideBoostMultiplier;
        boostMultiplier = carAttributes.boostMultiplier;

        turnSpeed = carAttributes.turnSpeed;

        normalTurnAngle = carAttributes.normalTurnAngle;
        driftTurnAngle = carAttributes.driftTurnAngle;

        driftingAcceleration = carAttributes.driftingAcceleration;
        acceleration = carAttributes.acceleration;

        sphereCollider.drag = carAttributes.drag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
