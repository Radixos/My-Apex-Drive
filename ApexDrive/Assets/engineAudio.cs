using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineAudio : MonoBehaviour
{
    // Start is called before the first frame update
    //Inheritance
    public CarAttributes carAttributes;
    [SerializeField]
    private Rigidbody sphereCollider;
    private CarInputHandler carInputHandler;
    private CarStats carStats;
    private CarController controller;


    //FMOD events
    FMOD.Studio.EventInstance sfxDrift;

    private void Start()
    {
        //car = GetComponent<SphereCollider>;
        sfxDrift = FMODUnity.RuntimeManager.CreateInstance("event:/TukTuk/Drifting");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sfxDrift, transform, GetComponent<Rigidbody>());

        //sfxDrift.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
