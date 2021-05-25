using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputHandler))]
public class SphereCarController : MonoBehaviour
{
    private CarInputHandler carInputHandler;
    private CarStats carStats;
    public AbilityCollision abilityCollision;

    public Transform model;

    private float horizontal;
    private float vertical;

    private float turnVelocity;

    [Header("DEBUG")]
    [SerializeField]
    private bool initialDriftDirectionRight;
    [SerializeField]
    private float targetMinusCurrAngle;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();
    }

    private void Update()
    {

        horizontal = Input.GetAxisRaw(carInputHandler.HorizontalInput);
        vertical = Input.GetButton(carInputHandler.AccelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(carInputHandler.BrakeInput) ? 0.5f : 0;

        HandleAnimation();


    }

    void FixedUpdate()
    {
        if (!carStats.InAir)
        {
            if (!abilityCollision.stunned)
                HandleMovement();

            HandleSteering();
        }

    }

    void HandleAnimation()
    {
        model.transform.position = transform.position - new Vector3(0, 1.5f, 0);
        //Raycast down - angle model based on normal of floor
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 4f))
        {
            carStats.InAir = false;

            Vector3 newUp = hit.normal;
            Vector3 oldForward = transform.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);

            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(newForward, newUp), Time.deltaTime * 8f);

            model.localEulerAngles = new Vector3(model.localEulerAngles.x, model.localEulerAngles.y, horizontal * carStats.CurrSpeed * 0.1f);

            switch (hit.collider.tag)
            {
                case "Offroad":
                    carStats.Surface = 2;
                    break;
                case "Road":
                    carStats.Surface = 1;
                    break;
                default:
                    carStats.Surface = 1;
                    break;
            }
        }
        else
        {
            carStats.InAir = true;
        }
    }

    void HandleSteering()
    {
        targetMinusCurrAngle = carStats.CurrAngle - carStats.TargetAngle;
        if (carStats.CurrSpeed <= 0.1f && carStats.CurrSpeed >= -.1f)
        {
            return;
        }

        if (Input.GetButton(carInputHandler.DriftInput) && horizontal != 0 && carStats.Acceleration / carStats.CurrSpeed >= carStats.DriftSpeedThresholdPercent)
        {
            if (!carStats.IsDrifting)
            {
                initialDriftDirectionRight = horizontal > 0;
                carStats.SphereCollider.AddForce(transform.transform.right * carStats.CurrSpeed * -horizontal * 0.5f, ForceMode.Acceleration);
            }
            carStats.IsDrifting = true;
        }
        else
        {
            carStats.IsDrifting = false;
        }

        if (carStats.IsDrifting)
        {
            float bonusDriftAngle = initialDriftDirectionRight == true ? carStats.DriftTurnAngle : -carStats.DriftTurnAngle;
            carStats.TargetAngle = carStats.CurrAngle + (((horizontal * carStats.DriftTurnAngle) + bonusDriftAngle) / 2);
        }
        else
        {
            carStats.TargetAngle = carStats.CurrAngle + (horizontal * carStats.NormalTurnAngle);
        }
        float angle = Mathf.SmoothDamp(transform.localEulerAngles.y, carStats.TargetAngle, ref turnVelocity, carStats.TurnSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        carStats.CurrAngle = transform.localEulerAngles.y;
    }

    void HandleMovement()
    {
        switch (carStats.Surface)
        {
            case 1:
                carStats.CurrentSurfaceMultiplier = 1f;
                break;
            case 2:
                carStats.CurrentSurfaceMultiplier = carStats.OffroadMultiplier;
                break;
            default:
                carStats.CurrentSurfaceMultiplier = 1f;
                break;
        }

        if (carStats.IsDrifting)
        {
            carStats.MaxSpeed = vertical * carStats.DriftingAcceleration * carStats.CurrentBoostMultiplier * carStats.CurrentSurfaceMultiplier;
        }
        else
        {
            carStats.MaxSpeed = vertical * carStats.Acceleration * carStats.CurrentBoostMultiplier * carStats.CurrentSurfaceMultiplier;
        }

        carStats.CurrSpeed = Mathf.SmoothStep(carStats.CurrSpeed, carStats.MaxSpeed, Time.deltaTime * 12f);

        //Forward Acceleration
        if (carStats.IsDrifting)
        {
            float oppositeDirection = initialDriftDirectionRight == true ? -1 : 1;
            carStats.SphereCollider.AddForce(transform.right * carStats.CurrSpeed * oppositeDirection * carStats.DriftSideBoostMultiplier, ForceMode.Acceleration);
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed * carStats.CurrentBoostMultiplier, ForceMode.Acceleration);
        }
        else
        {
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed, ForceMode.Acceleration);
        }
    }
}
