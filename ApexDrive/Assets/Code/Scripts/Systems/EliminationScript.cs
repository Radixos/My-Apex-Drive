using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    private Vector3 carCameraPos;
    private RaceManager carManager;
    private Camera mainCamera;
    private float waitTimer = 2.5f;
    float eliminatedTotal = 0.0f;

    void Start()
    {
        mainCamera = Camera.main;
        carManager = this.GetComponent<RaceManager>();
    }

    void Update()
    {
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCar = carManager.raceCars[i];
            carCameraPos = mainCamera.WorldToViewportPoint(currentCar.transform.position);
            bool boundaryCheck = checkBoundaries(carCameraPos);

            EliminationProcess(currentCar, boundaryCheck);
            WinnerCheck(currentCar);
        }
    }

    private void EliminationProcess(PositionUpdate currentCar, bool boundaryCheck)
    {
        if (boundaryCheck == true && currentCar.eliminated == false)
        {
            if (currentCar.offScreenTimer < waitTimer)
            {
                currentCar.offScreenTimer += Time.deltaTime;
            }

            else
            {
                currentCar.gameObject.SetActive(false);
                currentCar.eliminated = true;
                FMODUnity.RuntimeManager.PlayOneShot("event:/TukTuk/Elimination");
                eliminatedTotal++;
            }
        }

        else if (boundaryCheck == false)
        {
            currentCar.offScreenTimer = 0;
        }
    }

    private void WinnerCheck(PositionUpdate currentCar)
    {
        if (eliminatedTotal == carManager.raceCars.Count - 1 && currentCar.eliminated == false)
        {
            currentCar.winner = true;
            GameManager.Instance.SubmitRoundWinner(1);
            eliminatedTotal = 0;
            //Debug.Log(GameManager.Instance.Players[currentCar.GetComponent<CarInputHandler>().currentPlayer].RoundWins);
        }
    }

    private bool checkBoundaries(Vector3 positionToCheck)
    {
        if ((positionToCheck.x > 1.0f || positionToCheck.x < 0.0f) || (positionToCheck.y > 1.0f || positionToCheck.y < 0.0f))
        {
            return true;
        }

        else {return false;}
    }
}