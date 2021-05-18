
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{
    public CarAttributes carAttributes;
    [SerializeField]
    private Rigidbody sphereCollider;

    [SerializeField]
    private bool inAir;

    [Header("Drifting Options")]
    [SerializeField]
    private bool isDrifting;
    [SerializeField]
    private float driftSpeedThresholdPercent;
    [SerializeField]
    private float driftSideBoostMultiplier;

    [Header("Boost Options")]
    [SerializeField]
    private float currentBoostMultiplier;
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
    private float currSpeed;
    [SerializeField]
    private float maxSpeed;

    public bool InAir { get => inAir; set => inAir = value; }
    public bool IsDrifting { get => isDrifting; set => isDrifting = value; }
    public float DriftSpeedThresholdPercent { get => driftSpeedThresholdPercent; set => driftSpeedThresholdPercent = value; }
    public float DriftSideBoostMultiplier { get => driftSideBoostMultiplier; set => driftSideBoostMultiplier = value; }
    public float CurrentBoostMultiplier { get => currentBoostMultiplier; set => currentBoostMultiplier = value; }
    public float BoostMultiplier { get => boostMultiplier; set => boostMultiplier = value; }
    public float TurnSpeed { get => turnSpeed; set => turnSpeed = value; }
    public float CurrAngle { get => currAngle; set => currAngle = value; }
    public float TargetAngle { get => targetAngle; set => targetAngle = value; }
    public float NormalTurnAngle { get => normalTurnAngle; set => normalTurnAngle = value; }
    public float DriftTurnAngle { get => driftTurnAngle; set => driftTurnAngle = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float DriftingAcceleration { get => driftingAcceleration; set => driftingAcceleration = value; }
    public float CurrSpeed { get => currSpeed; set => currSpeed = value; }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public Rigidbody SphereCollider { get => sphereCollider; set => sphereCollider = value; }

    void Start()
    {
        //Assign car attributes
        DriftSpeedThresholdPercent = carAttributes.driftSpeedThresholdPercent;
        DriftSideBoostMultiplier = carAttributes.driftSideBoostMultiplier;
        BoostMultiplier = carAttributes.boostMultiplier;

        TurnSpeed = carAttributes.turnSpeed;

        NormalTurnAngle = carAttributes.normalTurnAngle;
        DriftTurnAngle = carAttributes.driftTurnAngle;

        DriftingAcceleration = carAttributes.driftingAcceleration;
        Acceleration = carAttributes.acceleration;
    }

    private void Update()
    {
        //Follow Collider
        transform.position = SphereCollider.position - new Vector3(0, -0.5f, 0);

        if (!inAir)
        {
            SphereCollider.drag = carAttributes.drag;
        }
        else
        {
            SphereCollider.drag = 0.05f;
        }
    }
}
