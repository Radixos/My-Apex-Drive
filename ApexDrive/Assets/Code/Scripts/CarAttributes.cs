using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Car", menuName = "ScriptableObjects/Custom Car", order = 1)]
public class CarAttributes : ScriptableObject
{
    [Header("Drifting Options")]
    public float driftSpeedThresholdPercent;

    [Header("Boost Options")]
    public float boostMultiplier;

    [Header("Turning Options")]
    public float normalTurnAngle;
    public float driftTurnAngle;

    [Header("Speed Options")]
    public float driftingAcceleration;
    public float acceleration;
}
