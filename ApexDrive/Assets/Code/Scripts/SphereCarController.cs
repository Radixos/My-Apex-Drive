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

    [Header("Acceleration Options")]
    [SerializeField]
    private float maxAcceleration;
    [SerializeField]
    private float currAcceleration;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("The lower this is, the faster the car will go")]
    private float friction;

    [Header("Speed Options")]
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float currSpeed;

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

        if (Input.GetKey(KeyCode.LeftShift) && currSpeed / maxSpeed > driftSpeedThresholdPercent && horizontal != 0)
        {
            isDrifting = true;
        } else
        {
            isDrifting = false;
        }
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
        if(sphereCollider.velocity.magnitude <= 0.1f)
        {
            return;
        }

        float targetAngle = currAngle + (horizontal * maxTurnAngle);
        float angle = Mathf.SmoothDamp(carModel.localEulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        if (isDrifting)
        {
            maxTurnAngle = 60;
        } else
        {
            maxTurnAngle = 25;
        }
        carModel.localEulerAngles = new Vector3(0, angle, 0);
        currAngle = carModel.localEulerAngles.y;
    }

    void HandleMovement()
    {
        currAcceleration = vertical * maxAcceleration;
        currSpeed = sphereCollider.velocity.magnitude;

        //Acceleration only if not too fast
        if (currSpeed < maxSpeed)
        {
            sphereCollider.AddForce(carModel.forward * currAcceleration);
        }

        //Clamp Acceleration
        if(Mathf.Abs(currAcceleration) > maxAcceleration)
        {
            currAcceleration = maxAcceleration;
        }

        //Lower speed while turning and not drifting
        if (!isDrifting && horizontal != 0)
        {
            sphereCollider.AddForce(-carModel.forward * currSpeed * friction);
        }

        //Reduce Speed
        //sphereCollider.AddForce(-carModel.forward * currSpeed * friction);
    }
}
