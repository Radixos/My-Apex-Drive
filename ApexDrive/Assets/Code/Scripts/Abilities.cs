using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputHandler))]
[RequireComponent(typeof(CarStats))]
[RequireComponent(typeof(SphereCarController))]
public class Abilities : MonoBehaviour
{
    private CarInputHandler carInputHandler;
    private CarStats carStats;

    // MANI'S CODE
    [Header("Abilities Options")]
    [SerializeField]
    [Range(0, 1)]
    private float powerAmount;

    public GameObject shield;
    [SerializeField]
    private float shieldEffectLifetime;
    [SerializeField]
    public float shieldEffectTimer;

    public GameObject rampage;
    [SerializeField]
    private float rampageLifetime;
    [SerializeField]
    private float rampageTimer;

    [SerializeField]
    // Applies to both rampage and speed boost
    private float speedMultiplier; 
    [SerializeField]
    private float speedBoostLifeTime;
    [SerializeField]
    private float speedBoostTimer;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();

        //Abilities Initialisation
        powerAmount = 1.0f; // TEMPORARY

        shieldEffectLifetime = 0.3f;
        shieldEffectTimer = shieldEffectLifetime;

        rampageLifetime = 3.0f;
        rampageTimer = rampageLifetime;

        speedBoostLifeTime = 3.0f;
        speedBoostTimer = speedBoostLifeTime;

        speedMultiplier = 1.0f;

    }

    private void Update()
    {
        AbilityLogic();
    }

    void AbilityLogic()
    {
        // Shield only will stay active as
        // long as the button is pressed
        shield.SetActive(false);

        if (shieldEffectTimer < shieldEffectLifetime)
            shieldEffectTimer += Time.deltaTime;

        // Active time of rampage
        if (rampage.activeSelf)
        {
            if (rampageTimer >= rampageLifetime)
                rampage.SetActive(false);
            else
                rampageTimer += Time.deltaTime;
        }

        if (powerAmount > 0)
        {
            // Activate one ability at a times
            // Shield power up
            if (Input.GetButton(carInputHandler.PowerAInput) &&
                rampage.activeSelf == false &&
                speedMultiplier == 1.0f &&
                powerAmount >= 0.3f
                )
            {
                if (shield.activeSelf == false)
                {
                    powerAmount -= 0.3f;
                }
                shield.SetActive(true);
                powerAmount -= Time.deltaTime * 0.5f;
            }
            // Attack power up
            else if (Input.GetButton(carInputHandler.PowerBInput) &&
                powerAmount >= 0.5f &&
                speedMultiplier == 1.0f &&
                shield.activeSelf == false &&
                rampage.activeSelf == false)
            {
                rampage.SetActive(true);
                rampageTimer = 0.0f;
                powerAmount -= 0.5f;
            }
            // Boost power up
            // Hold or tap?
            else if (Input.GetButton(carInputHandler.BoostInput) &&
                powerAmount >= 0.3f &&
                shield.activeSelf == false &&
                rampage.activeSelf == false)
            {
                carStats.CurrentBoostMultiplier = Input.GetButton(carInputHandler.BoostInput) ? carStats.BoostMultiplier : 1;
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && rampage.activeSelf)
        {
            Vector3 normal = Vector3.zero;
            normal = collision.contacts[0].normal;
        }
    }
}

