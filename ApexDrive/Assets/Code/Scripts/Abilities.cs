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
    private bool initialShieldPowerDepleted;

    public GameObject rampage;
    [SerializeField]
    private float rampageLifetime;
    [SerializeField]
    private float rampageTimer;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();

        //Abilities Initialisation
        powerAmount = 1.0f; // TEMPORARY

        initialShieldPowerDepleted = false;

        rampageLifetime = 3.0f;
        rampageTimer = rampageLifetime;

    }

    private void Update()
    {
        AbilityLogic();
    }

    void AbilityLogic()
    {
        // Two abilities that stay active as
        // long as 
        shield.SetActive(false);
        carStats.CurrentBoostMultiplier = 1;

        if (Input.GetButtonUp(carInputHandler.PowerAInput))
            initialShieldPowerDepleted = false;

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
                powerAmount >= 0.3f
                )
            {
                if(!initialShieldPowerDepleted)
                {
                    powerAmount -= 0.25f;
                    initialShieldPowerDepleted = true;
                }

                shield.SetActive(true);
                powerAmount -= Time.deltaTime * 0.5f;
            }
            // Attack power up
            else if (Input.GetButtonDown(carInputHandler.PowerBInput) &&
                powerAmount >= 0.5f &&
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
                powerAmount >= 0.3f)
            {
                carStats.CurrentBoostMultiplier = carStats.BoostMultiplier;
                powerAmount -= Time.deltaTime * 0.4f;
            }
        }
    }
}

