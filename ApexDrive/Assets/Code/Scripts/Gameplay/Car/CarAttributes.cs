// Jason Lui

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Car", menuName = "ScriptableObjects/Custom Car", order = 1)]
public class CarAttributes : ScriptableObject
{
    [Header("Drifting Options")]
    [Tooltip("The higher this is, the faster % of max speed the car will need to be going before you can initiate a drift (does basically nothing)")]
    public float driftSpeedThresholdPercent = 0.6f;
    [Tooltip("The higher this is, the faster/wider the car will swing when drifting")]
    public float driftSideBoostMultiplier = 0.5f;

    [Header("Boost Options")]
    [Tooltip("The higher this is, the faster car will go while boosting")]
    public float boostMultiplier = 1.3f;

    [Header("Turning Options")]
    [Tooltip("Lowering this effectively increases the turn angle")]
    public float turnSpeed = 0.6f;
    [Tooltip("This determines the car's turning speed while not boosting")]
    public float normalTurnAngle = 40f;
    [Tooltip("This determines the car's turning speed while boosting")]
    public float driftTurnAngle = 60f;

    [Header("Speed Options")]
    [Tooltip("This determines the car's acceleration while not boosting")]
    public float acceleration = 40f;
    [Tooltip("This determines the car's acceleration while boosting")]
    public float driftingAcceleration = 40f;
    [Tooltip("This determines the car's acceleration multiplier while offroad")]
    public float offroadMultiplier = 0.4f;
    [Tooltip("This determines the car's drag")]
    public float drag = 2f;
}
