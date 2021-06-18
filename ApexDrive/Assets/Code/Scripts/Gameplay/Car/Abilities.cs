using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    public CarController sphereCarController;
    private CarInputHandler carInputHandler;
    private CarStats carStats;

    // MANI'S CODE

    //FMOD Stuff
    FMOD.Studio.EventInstance ability;
    FMOD.Studio.EventDescription onOff;

    FMOD.Studio.PARAMETER_DESCRIPTION stop;
    FMOD.Studio.PARAMETER_ID stp;

    FMOD.Studio.PLAYBACK_STATE pbs;

    private void Start()
    {
        carInputHandler = GetComponent<CarInputHandler>();
        carStats = GetComponent<CarStats>();

        ability = FMODUnity.RuntimeManager.CreateInstance("event:/HUD/Abilities/defensive");

        onOff = FMODUnity.RuntimeManager.GetEventDescription("event:/HUD/Abilities/defensive");
        onOff.getParameterDescriptionByName("stop", out stop);
        stp = stop.id;

    }

    private void Update()
    {
        AbilityLogic();
        if (carStats.PowerAmount < 0)
        {
            carStats.PowerAmount = 0;
        }
    }

    /// <summary>
    /// Handles ability inputs
    /// </summary>
    void AbilityLogic()
    {
        // Two abilities that stay active as
        // long as 
        if (!carStats.InitialShieldPowerDepleted)
            carStats.Shield.SetActive(false);

        carStats.CurrentBoostMultiplier = 1;

        if (Input.GetButtonUp(carInputHandler.PowerAInput))
            carStats.InitialShieldPowerDepleted = false;

        // Active time of carStats.Rampage
        if (carStats.Rampage.activeSelf)
        {
            if (carStats.RampageTimer >= carStats.RampageLifetime)
                carStats.Rampage.SetActive(false);
            else
                carStats.RampageTimer += Time.deltaTime;
        }

        if (carStats.PowerAmount > 0)
        {
            // Activate one ability at a times
            // Shield power up
            if (Input.GetButton(carInputHandler.PowerAInput) &&
                carStats.Rampage.activeSelf == false //&& carStats.PowerAmount >= 0.3f
                )
            {
                if (!carStats.InitialShieldPowerDepleted)
                {
                    carStats.PowerAmount -= 0.25f;
                    carStats.InitialShieldPowerDepleted = true;
                    carStats.Shield.SetActive(true);
                }

                ability.getPlaybackState(out pbs);
                if (pbs != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    ability.start();
                }
                carStats.PowerAmount -= Time.deltaTime * 0.5f;
            }
            // Attack power up
            else if (Input.GetButtonDown(carInputHandler.PowerBInput) &&
                carStats.PowerAmount >= 0.5f &&
                carStats.Shield.activeSelf == false &&
                carStats.Rampage.activeSelf == false)
            {
                carStats.Rampage.SetActive(true);
                carStats.RampageTimer = 0.0f;
                carStats.PowerAmount -= 0.5f;
            }
            // Boost power up
            // Hold or tap?
            else if (Input.GetButton(carInputHandler.BoostInput)) //&& carStats.PowerAmount >= 0.3f)
            {
                carStats.CurrentBoostMultiplier = carStats.BoostMultiplier;
                carStats.PowerAmount -= Time.deltaTime * 0.4f;
            }
        }
    }

    /// <summary>
    /// When colliding with a car with an ability active...
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {

        // Only check collision if the car has activated rampage
        if (collision.gameObject.CompareTag("Player") && carStats.Rampage.activeSelf)
        {
            Vector3 normal = collision.contacts[0].normal;

            if (collision.gameObject.GetComponent<CarStats>().Shield.activeSelf)
            {
                sphereCarController.Impact(100, normal, 0.75f);
            }
            else
            {
                collision.gameObject.GetComponent<CarController>().Impact(100, -normal, 0.75f);
            }

        }

    }
}