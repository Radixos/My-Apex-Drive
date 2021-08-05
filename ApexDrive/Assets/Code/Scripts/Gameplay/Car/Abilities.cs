using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Abilities : CarModule
{
    [SerializeField] private InputAction m_BoostInput;

    [SerializeField]
    [EventRef]
    private string m_BoostSFXPath = null;
    FMOD.Studio.EventInstance m_BoostSFX;
    FMOD.Studio.PLAYBACK_STATE m_BoostState;

    [SerializeField] private ParticleSystem m_BoostVFX;
    [SerializeField] private ParticleSystem m_AttackVFX;
    [SerializeField] private ParticleSystem m_ShieldVFX;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Stats.CanDrive) Stats.PowerAmount += 0.05f * Time.deltaTime;

        if(Stats.CanDrive && InputManager.GetButtonDown(Player.ControllerType, m_BoostInput, Player.ControllerID) && Stats.PowerAmount >= 0.25f)
        {
            Rigidbody.AddForce(transform.forward * 10000.0f, ForceMode.Impulse);
            if(m_BoostVFX != null) m_BoostVFX.Play();
            Stats.PowerAmount -= 0.25f;
        }
        if (Stats.PowerAmount < 0) Stats.PowerAmount = 0;
    }

    private void AbilityLogic()
    {
        if (Stats.PowerAmount > 0)
        {
            // Activate one ability at a times
            // Shield power up
            // if (Input.GetButton(PlayerInput.PowerAInput) && abilitiesActive == false)
            // {
            //     if (!Stats.InitialShieldPowerDepleted)
            //     {
            //         Stats.PowerAmount -= 0.15f;
            //         Stats.InitialShieldPowerDepleted = true;
            //         Stats.Shield.SetActive(true);
            //     }
            //     Stats.PowerAmount -= Time.deltaTime * 0.5f;
            // }


            // Attack power up
            // else if (Input.GetButtonDown(PlayerInput.PowerBInput) &&
            //     Stats.PowerAmount >= 0.5f &&
            //     Stats.Shield.activeSelf == false &&
            //     Stats.Rampage.activeSelf == false)
            // {
            //     Stats.Rampage.SetActive(true);
            //     Stats.RampageTimer = 0.0f;
            //     Stats.PowerAmount -= 0.5f;
            // }

            
        }
    }

}