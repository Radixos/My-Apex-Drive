using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    public Rigidbody sphereCollider;
    public Transform carModel;

    private float horizontal;
    private float vertical;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("The higher this is, the faster the car will need to be going before you can initiate a drift")]
    private float driftSpeedThresholdPercent;

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

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

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

        if(Input.GetKey(KeyCode.LeftShift) && horizontal != 0 && acceleration / currSpeed >= driftSpeedThresholdPercent)
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
            maxSpeed = vertical * acceleration;
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
            sphereCollider.AddForce(carModel.transform.forward * currSpeed, ForceMode.Acceleration);
        }

        ////Lower speed while turning and not drifting
        //if (!isDrifting && horizontal != 0)
        //{
        //    sphereCollider.AddForce(-carModel.forward * currSpeed * friction);
        //}

        //Reduce Speed
        //sphereCollider.AddForce(-carModel.forward * currSpeed * friction);
    }
}
