using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    public Rigidbody sphereCollider;
    public Transform carModel;

    private float horizontal;
    private float vertical;

    [Header("Player Options")]
    [SerializeField]
    [Range(1, 4)]
    public int currentPlayer;
    private string horizontalInput;
    private string accelerateInput;
    private string brakeInput;
    private string driftInput;
    private string boostInput;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("The higher this is, the faster the car will need to be going before you can initiate a drift")]
    private float driftSpeedThresholdPercent;

    [Header("Turning Options")]
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
    private float maxTurnAngle;

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
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw(horizontalInput);
        vertical = Input.GetButton(accelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(brakeInput) ? 1 : 0;
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
        Physics.Raycast(carModel.position, -carModel.up, out hit, 1f);

        //Turn wheels/steering wheel based on horizontal input
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

        float targetAngle = currAngle + (horizontal * maxTurnAngle);
        float angle = Mathf.SmoothDamp(carModel.localEulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        if (isDrifting)
        {
            maxTurnAngle = 60;
        }
        else
        {
            maxTurnAngle = 25;
        }
        carModel.localEulerAngles = new Vector3(0, angle, 0);
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
            sphereCollider.AddForce(carModel.transform.forward * currSpeed, ForceMode.Acceleration);
        }
        else
        {
            sphereCollider.AddForce(carModel.transform.forward * currSpeed * currentBoostMultiplier, ForceMode.Acceleration);
        }
    }
}
