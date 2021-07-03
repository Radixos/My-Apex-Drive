using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Car_Impact : MonoBehaviour
{
    FMOD.Studio.EventInstance Impact;
    FMOD.Studio.EventDescription Force;

    FMOD.Studio.PARAMETER_DESCRIPTION impForce;
    FMOD.Studio.PARAMETER_ID FCE;

    float carVelocity;
    float direction;

    // Start is called before the first frame update
    void Start()
    {
        Impact = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/Impact");

        Force = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/Impact");
        Force.getParameterDescriptionByName("Force", out impForce);
        FCE = impForce.id;

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Impact, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
