using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetTrackScript : MonoBehaviour
{
    //retrieving data from other scripts

    private Vector3 carCameraPos;
    private RaceManager carManager;
    private Camera mainCamera;

    //saving default positions

    public Vector3[] defaultPositions;
    public Quaternion[] defaultRotations;

    //victory state

    private bool setVictoryState = false;

    //reset state

    private bool firstPlay = true;
    //avoids conflict with elimination script
    private bool setResetState = false;
    public int noOfPlayers;

    //track player wins (probably temp)

    public float[] playerWins;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        carManager = this.GetComponent<RaceManager>();
        defaultPositions = new Vector3[carManager.raceCars.Count];
        defaultRotations = new Quaternion[carManager.raceCars.Count];
        playerWins = new float[carManager.raceCars.Count];
        noOfPlayers = carManager.raceCars.Count;

        //init player data
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCarPos = carManager.raceCars[i];
            defaultPositions[i] = new Vector3(currentCarPos.transform.position.x, currentCarPos.transform.position.y, currentCarPos.transform.position.z);
            defaultRotations[i] = new Quaternion(currentCarPos.transform.rotation.w, currentCarPos.transform.rotation.x, currentCarPos.transform.rotation.y, currentCarPos.transform.rotation.z);
            playerWins[i] = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (setVictoryState == true)
        {
            setVictoryState = false;
            SceneManager.LoadScene("VictoryScene");
        }
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCar = carManager.raceCars[i];
            initVictoryState(i, currentCar);
            resetPlayers(i, currentCar);
        }
    }

    private void initVictoryState(int i, PositionUpdate currentCar)
    {
        if (currentCar.winner == true && setResetState == false)
        {
            setResetState = true;
            playerWins[i] = playerWins[i]+1;
            firstPlay = false;
        }
    }

    private void resetPlayers(int i, PositionUpdate currentCar)
    {
        for (int j = 0; j < carManager.raceCars.Count; j++)
        {
            if (setResetState == true)
            {
                PositionUpdate cycleThroughCars = carManager.raceCars[j];
                //reset player positions
                cycleThroughCars.transform.position = defaultPositions[j];
                cycleThroughCars.transform.rotation = defaultRotations[j];
                cycleThroughCars.gameObject.SetActive(true);
                cycleThroughCars.eliminated = false;
                cycleThroughCars.laps = 0;
                cycleThroughCars.collidersHit = 0;
                cycleThroughCars.winner = false;
                //after cycling through all players and resetting data, end reset state
                if (j == (noOfPlayers - 1) && setResetState == true)
                {
                    Debug.Log(cycleThroughCars.transform.position);
                    setResetState = false;
                    setVictoryState = true;
                }
            }
        }
    }
}
