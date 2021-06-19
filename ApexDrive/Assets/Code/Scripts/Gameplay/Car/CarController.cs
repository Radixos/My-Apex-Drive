// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private CarInputHandler carInputHandler;
    private CarStats carStats;
    public TrailRenderer[] trails;

    public Transform model;

    private float turnVelocity;

    [Header("DEBUG")] // Required for the car! Don't delete!
    [SerializeField]
    private float horizontal;
    [SerializeField]
    private float vertical;
    [SerializeField]
    private bool initialDriftDirectionRight;
    [SerializeField]
    private float sideBoostRamp;
    [SerializeField]
    private RaycastHit hit;

    //FMOD events
    FMOD.Studio.EventInstance sfxEngine;
    FMOD.Studio.EventInstance sfxDrift;

    FMOD.Studio.EventDescription sfxControl;
    FMOD.Studio.EventDescription sfxSkid;

    FMOD.Studio.PARAMETER_DESCRIPTION sfxSpeed;
    FMOD.Studio.PARAMETER_DESCRIPTION sfxAcceleration;
    FMOD.Studio.PARAMETER_DESCRIPTION sfxStopDrift;

    FMOD.Studio.PARAMETER_ID sd;
    FMOD.Studio.PARAMETER_ID rpm;
    FMOD.Studio.PARAMETER_ID acc;

    FMOD.Studio.PLAYBACK_STATE state;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();

        //FMOD Instances
        sfxEngine = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/engine");
        sfxDrift = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/Drifting");

        //FMOD Variables
        sfxControl = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/engine");
        sfxControl.getParameterDescriptionByName("RPM", out sfxSpeed);
        rpm = sfxSpeed.id;

        sfxControl.getParameterDescriptionByName("Acceleration", out sfxAcceleration);
        acc = sfxAcceleration.id;

        sfxSkid = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/Drifting");
        sfxSkid.getParameterDescriptionByName("Stop Drift", out sfxStopDrift);
        sd = sfxStopDrift.id;

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sfxEngine, transform, carStats.SphereCollider);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sfxDrift, transform, GetComponent<Rigidbody>());

        sfxEngine.start();
    }

    private void Update()
    {
        HandleAnalogueInput();
        HandleAnimation();
        HandleCarState();
        HandleCarAudio();

        //CalculateSpeedAndGear(); DEPRECATED
    }

    void FixedUpdate()
    {
        if (carStats.CanDrive && carStats.StunDuration <= 0)
        {
            if (!carStats.InAir)
            {
                HandleMovement();
                HandleSteering();
            } else
            {
                carStats.IsDrifting = false;
            }
        }
    }

    /// <summary>
    /// HandleAnalogueInput takes care of the vertical and horizontal inputs of the player.
    /// This might be better in CarStats.cs
    /// </summary>
    void HandleAnalogueInput()
    {
        horizontal = Mathf.Abs(Input.GetAxisRaw(carInputHandler.HorizontalInput)) > 0.15f ? Input.GetAxisRaw(carInputHandler.HorizontalInput) : 0;
        vertical = Input.GetButton(carInputHandler.AccelerateInput) ? 1 : 0;
        vertical -= Input.GetButton(carInputHandler.BrakeInput) ? 0.5f : 0;
    }

    /// <summary>
    /// HandleCarState calculates other variables of the car which may be useful in multiple functions,
    /// or are otherwise not fit to go in HandleAnimation(), HandleMovement(), and HandleSteering().
    /// </summary>
    void HandleCarState()
    {
        // Used to calculate horizontal force the car while it is drifting
        if (carStats.IsDrifting && sideBoostRamp < 1f)
        {
            sideBoostRamp += 0.5f * Time.deltaTime;
        }
        else if (sideBoostRamp > 0f)
        {
            sideBoostRamp -= 0.5f * Time.deltaTime;
        }

        // Used to determine if the car is in the air
        carStats.InAir = !Physics.Raycast(transform.position, Vector3.down, out hit, 4f);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);

        // Used to determine what surface the car is on
        if (hit.collider == null) return;
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

        // Reduce stun timer if we are stunned :)
        if (carStats.StunDuration >= 0)
        {
            carStats.StunDuration -= Time.deltaTime;
        }
    }

    /// <summary>
    /// HandleAnimation handles all animations and effecs of the car such as angle of the model.
    /// </summary>
    void HandleAnimation()
    {
        transform.position = carStats.SphereCollider.transform.position - new Vector3(0, .70f, 0);

        if (!carStats.InAir)
        {
            Vector3 newUp = hit.normal;
            Vector3 oldForward = transform.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);

            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(newForward, newUp), Time.deltaTime * 8f);

            model.localEulerAngles = new Vector3(model.localEulerAngles.x, model.localEulerAngles.y, horizontal * carStats.CurrSpeed * 0.1f);
        }

        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = carStats.IsDrifting;
        }
    }

    /// <summary>
    /// HandleSteering handles all turning related controls of the car, including determining whether the car is drifting or not.
    /// This function does not move the car while turning, rather just steers the car/directs it.
    /// </summary>
    void HandleSteering()
    {
        if (carStats.CurrSpeed <= 0.1f && carStats.CurrSpeed >= -.1f) return;

        if (Input.GetButton(carInputHandler.DriftInput) && carStats.CurrSpeed / carStats.Acceleration >= carStats.DriftSpeedThresholdPercent)
        {
            if (!carStats.IsDrifting)
            {
                initialDriftDirectionRight = horizontal > 0;
                if ((horizontal < 0) != initialDriftDirectionRight)
                {
                    sideBoostRamp = 0;
                }
                //carStats.SphereCollider.AddForce(transform.transform.right * carStats.CurrSpeed * -horizontal * 0.5f, ForceMode.Acceleration);
            }
            carStats.IsDrifting = true;
        }
        else
        {
            carStats.IsDrifting = false;
        }

        if (carStats.IsDrifting)
        {
            float bonusDriftAngle = initialDriftDirectionRight == true ? carStats.NormalTurnAngle : -carStats.NormalTurnAngle;
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

    /// <summary>
    /// HandleMovement handles all movement functions of the car such as acceleration, decceleration, turning momentum, and boosting.
    /// </summary>
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

        // Removed, might add it back - if(isDrifting) carStats.MaxSpeed = vertical * carStats.DriftingAcceleration * carStats.CurrentBoostMultiplier * carStats.CurrentSurfaceMultiplier, else
        carStats.MaxSpeed = vertical * carStats.Acceleration * carStats.CurrentBoostMultiplier * carStats.CurrentSurfaceMultiplier;

        // Slowly accelerate/decelerate.
        carStats.CurrSpeed = Mathf.SmoothStep(carStats.CurrSpeed, carStats.MaxSpeed, Time.deltaTime * 9f);

        // Used to add force in the opposite direction while turning.
        float oppositeDirection = initialDriftDirectionRight ? -1 : 1;

        // Forward Acceleration + Side Acceleration if drifting
        if (carStats.IsDrifting)
        {
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed * carStats.CurrentBoostMultiplier, ForceMode.Acceleration);
        }
        else
        {
            carStats.SphereCollider.AddForce(transform.forward * carStats.CurrSpeed * carStats.CurrentBoostMultiplier * (1-sideBoostRamp), ForceMode.Acceleration);
            sfxEngine.setParameterByID(acc, 1f);
        }

        carStats.SphereCollider.AddForce(transform.right * carStats.CurrSpeed * oppositeDirection * sideBoostRamp, ForceMode.Acceleration);
    }

    /// <summary>
    /// HandlecarAudio handles all sounds of the car.
    /// </summary>
    void HandleCarAudio()
    {
        if (!Input.GetButton(carInputHandler.AccelerateInput))
        {
            sfxEngine.setParameterByID(acc, 0f);
        }

        if (carStats.InAir)
        {
            sfxEngine.setParameterByID(rpm, 60f);
        }
        else
        {
            sfxEngine.setParameterByID(rpm, carStats.CurrSpeed);
        }
        if (!Input.GetButton(carInputHandler.AccelerateInput))
        {
            sfxEngine.setParameterByID(acc, 0f);
        }
        if(carStats.IsDrifting)
        {
            sfxEngine.setParameterByID(acc, 1f);
            sfxDrift.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED || state == FMOD.Studio.PLAYBACK_STATE.STOPPING)
            {
                sfxDrift.setParameterByID(sd, 0f);
                sfxDrift.start();
            }
        }
        else if(!carStats.IsDrifting)
        {
            sfxDrift.setParameterByID(sd, 1f);
            sfxDrift.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    /// <summary>
    /// Call when need an impact! The mass of cars is 10, so take that into account when applying force.
    /// </summary>
    /// <param name="force"></param>
    /// <param name="direction"></param>
    /// <param name="stunDuration"></param>
    public void Impact(int force, Vector3 direction, float stunDuration = 0)
    {
        carStats.SphereCollider.AddForce(direction * force, ForceMode.Impulse);
        carStats.StunDuration = stunDuration;
    }
}
