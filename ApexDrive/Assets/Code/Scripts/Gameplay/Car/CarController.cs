// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CarController : CarModule
{
    // Inputs
    [SerializeField] private InputAction m_AccelerateInput;
    [SerializeField] private InputAction m_HorizontalInput;
    [SerializeField] private InputAction m_BreakInput;
    [SerializeField] private InputAction m_DriftInput;
    [SerializeField] private InputAction m_BoostInput;
    [SerializeField] private InputAction m_EmoteInput;
    

    [SerializeField] private Animator m_EmoteAnimator;

    public TrailRenderer[] trails;

    public Transform model;

    private float turnVelocity;

    private float horizontal;
    private float acceleration;
    private bool initialDriftDirectionRight;
    private float sideBoostRamp;
    private RaycastHit hit;


    [SerializeField, FMODUnity.EventRef] private string m_BoostSFXPath, m_EngineSFXPath, m_DriftSFXPath, m_ImpactSFXPath, m_HornSFXPath, m_DespawnSFXPath;
    private FMOD.Studio.EventInstance sfxImpact;
    private FMOD.Studio.EventInstance sfxHorn;

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

    // VFX
    [SerializeField] private ParticleSystem m_BoostVFX;
    [SerializeField] private ParticleSystem m_DeathVFX;
    [SerializeField] private ParticleSystem m_VictoryVFX;



    private float impactForce;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {

        //FMOD Instances
        sfxEngine = FMODUnity.RuntimeManager.CreateInstance(m_EngineSFXPath);
        sfxDrift = FMODUnity.RuntimeManager.CreateInstance(m_DriftSFXPath);
        sfxImpact = RuntimeManager.CreateInstance(m_ImpactSFXPath);
        sfxHorn = RuntimeManager.CreateInstance(m_HornSFXPath);

        //FMOD Variables - This is dumb. Remind me never to do it like this again
        sfxControl = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/engine");
        sfxControl.getParameterDescriptionByName("RPM", out sfxSpeed);
        rpm = sfxSpeed.id;

        sfxControl.getParameterDescriptionByName("Acceleration", out sfxAcceleration);
        acc = sfxAcceleration.id;

        sfxSkid = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/Drifting");
        sfxSkid.getParameterDescriptionByName("Stop Drift", out sfxStopDrift);
        sd = sfxStopDrift.id;

        //FMOD Attaching thingy
        RuntimeManager.AttachInstanceToGameObject(sfxEngine, transform, Rigidbody);
        RuntimeManager.AttachInstanceToGameObject(sfxHorn, transform, Rigidbody);

        sfxHorn.setParameterByName("Player", Player.PlayerReadableID);

        sfxEngine.start();
        sfxHorn.start();
    }

    private void Update()
    {
        HandleAnalogueInput();
        HandleAnimation();
        HandleCarState();
        HandleCarAudio();

        if(Stats.CanDrive) Stats.PowerAmount += 0.05f * Time.deltaTime;

        if(Stats.CanDrive && Stats.CanBoost && InputManager.GetButtonDown(Player.ControllerType, m_BoostInput, Player.ControllerID) && Stats.PowerAmount >= Stats.BoostCost)
        {
            StartCoroutine(Co_Boost());
        }
        Stats.PowerAmount = Mathf.Clamp01(Stats.PowerAmount);

        if(InputManager.GetButtonDown(Player.ControllerType, m_EmoteInput, Player.ControllerID)) StartCoroutine(Co_Emote());
        //CalculateSpeedAndGear(); DEPRECATED
        Stats.PowerAmount = Mathf.Clamp01(Stats.PowerAmount);
    }

    public void PlayVictoryVFX()
    {
        RuntimeManager.PlayOneShotAttached(m_DespawnSFXPath, gameObject);
        Destroy(GameObject.Instantiate(m_VictoryVFX, transform.position + Vector3.up, Quaternion.identity), 2.5f);
    }

    private void FixedUpdate()
    {
        if (Stats.CanDrive)
        {
            if (!Stats.InAir)
            {
                HandleMovement();
                HandleSteering();
            } else
            {
                Stats.IsDrifting = false;
            }
        }
    }

    /// <summary>
    /// HandleAnalogueInput takes care of the acceleration and horizontal inputs of the player.
    /// This might be better in Stats.cs
    /// </summary>
    void HandleAnalogueInput()
    {
        horizontal = InputManager.GetAxis(Player.ControllerType, m_HorizontalInput, Player.ControllerID);
        acceleration = InputManager.GetAxis(Player.ControllerType, m_AccelerateInput, Player.ControllerID) > 0.5f ? 1 : 0;
        acceleration -= InputManager.GetButton(Player.ControllerType, m_BreakInput, Player.ControllerID) ? 0.5f : 0;
    }

    /// <summary>
    /// HandleCarState calculates other variables of the car which may be useful in multiple functions,
    /// or are otherwise not fit to go in HandleAnimation(), HandleMovement(), and HandleSteering().
    /// </summary>
    void HandleCarState()
    {
        // Used to calculate horizontal force the car while it is drifting
        if (Stats.IsDrifting && sideBoostRamp < 1f)
        {
            sideBoostRamp += 0.5f * Time.deltaTime;
        }
        else if (sideBoostRamp > 0f)
        {
            sideBoostRamp -= 0.5f * Time.deltaTime;
        }

        // Used to determine if the car is in the air
        Stats.InAir = !Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 1.5f);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 1.5f, Color.red);

        // Used to determine what surface the car is on
        if (hit.collider == null) return;
        switch (hit.collider.tag)
        {
            case "Offroad":
                Stats.Surface = 2;
                break;
            case "Road":
                Stats.Surface = 1;
                break;
            default:
                Stats.Surface = 1;
                break;
        }
    }

    /// <summary>
    /// HandleAnimation handles all animations and effecs of the car such as angle of the model.
    /// </summary>
    void HandleAnimation()
    {
        if (!Stats.InAir)
        {
            Vector3 newUp = hit.normal;
            Vector3 oldForward = transform.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);

            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(newForward, newUp), Time.deltaTime * 8f);

            model.localEulerAngles = new Vector3(model.localEulerAngles.x, model.localEulerAngles.y, horizontal * Stats.CurrSpeed * 0.1f);
        }

        // foreach (TrailRenderer trail in trails)
        // {
        //     trail.emitting = Stats.IsDrifting;
        // }
    }

    /// <summary>
    /// HandleSteering handles all turning related controls of the car, including determining whether the car is drifting or not.
    /// This function does not move the car while turning, rather just steers the car/directs it.
    /// </summary>
    void HandleSteering()
    {
        if (Stats.CurrSpeed <= 0.1f && Stats.CurrSpeed >= -.1f) return;

        if (InputManager.GetAxis(Player.ControllerType, m_DriftInput, Player.ControllerID) > 0.0f && Stats.CurrSpeed / Stats.Acceleration >= Stats.DriftSpeedThresholdPercent)
        {
            if (!Stats.IsDrifting)
            {
                initialDriftDirectionRight = horizontal > 0;
                if ((horizontal < 0) != initialDriftDirectionRight)
                {
                    sideBoostRamp = 0;
                }
            }
            Stats.IsDrifting = true;
        }
        else
        {
            Stats.IsDrifting = false;
        }

        if (Stats.IsDrifting)
        {
            float bonusDriftAngle = initialDriftDirectionRight == true ? Stats.NormalTurnAngle : -Stats.NormalTurnAngle;
            Stats.TargetAngle = Stats.CurrAngle + (((horizontal * Stats.DriftTurnAngle) + bonusDriftAngle) / 2);
        }
        else
        {
            Stats.TargetAngle = Stats.CurrAngle + (horizontal * Stats.NormalTurnAngle);
        }
        float angle = Mathf.SmoothDamp(transform.localEulerAngles.y, Stats.TargetAngle, ref turnVelocity, Stats.TurnSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        Stats.CurrAngle = transform.localEulerAngles.y;
    }

    /// <summary>
    /// HandleMovement handles all movement functions of the car such as acceleration, decceleration, turning momentum, and boosting.
    /// </summary>
    void HandleMovement()
    {
        switch (Stats.Surface)
        {
            case 1:
                Stats.CurrentSurfaceMultiplier = 1f;
                break;
            case 2:
                Stats.CurrentSurfaceMultiplier = Stats.OffroadMultiplier;
                break;
            default:
                Stats.CurrentSurfaceMultiplier = 1f;
                break;
        }

        // Removed, might add it back - if(isDrifting) Stats.MaxSpeed = acceleration * Stats.DriftingAcceleration * Stats.CurrentBoostMultiplier * Stats.CurrentSurfaceMultiplier, else
        Stats.MaxSpeed = acceleration * Stats.Acceleration * Stats.CurrentSurfaceMultiplier;

        // Slowly accelerate/decelerate.
        Stats.CurrSpeed = Mathf.SmoothStep(Stats.CurrSpeed, Stats.MaxSpeed, Time.deltaTime * 9f);

        // Used to add force in the opposite direction while turning.
        float oppositeDirection = initialDriftDirectionRight ? -1 : 1;

        // Forward Acceleration + Side Acceleration if drifting
        if (Stats.IsDrifting)
        {
            this.Rigidbody.AddForce(transform.forward * Stats.CurrSpeed , ForceMode.Acceleration);
        }
        else
        {
            this.Rigidbody.AddForce(transform.forward * Stats.CurrSpeed * (1-sideBoostRamp), ForceMode.Acceleration);
            sfxEngine.setParameterByID(acc, 1f);
        }

        this.Rigidbody.AddForce(transform.right * Stats.CurrSpeed * oppositeDirection * sideBoostRamp, ForceMode.Acceleration);
    }

    /// <summary>
    /// HandlecarAudio handles all sounds of the car.
    /// </summary>
    void HandleCarAudio()
    {
        if (!InputManager.GetButton(Player.ControllerType, m_AccelerateInput, Player.ControllerID))
        {
            sfxEngine.setParameterByID(acc, 0f);
        }

        if (Stats.InAir)
        {
            sfxEngine.setParameterByID(rpm, 60f);
        }
        else
        {
            sfxEngine.setParameterByID(rpm, Stats.CurrSpeed);
        }
        if (!InputManager.GetButton(Player.ControllerType, m_AccelerateInput, Player.ControllerID))
        {
            sfxEngine.setParameterByID(acc, 0f);
        }
        if(Stats.IsDrifting)
        {
            RuntimeManager.AttachInstanceToGameObject(sfxDrift, transform, Rigidbody);
            sfxEngine.setParameterByID(acc, 1f);
            sfxDrift.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED || state == FMOD.Studio.PLAYBACK_STATE.STOPPING)
            {
                sfxDrift.setParameterByID(sd, 0f);
                sfxDrift.start();
            }
        }
        else if(!Stats.IsDrifting)
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
        this.Rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        RuntimeManager.AttachInstanceToGameObject(sfxImpact, transform, Rigidbody);
        impactForce = collision.relativeVelocity.magnitude;
        if (collision.rigidbody != null)
        {
            sfxImpact.setParameterByName("Force", impactForce);
            sfxImpact.start();
        }
    }

    private void OnDisable()
    {
        sfxEngine.release();
        sfxDrift.release();
        sfxImpact.release();
        sfxHorn.release();
    }

    public void Eliminate()
    {
        if(m_DeathVFX != null) Destroy(GameObject.Instantiate(m_DeathVFX, transform.position, Quaternion.identity, null), 5.0f);
        if(CameraManager.Instance != null) CameraManager.Instance.AddShake(4.0f, 0.25f, 0.25f, 0.25f);
        Player.Laps = 0;
        Player.TrackProgress = 0;
    }


    public IEnumerator Co_Boost()
    {
        FMODUnity.RuntimeManager.PlayOneShot(m_BoostSFXPath, transform.position);
        Stats.CanBoost = false;
        Stats.CurrSpeed = Stats.MaxSpeed + Stats.BoostStrength;
        if(m_BoostVFX != null) m_BoostVFX.Play();
        Debug.Log(Stats.PowerAmount + "-" + Stats.BoostCost + (Stats.PowerAmount - Stats.BoostCost));
        Stats.PowerAmount -= Stats.BoostCost;

        yield return new WaitForSeconds(Stats.BoostCooldown);

        Stats.CanBoost = true;
    }

    private IEnumerator Co_Emote()
    {
        m_EmoteAnimator.SetBool("Visible", true);
        bool buttonRelease = false;

        sfxHorn.start();

        while(!buttonRelease)
        {
            Debug.Log(InputManager.GetButtonUp(Player.ControllerType, m_EmoteInput, Player.ControllerID));
            yield return null;
            if(InputManager.GetButtonUp(Player.ControllerType, m_EmoteInput, Player.ControllerID)) buttonRelease = true;
        }
    }
}
