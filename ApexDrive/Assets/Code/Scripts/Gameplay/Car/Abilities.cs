using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : CarModule
{
    // MANI'S CODE

    //FMOD Stuff
    FMOD.Studio.EventInstance ability;
    FMOD.Studio.EventDescription onOff;

    FMOD.Studio.PARAMETER_DESCRIPTION stop;
    FMOD.Studio.PARAMETER_ID stp;

    FMOD.Studio.PLAYBACK_STATE pbs;

    private void Start()
    {
        ability = FMODUnity.RuntimeManager.CreateInstance("event:/HUD/Abilities/defensive");

        onOff = FMODUnity.RuntimeManager.GetEventDescription("event:/HUD/Abilities/defensive");
        onOff.getParameterDescriptionByName("stop", out stop);
        stp = stop.id;

    }

    private void Update()
    {
        AbilityLogic();
        if (Stats.PowerAmount < 0)
        {
            Stats.PowerAmount = 0;
        }
    }

    /// <summary>
    /// Handles ability inputs
    /// </summary>
    void AbilityLogic()
    {
        // Two abilities that stay active as
        // long as 
        if(!Stats.InitialShieldPowerDepleted)
        Stats.Shield.SetActive(false);

        Stats.CurrentBoostMultiplier = 1;

        if (Input.GetButtonUp(PlayerInput.PowerAInput))
            Stats.InitialShieldPowerDepleted = false;

        // Active time of Stats.Rampage
        if (Stats.Rampage.activeSelf)
        {
            if (Stats.RampageTimer >= Stats.RampageLifetime)
                Stats.Rampage.SetActive(false);
            else
                Stats.RampageTimer += Time.deltaTime;
        }

        if (Stats.PowerAmount > 0)
        {
            // Activate one ability at a times
            // Shield power up
            if (Input.GetButton(PlayerInput.PowerAInput) &&
                Stats.Rampage.activeSelf == false //&& Stats.PowerAmount >= 0.3f
                )
            {
                if (!Stats.InitialShieldPowerDepleted)
                {
                    Stats.PowerAmount -= 0.25f;
                    Stats.InitialShieldPowerDepleted = true;
                    Stats.Shield.SetActive(true);
                }

                ability.getPlaybackState(out pbs);
                if (pbs != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    ability.start();
                }
                Stats.PowerAmount -= Time.deltaTime * 0.5f;
            }
            // Attack power up
            else if (Input.GetButtonDown(PlayerInput.PowerBInput) &&
                Stats.PowerAmount >= 0.5f &&
                Stats.Shield.activeSelf == false &&
                Stats.Rampage.activeSelf == false)
            {
                Stats.Rampage.SetActive(true);
                Stats.RampageTimer = 0.0f;
                Stats.PowerAmount -= 0.5f;
            }
            // Boost power up
            // Hold or tap?
            else if (Input.GetButton(PlayerInput.BoostInput)) //&& Stats.PowerAmount >= 0.3f)
            {
                Stats.CurrentBoostMultiplier = Stats.BoostMultiplier;
                Stats.PowerAmount -= Time.deltaTime * 0.4f;
            }
        }
    }

    /// <summary>
    /// When colliding with a car with an ability active...
    /// </summary>
    /// <param name="collision"></param>
    //private void OnCollisionEnter(Collision collision)
    //{

    //    // Only check collision if the car has activated rampage
    //    if (collision.gameObject.CompareTag("Player") && Stats.Rampage.activeSelf)
    //    {
    //        Vector3 normal = collision.contacts[0].normal;
            
    //        CoreCarModule otherCar = collision.gameObject.GetComponent<CoreCarModule>();

    //        if (otherCar != null && otherCar.Stats.Shield.activeSelf)
    //        {
    //            Controller.Impact(100, normal, 0.75f);
    //        }
    //        else
    //        {
    //            otherCar.Controller.Impact(100, -normal, 0.75f);
    //        }
    //    }
    //}
}