using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputHandler))]
public class Abilities : MonoBehaviour
{
    public CarInputHandler carInputHandler;

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
            if (Input.GetButton(carInputHandler.PowerAInput) &&
                rampage.activeSelf == false &&
                speedMultiplier == 1.0f)
            {
                shield.SetActive(true);
                powerAmount -= Time.deltaTime * 0.5f;
            }
            else if (Input.GetKeyDown(KeyCode.E) &&
                powerAmount >= 0.5f &&
                speedMultiplier == 1.0f &&
                shield.activeSelf == false &&
                rampage.activeSelf == false)
            {
                rampage.SetActive(true);
                rampageTimer = 0.0f;
                powerAmount -= 0.5f;
            }
            else if (Input.GetKeyDown(KeyCode.Q) &&
                powerAmount >= 0.3f &&
                shield.activeSelf == false &&
                rampage.activeSelf == false &&
                speedMultiplier != 1.0f)
            {
                speedMultiplier = 2.0f;
                speedBoostTimer = 0.0f;
                powerAmount -= 0.3f;
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

