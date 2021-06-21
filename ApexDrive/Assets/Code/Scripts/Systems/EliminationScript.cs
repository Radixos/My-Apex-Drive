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
    private bool winnerHasBeenProcessed;

    void Start()
    {
        mainCamera = Camera.main;
        carManager = this.GetComponent<RaceManager>();
        winnerHasBeenProcessed = false;
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

        Debug.Log("Player 1 has won " + GameManager.Instance.Players[0].RoundWins + " rounds and " 
            + GameManager.Instance.Players[0].GameWins + " games");
        Debug.Log("Player 2 has won " + GameManager.Instance.Players[1].RoundWins + " rounds and "
            + GameManager.Instance.Players[1].GameWins + " games");
    }

    private void EliminationProcess(PositionUpdate currentCar, bool boundaryCheck)
    {
        if (boundaryCheck == true && currentCar.eliminated == false && winnerHasBeenProcessed == false)
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
            CoreCarModule currentCarModule = currentCar.gameObject.transform.parent.GetChild(1).GetComponent<CoreCarModule>();
            int playerToModify = currentCarModule.Player.PlayerID - 1;
            GameManager.Instance.SubmitRoundWinner(playerToModify);
            currentCar.winner = true;
            eliminatedTotal = 0;
            winnerHasBeenProcessed = true;
            //Debug.Log(GameManager.Instance.Players[currentCar.GetComponent<CarInputHandler>().currentPlayer].RoundWins);
            //debug.log(gamemanager.instance.players[playertomodify].roundwins);

            if (GameManager.Instance.Players[playerToModify].RoundWins % 3 == 0)
            {
                GameManager.Instance.SubmitGameWinner(playerToModify);
            }
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