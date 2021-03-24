using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Car", menuName = "ScriptableObjects/Custom Car", order = 1)]
public class CarAttributes : ScriptableObject
{
    [Header("Drifting Options")]
    public float driftSpeedThresholdPercent = 0.6f;
    public float driftSideBoostMultiplier = 1.0f;

    [Header("Boost Options")]
    public float boostMultiplier = 1.3f;

    [Header("Turning Options")]
    public float normalTurnAngle = 25f;
    public float driftTurnAngle = 60f;

    [Header("Speed Options")]
    public float driftingAcceleration = 70f;
    public float acceleration = 70f;
    public float drag = 2f;
}
