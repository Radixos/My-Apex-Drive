using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private float sideBoostRampUp;

<<<<<<< Updated upstream:ApexDrive/Assets/Code/Scripts/SphereCarController.cs
=======
    /// <summary>
    /// The Following options only affect the sound. These do not have any impact on gameplay.
    /// 
    /// CURRENTLY DEPRECATED
    /// </summary>
    //[SerializeField]
    //private bool changingGear;
    //[SerializeField]
    //private float gearSpeed;
    //[SerializeField]
    //private float timeSpentInEachGear = 4f;
    //[SerializeField]
    //private int numGears = 3;
    //[SerializeField]
    //private int currGear = 1;
    //[SerializeField]
    //private float durationOfGearChange = 1f;
    //[SerializeField]
    //private float changingGearTimer = 0;

>>>>>>> Stashed changes:ApexDrive/Assets/Code/Scripts/Gameplay/Car/SphereCarController.cs
    //FMOD events
    FMOD.Studio.EventInstance engine;
    FMOD.Studio.EventDescription control;

    FMOD.Studio.PARAMETER_DESCRIPTION speed;
    FMOD.Studio.PARAMETER_DESCRIPTION accelleration;

    FMOD.Studio.PARAMETER_ID spd;
    FMOD.Studio.PARAMETER_ID acc;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();

        engine = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/engine");

        control = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/engine");
        control.getParameterDescriptionByName("RPM", out speed);
        spd = speed.id;

        control = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/engine");
        control.getParameterDescriptionByName("Accelleration", out accelleration);
        acc = accelleration.id;

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine, transform, GetComponent<Rigidbody>());

        engine.start();
    }

    private void Update()
    {

        horizontal = Input.GetAxisRaw(carInputHandler.HorizontalInput);
        vertical = Input.GetButton(carInputHandler.AccelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(carInputHandler.BrakeInput) ? 0.5f : 0;

        if(!Input.GetButton(carInputHandler.AccelerateInput))
        {
            engine.setParameterByID(acc, 0f);
        }

        HandleAnimation();
<<<<<<< Updated upstream:ApexDrive/Assets/Code/Scripts/SphereCarController.cs

=======
        //CalculateSpeedAndGear(); DEPRECATED

        if (carStats.IsDrifting && sideBoostRampUp < 1f)
        {
            sideBoostRampUp += 0.5f * Time.deltaTime;
        } else if (sideBoostRampUp > 0f)
        {
            sideBoostRampUp -= 0.5f * Time.deltaTime;
        }
>>>>>>> Stashed changes:ApexDrive/Assets/Code/Scripts/Gameplay/Car/SphereCarController.cs
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
<<<<<<< Updated upstream:ApexDrive/Assets/Code/Scripts/SphereCarController.cs
        model.transform.position = transform.position - new Vector3(0, 1.5f, 0);
        //Raycast down - angle model based on normal of floor
=======
        model.transform.position = carStats.SphereCollider.transform.position - new Vector3(0, .5f, 0);

        // Raycast down - angle model based on normal of floor
>>>>>>> Stashed changes:ApexDrive/Assets/Code/Scripts/Gameplay/Car/SphereCarController.cs
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

<<<<<<< Updated upstream:ApexDrive/Assets/Code/Scripts/SphereCarController.cs
        carStats.CurrSpeed = Mathf.SmoothStep(carStats.CurrSpeed, carStats.MaxSpeed, Time.deltaTime * 12f);
=======
        // Slowly accelerate/decelerate.
        carStats.CurrSpeed = Mathf.SmoothStep(carStats.CurrSpeed, carStats.MaxSpeed, Time.deltaTime * 9f);
>>>>>>> Stashed changes:ApexDrive/Assets/Code/Scripts/Gameplay/Car/SphereCarController.cs

        //Forward Acceleration
        if (carStats.IsDrifting)
        {
            float oppositeDirection = initialDriftDirectionRight == true ? -1 : 1;
            carStats.SphereCollider.AddForce(transform.right * carStats.CurrSpeed * oppositeDirection * sideBoostRampUp, ForceMode.Acceleration);
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed * carStats.CurrentBoostMultiplier, ForceMode.Acceleration);
            engine.setParameterByID(acc, 1f);
        }
        else
        {
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed, ForceMode.Acceleration);
            engine.setParameterByID(acc, 1f);
        }
        engine.setParameterByID(spd, carStats.CurrSpeed);
    }
<<<<<<< Updated upstream:ApexDrive/Assets/Code/Scripts/SphereCarController.cs
=======

    /// <summary>
    /// Yes, this is a mess. This calculates gears / changing gears.
    /// 
    /// 1. When the accelerate button is pressed, gearSpeed increases
    /// 2. Once gearSpeed reaches a certain threshold (default 4 seconds), the currentGear increases.
    ///  2a. Note that changingGear becomes true for 1 second while this happens.
    /// 3. If the accelerate button is not pressed, the gear speed decreases at the same rate.
    ///  3a. changingGear does not become true at all while decelerating (you don't hear the gear change while decelerating)
    /// 
    /// These numbers are not connected to the actual speed of the car.
    /// 
    /// I found that connecting them was very difficult to keep the arcade like controls of the car.
    /// Let me know if you think this could be changed to be something better.
    /// 
    /// CURRENTLY DEPRECATED
    /// </summary>
    /// 
    //private void CalculateSpeedAndGear()
    //{
    //    if (changingGear)
    //    {
    //        changingGearTimer += 1f * Time.deltaTime;
    //        if (changingGearTimer >= durationOfGearChange)
    //        {
    //            changingGear = false;
    //            changingGearTimer = 0;
    //        }
    //    }
    //    if (vertical > 0)
    //    {
    //        if (currGear < numGears)
    //        {
    //            gearSpeed += 1f * Time.deltaTime;
    //            if (gearSpeed > timeSpentInEachGear)
    //            {
    //                gearSpeed = 0;
    //                currGear++;
    //                changingGear = true;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (currGear > 0)
    //        {
    //            gearSpeed -= 1f * Time.deltaTime;
    //            if (gearSpeed < 0)
    //            {
    //                gearSpeed = timeSpentInEachGear;
    //                currGear--;
    //            }
    //        }
    //    }
    //}
>>>>>>> Stashed changes:ApexDrive/Assets/Code/Scripts/Gameplay/Car/SphereCarController.cs
}
