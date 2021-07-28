// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CarStats : CarModule
{
    [Header("Car Components")]
    [SerializeField]
    private CarAttributes carAttributes;
    [SerializeField]
    private GameObject shield;
    [SerializeField]
    private GameObject rampage;

    [Header("Car States")]
    [SerializeField]
    private bool inAir;
    [SerializeField]
    [Tooltip("0 = In Air, 1 = Road, 2 = Offroad")]
    private int surface;
    [SerializeField]
    private bool canDrive;
    [SerializeField]
    private float stunDuration;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    private float driftSpeedThresholdPercent;
    [SerializeField]
    private float driftSideBoostMultiplier;

    [Header("Boost Options")]
    [SerializeField]
    private float currentBoostMultiplier = 1f;
    [SerializeField]
    private float boostMultiplier;

    [Header("Turning Options")]
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float currAngle;
    [SerializeField]
    private float targetAngle;
    [SerializeField]
    private float normalTurnAngle;
    [SerializeField]
    private float driftTurnAngle;

    [Header("Speed Options")]
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float driftingAcceleration;
    [SerializeField]
    private float currentSurfaceMultiplier = 1f;
    [SerializeField]
    private float offroadMultiplier;
    [SerializeField]
    private float currSpeed;
    [SerializeField]
    private float maxSpeed;

    [Header("Ability Options")]
    [SerializeField]
    [Range(0, 1)]
    private float powerAmount;
    [SerializeField]
    private bool initialShieldPowerDepleted;
    [SerializeField]
    private float rampageLifetime;
    [SerializeField]
    private float rampageTimer;

    [Header("FMOD Global parameters")]
    [SerializeField]
    [ParamRef]
    private string powerMeter;

    // Getters and Setters :)
    // Car States
    public bool InAir { get => inAir; set => inAir = value; }
    public bool IsDrifting { get => isDrifting; set => isDrifting = value; }
    public int Surface { get => surface; set => surface = value; }
    public bool CanDrive { get => canDrive; set => canDrive = value; }
    public float StunDuration { get => stunDuration; set => stunDuration = value; }

    // Car Stats
    public float DriftSpeedThresholdPercent { get => driftSpeedThresholdPercent; set => driftSpeedThresholdPercent = value; }
    // public float DriftSideBoostMultiplier { get => driftSideBoostMultiplier; set => driftSideBoostMultiplier = value; }
    // public float CurrentBoostMultiplier { get => currentBoostMultiplier; set => currentBoostMultiplier = value; }
    // public float BoostMultiplier { get => boostMultiplier; set => boostMultiplier = value; }
    public float TurnSpeed { get => turnSpeed; set => turnSpeed = value; }
    public float CurrAngle { get => currAngle; set => currAngle = value; }
    public float TargetAngle { get => targetAngle; set => targetAngle = value; }
    public float NormalTurnAngle { get => normalTurnAngle; set => normalTurnAngle = value; }
    public float DriftTurnAngle { get => driftTurnAngle; set => driftTurnAngle = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float DriftingAcceleration { get => driftingAcceleration; set => driftingAcceleration = value; }
    public float CurrSpeed { get => currSpeed; set => currSpeed = value; }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public float OffroadMultiplier { get => offroadMultiplier; set => offroadMultiplier = value; }
    public float CurrentSurfaceMultiplier { get => currentSurfaceMultiplier; set => currentSurfaceMultiplier = value; }

    // Car Components
    public CarAttributes CarAttributes { get => carAttributes; set => carAttributes = value; }
    public GameObject Shield { get => shield; set => shield = value; }
    public GameObject Rampage { get => rampage; set => rampage = value; }

    // Ability Options
    public float PowerAmount { get => powerAmount; set => powerAmount = value; }
    public bool InitialShieldPowerDepleted { get => initialShieldPowerDepleted; set => initialShieldPowerDepleted = value; }
    public float RampageLifetime { get => rampageLifetime; set => rampageLifetime = value; }
    public float RampageTimer { get => rampageTimer; set => rampageTimer = value; }

    void Start()
    {
        // Assign car attributes
        DriftSpeedThresholdPercent = CarAttributes.driftSpeedThresholdPercent;
        // DriftSideBoostMultiplier = CarAttributes.driftSideBoostMultiplier;
        // BoostMultiplier = CarAttributes.boostMultiplier;

        TurnSpeed = CarAttributes.turnSpeed;

        NormalTurnAngle = CarAttributes.normalTurnAngle;
        DriftTurnAngle = CarAttributes.driftTurnAngle;

        DriftingAcceleration = CarAttributes.driftingAcceleration;
        Acceleration = CarAttributes.acceleration;

        OffroadMultiplier = CarAttributes.offroadMultiplier;

        // Abilities initialisation
        PowerAmount = 0.0f; // TEMPORARY

        InitialShieldPowerDepleted = false;

        RampageLifetime = 4.0f;
        RampageTimer = RampageLifetime;
    }

    private void Update()
    {
        if (!inAir)
        {
            Rigidbody.drag = CarAttributes.drag;
        }
        else
        {
            Rigidbody.drag = 0.05f;
        }

        RuntimeManager.StudioSystem.setParameterByName(powerMeter, powerAmount);
    }
}
