using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    private Vector3 carCameraPos;
    private Camera mainCamera;
    private float waitTimer = 2.5f;
    int eliminatedTotal = 0;
    private bool winnerHasBeenProcessed;

    void Start()
    {
        mainCamera = Camera.main;
        winnerHasBeenProcessed = false;
        //GameManager instance connected players --> core carmodule
    }

    void Update()
    {
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            CoreCarModule currentCar = GameManager.Instance.ConnectedPlayers[i].Car;
            carCameraPos = mainCamera.WorldToViewportPoint(currentCar.transform.position);
            bool boundaryCheck = checkBoundaries(carCameraPos);

            EliminationProcess(currentCar, boundaryCheck);
            WinnerCheck(currentCar);
        }
    }

    private void EliminationProcess(CoreCarModule currentCar, bool boundaryCheck)
    {
        if (boundaryCheck == true && currentCar.Player.PlayerEliminated == false && winnerHasBeenProcessed == false)
        {
            if (currentCar.Player.OffScreenTimer < waitTimer)
            {
                currentCar.Player.SetOffScreenTimer(currentCar.Player.OffScreenTimer + Time.deltaTime);
            }

            else
            {
                currentCar.gameObject.SetActive(false);
                currentCar.Player.EliminatePlayer(true);
                FMODUnity.RuntimeManager.PlayOneShot("event:/TukTuk/Elimination");
                eliminatedTotal++;
            }
        }

        else if (boundaryCheck == false)
        {
            currentCar.Player.SetOffScreenTimer(0);
        }
    }

    private void WinnerCheck(CoreCarModule currentCar)
    {
        if (eliminatedTotal == GameManager.Instance.PlayerCount - 1 && currentCar.Player.PlayerEliminated == false)
        {
            int playerToModify = currentCar.Player.PlayerID - 1;
            //GameManager.Instance.SubmitRoundWinner(playerToModify);
            currentCar.Player.WinRound();
            //currentCar.winner = true;
            eliminatedTotal = 0;
            winnerHasBeenProcessed = true;

            if (currentCar.Player.RoundWins % 3 == 0)
            {
                //GameManager.Instance.SubmitGameWinner(playerToModify);
                currentCar.Player.WinGame();
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