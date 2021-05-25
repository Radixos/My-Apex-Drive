using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrackScript : MonoBehaviour
{
    //retrieving data from other scripts
    private Vector3 carCameraPos;
    private RaceManager carManager;
    private Camera mainCamera;

    //saving default positions
    private Vector3[] defaultPositions;
    private bool playerDefaultPositionsAcquired = false;

    //victory state
    private bool setVictoryState = false;

    //reset state
    private bool firstPlay = true;
    private bool setResetState = false;

    //track player wins (probably temp)
    private float[] playerWins;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        carManager = this.GetComponent<RaceManager>();
        defaultPositions = new Vector3[carManager.raceCars.Count];
        playerWins = new float[carManager.raceCars.Count];

        //init player wins
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCarPos = carManager.raceCars[i];
            defaultPositions[i] = new Vector3(currentCarPos.transform.position.x, currentCarPos.transform.position.y, currentCarPos.transform.position.z);
            playerWins[i] = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCar = carManager.raceCars[i];
            if (setResetState == true)
            {
                setVictoryState = false;
                //reset player positions
                currentCar.transform.position = defaultPositions[i];
                Debug.Log(currentCar.transform.position);
            }
            if (currentCar.winner == true)
            {
                setResetState = true;
                playerWins[i] = playerWins[i]++;
                firstPlay = false;
                currentCar.winner = false;
            }

        }

        if (setVictoryState == true)
        {
            //menu
        }
    }
}
