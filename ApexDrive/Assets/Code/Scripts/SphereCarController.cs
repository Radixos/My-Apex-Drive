﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SphereCarController : MonoBehaviour
{
    public CarAttributes carAttributes;
    public Rigidbody sphereCollider;
    public Transform carModel;

    private float horizontal;
    private float vertical;

    [Header("Player Options")]
    [Tooltip("This is assigned on Start so this cannot be changed in run time!")]
    [SerializeField]
    [Range(1, 4)]
    public int currentPlayer;
    private string horizontalInput;
    private string accelerateInput;
    private string brakeInput;
    private string driftInput;
    private string boostInput;
    private bool inAir;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("The higher this is, the faster the car will need to be going before you can initiate a drift (likely does nothing)")]
    private float driftSpeedThresholdPercent;
    [SerializeField]
    private float driftSideBoostMultiplier;

    [Header("Boost Options")]
    [SerializeField]
    private float currentBoostMultiplier;
    [SerializeField]
    private float boostMultiplier;

    [Header("Turning Options")]
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float turnVelocity;
    [SerializeField]
    private float currAngle;
    [SerializeField]
    private float currTurnAngle;
    [SerializeField]
    private float normalTurnAngle;
    [SerializeField]
    private float driftTurnAngle;

    [Header("Speed Options")]
    [SerializeField]
    private float driftingAcceleration;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float currSpeed;
    [SerializeField]
    private float maxSpeed;

    private void Start()
    {
        //Assign controller at start. Could be done in update if we want to swap player controls mid game?
        
        //Input clarification: 
        //Brake is actually reverse!!! To simulate controls similar to Rocket League.
        horizontalInput = "Horizontal " + currentPlayer;
        accelerateInput = "Accelerate " + currentPlayer;
        brakeInput = "Brake " + currentPlayer;
        driftInput = "Drift " + currentPlayer;
        boostInput = "Boost " + currentPlayer;

        //Assign car attributes
        driftSpeedThresholdPercent = carAttributes.driftSpeedThresholdPercent;
        driftSideBoostMultiplier = carAttributes.driftSideBoostMultiplier;
        boostMultiplier = carAttributes.boostMultiplier;

        normalTurnAngle = carAttributes.normalTurnAngle;
        driftTurnAngle = carAttributes.driftTurnAngle;

        driftingAcceleration = carAttributes.driftingAcceleration;
        acceleration = carAttributes.acceleration;

        sphereCollider.drag = carAttributes.drag;
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw(horizontalInput);
        vertical = Input.GetButton(accelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(brakeInput) ? 0.5f : 0;
        currentBoostMultiplier = Input.GetButton(boostInput) ? boostMultiplier : 1;
    }

    void FixedUpdate()
    {
        //Follow Collider
        carModel.position = sphereCollider.position;

        HandleMovement();
        HandleSteering();
        HandleAnimation();
    }

    void HandleAnimation()
    {
        //Raycast down - angle model based on normal of floor
        RaycastHit hit;
        if (Physics.Raycast(carModel.position, Vector3.down, out hit, 3f))
        {
            inAir = false;
            Vector3 newUp = hit.normal;
            Vector3 oldForward = carModel.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);

            carModel.rotation = Quaternion.Lerp(carModel.rotation, Quaternion.LookRotation(newForward, newUp), Time.deltaTime * 8.0f);
        } else
        {
            inAir = true;
        }
    }

    void HandleSteering()
    {
        if (currSpeed <= 0.1f && currSpeed >= -.1f)
        {
            return;
        }

        if(Input.GetButton(driftInput) && horizontal != 0 && acceleration / currSpeed >= driftSpeedThresholdPercent)
        {
            isDrifting = true;
        } else
        {
            isDrifting = false;
        }

        float targetAngle = currAngle + (horizontal * currTurnAngle);
        float angle = Mathf.SmoothDamp(carModel.localEulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        if (isDrifting)
        {
            currTurnAngle = driftTurnAngle;
        }
        else
        {
            currTurnAngle = normalTurnAngle;
        }
        carModel.localEulerAngles = new Vector3(carModel.localEulerAngles.x, angle, carModel.localEulerAngles.z);
        currAngle = carModel.localEulerAngles.y;
    }

    void HandleMovement()
    {
        if (isDrifting)
        {
            maxSpeed = vertical * driftingAcceleration;
        } else
        {
            maxSpeed = vertical * acceleration * currentBoostMultiplier;
        }

        currSpeed = Mathf.SmoothStep(currSpeed, maxSpeed, Time.deltaTime * 12f);

        //Forward Acceleration
        if (isDrifting)
        {
            sphereCollider.AddForce(carModel.transform.right * currSpeed * -horizontal * 0.5f, ForceMode.Acceleration);
            sphereCollider.AddForce(carModel.transform.forward * currSpeed * driftSideBoostMultiplier, ForceMode.Acceleration);
        }
        else
        {
            sphereCollider.AddForce(carModel.transform.forward * currSpeed * currentBoostMultiplier, ForceMode.Acceleration);
        }
    }
}
