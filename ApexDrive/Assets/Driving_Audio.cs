using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driving_Audio : MonoBehaviour
{
    FMOD.Studio.EventInstance Engine;
    FMOD.Studio.EventDescription SPD;
    FMOD.Studio.PARAMETER_DESCRIPTION Speed;
    FMOD.Studio.PARAMETER_ID RPM;

    FMOD.Studio.PARAMETER_DESCRIPTION Accelleration;
    FMOD.Studio.PARAMETER_ID ACE;
    // Start is called before the first frame update
    void Start()
    {
        Engine = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/Engine");

        SPD = FMODUnity.RuntimeManager.GetEventDescription("event:/TukTuk/Engine");
        SPD.getParameterDescriptionByName("RPM", out Speed);
        RPM = Speed.id;

        SPD.getParameterDescriptionByName("Accelleration", out Accelleration);
        ACE = Accelleration.id;

        Engine.start();
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Engine, GetComponent<Transform>(), GetComponent <Rigidbody>());
        if(Input.GetMouseButtonDown(1))
        {
            Engine.setParameterByID(RPM, 1f);
            Engine.setParameterByID(ACE, 1f);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Engine.setParameterByID(RPM, 0f);
            Engine.setParameterByID(ACE, 0f);
        }
    }
}
