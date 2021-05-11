using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SphereCarController : MonoBehaviour
{
    public CarAttributes carAttributes;
    public Rigidbody sphereCollider;
    public Transform model;

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

    [SerializeField]
    private bool inAir;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    [Tooltip("The higher this is, the faster % of max speed the car will need to be going before you can initiate a drift (does basically nothing)")]
    private float driftSpeedThresholdPercent;
    [SerializeField]
    [Tooltip("The higher this is, the faster/wider the car will swing when drifting")]
    private float driftSideBoostMultiplier;

    [Header("Boost Options")]
    [SerializeField]
    private float currentBoostMultiplier;
    [SerializeField]
    [Tooltip("The higher this is, the faster car will go when drifting")]
    private float boostMultiplier;

    [Header("Turning Options")]
    [SerializeField]
    [Tooltip("Lowering this effectively increases the turn angle.")]
    private float turnSpeed;
    [SerializeField]
    private float turnVelocity;
    [SerializeField]
    private float currAngle;
    [SerializeField]
    private float targetAngle;
    [SerializeField]
    [Tooltip("This determines the car's turning speed while not boosting")]
    private float normalTurnAngle;
    [SerializeField]
    [Tooltip("This determines the car's turning speed while boosting")]
    private float driftTurnAngle;

    [Header("Speed Options")]
    [SerializeField]
    [Tooltip("This determines the car's accelerationg while not boosting")]
    private float acceleration;
    [SerializeField]
    [Tooltip("This determines the car's accelerationg while boosting")]
    private float driftingAcceleration;
    [SerializeField]
    private float currSpeed;
    [SerializeField]
    private float maxSpeed;

    [Header("DEBUG")]
    [SerializeField]
    private bool initialDriftDirectionRight;
    [SerializeField]
    private float targetMinusCurrAngle;

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

        turnSpeed = carAttributes.turnSpeed;

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

        HandleAnimation();
    }

    void FixedUpdate()
    {
        //Follow Collider
        transform.position = sphereCollider.position - new Vector3(0, -0.5f, 0);

        if (!inAir)
        {
            sphereCollider.drag = carAttributes.drag;
            HandleMovement();
            HandleSteering();
        }
        else
        {
            sphereCollider.drag = 0.05f;
        }
    }

    void HandleAnimation()
    {
        model.transform.position = transform.position;
        //Raycast down - angle model based on normal of floor
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 4f))
        {
            inAir = false;

            Vector3 newUp = hit.normal;
            Vector3 oldForward = transform.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);

            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(newForward, newUp), Time.deltaTime * 8f);

            model.localEulerAngles = new Vector3(model.localEulerAngles.x, model.localEulerAngles.y, horizontal * currSpeed * 0.1f);
        } else
        {
            inAir = true;
        }
    }

    void HandleSteering()
    {
        targetMinusCurrAngle = currAngle - targetAngle;
        if (currSpeed <= 0.1f && currSpeed >= -.1f)
        {
            return;
        }

        if (Input.GetButton(driftInput) && horizontal != 0 && acceleration / currSpeed >= driftSpeedThresholdPercent)
        {
            if (!isDrifting)
            {
                initialDriftDirectionRight = horizontal > 0;
                sphereCollider.AddForce(transform.transform.right * currSpeed * -horizontal * 0.5f, ForceMode.Acceleration);
            }
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        if (isDrifting)
        {
            float bonusDriftAngle = initialDriftDirectionRight == true ? driftTurnAngle : -driftTurnAngle;
            targetAngle = currAngle + (((horizontal * driftTurnAngle) + bonusDriftAngle) / 2);
        }
        else
        {
            targetAngle = currAngle + (horizontal * normalTurnAngle);
        }
        float angle = Mathf.SmoothDamp(transform.localEulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        currAngle = transform.localEulerAngles.y;
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
            float oppositeDirection = initialDriftDirectionRight == true ? -1 : 1;
            sphereCollider.AddForce(transform.transform.right * currSpeed * oppositeDirection * driftSideBoostMultiplier, ForceMode.Acceleration);
            sphereCollider.AddForce(transform.transform.forward * currSpeed * currentBoostMultiplier, ForceMode.Acceleration);
        }
        else
        {
            sphereCollider.AddForce(transform.transform.forward * currSpeed * currentBoostMultiplier, ForceMode.Acceleration);
        }
    }
}
