using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputHandler))]
public class SphereCarController : MonoBehaviour
{
    public CarInputHandler carInputHandler;
    public Transform model;

    private float horizontal;
    private float vertical;

    private float currentBoostMultiplier;

    [Header("DEBUG")]
    [SerializeField]
    private bool initialDriftDirectionRight;
    [SerializeField]
    private float targetMinusCurrAngle;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();

    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw(carInputHandler.HorizontalInput);
        vertical = Input.GetButton(carInputHandler.AccelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(carInputHandler.BrakeInput) ? 0.5f : 0;
        currentBoostMultiplier = Input.GetButton(carInputHandler.BoostInput) ? boostMultipliezr : 1;

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

        if (Input.GetButton(carInputHandler.DriftInput) && horizontal != 0 && acceleration / currSpeed >= driftSpeedThresholdPercent)
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
