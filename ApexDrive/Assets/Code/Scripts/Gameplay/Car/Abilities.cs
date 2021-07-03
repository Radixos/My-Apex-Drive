using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Abilities : CarModule
{
    // MANI'S CODE

    //FMOD Stuff
    [SerializeField]
    [EventRef]
    private string defense = null;
    FMOD.Studio.EventInstance sfxDefense;

    [SerializeField]
    [EventRef]
    private string offense = null;
    FMOD.Studio.EventInstance sfxOffense;

    [SerializeField]
    [EventRef]
    private string boost = null;
    FMOD.Studio.EventInstance sfxBoost;

    FMOD.Studio.PLAYBACK_STATE pbsdef;
    FMOD.Studio.PLAYBACK_STATE pbsboost;

    //Used to detect if any abilities are active or not to prevent the player from using more than one ability
    private bool abilitiesActive;
    public int sfxStop;

    private void Start()
    {
        sfxDefense = RuntimeManager.CreateInstance(defense);
        sfxDefense.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
    }

    private void Update()
    {
        AbilityLogic();
        SfxAbilities();
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
        if (Stats.PowerAmount <= 0)
        {
            Stats.Shield.SetActive(false);

            Stats.CurrentBoostMultiplier = 1;
        }

        if (Input.GetButtonUp(PlayerInput.PowerAInput))
        {
            Stats.InitialShieldPowerDepleted = false;
            Stats.Shield.SetActive(false);
        }

        if (Input.GetButtonUp(PlayerInput.BoostInput))
        {
            Stats.CurrentBoostMultiplier = 1;
        }

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
            if (Input.GetButton(PlayerInput.PowerAInput) && abilitiesActive == false)
            {
                if (!Stats.InitialShieldPowerDepleted)
                {
                    Stats.PowerAmount -= 0.15f;
                    Stats.InitialShieldPowerDepleted = true;
                    Stats.Shield.SetActive(true);
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

    public void SfxAbilities()
    {
        sfxDefense.setParameterByName("Stop", sfxStop);
        sfxDefense.getPlaybackState(out pbsdef);
        if (pbsdef == FMOD.Studio.PLAYBACK_STATE.STOPPED || pbsdef == FMOD.Studio.PLAYBACK_STATE.STOPPING)
        {
            if (Stats.Shield.activeSelf == true)
            {
                sfxStop = 0;
                sfxDefense.start();
            }
        }
        if (Stats.Shield.activeSelf == false)
        {
            sfxStop = 1;
            sfxDefense.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {

        // Only check collision if the car has activated rampage
        if (collision.gameObject.CompareTag("PlayerTuk") && Stats.Rampage.activeSelf)
        {
            Vector3 normal = collision.contacts[0].normal;

            // if (collision.gameObject.GetComponent<AbilityCollision>().Stats.Shield.activeSelf)
            // {
            //     Controller.Impact(200, normal, 0.75f);
            // }
            // else
            // {
                // collision.gameObject.GetComponent<AbilityCollision>().sphereCarController.Impact(200, -normal, 0.75f);
            // }

        }

    }
}