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
    // [SerializeField] private GameObject leadPlayer;

    // private RaceManager temp;

    // private Transform objToFollow;
    // private Transform objToLookAt;

    // void Start()
    // {
    //     Initialise();
    // }

    // void Initialise()
    // {
    //     CinemachineVirtualCamera cinemachine = GetComponent<CinemachineVirtualCamera>();
    //     Transform tempSphere = GameObject.FindGameObjectWithTag("PlayerTuk").GetComponent<Transform>();
    //     cinemachine.Follow = tempSphere;
    //     cinemachine.LookAt = tempSphere;
    //     temp = FindObjectOfType<RaceManager>();
    // }

    // void Update()
    // {
    //     UpdateLeadFollow();
    // }

    // void UpdateLeadFollow()
    // {
    //     leadPlayer = temp.raceCars[0].gameObject;

    //     gameObject.GetComponent<CinemachineVirtualCamera>().Follow = leadPlayer.transform;
    //     gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = leadPlayer.transform;
    // }
}
