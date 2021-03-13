using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//------------------------------------------------------
//                
//This script needs to be attached to the working camera
//           
//------------------------------------------------------

public class CameraFollowLead : MonoBehaviour
{
    [SerializeField] private GameObject leadPlayer;

    private RaceManager temp;

    private Transform objToFollow;
    private Transform objToLookAt;

    //TODO: research: transform.GetComponent(typeof(Transform)) as Transform; https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html 

    void Start()
    {
        temp = FindObjectOfType<RaceManager>();
    }

    void Update()
    {
        UpdateLeadFollow();
    }

    void UpdateLeadFollow()
    {
        leadPlayer = temp.raceCars[0].gameObject;

        //Determine transform for objToFollow
        //Determine transform for objToLookAt

        gameObject.GetComponent<CinemachineVirtualCamera>().Follow = leadPlayer.transform;
        gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = leadPlayer.transform;
    }
}
