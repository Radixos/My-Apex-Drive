using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 carCameraPos;
    private RaceManager carManager;
    private bool destroyPlayer;
    private Camera mainCamera;
    private float waitTimer = 2.5f;

    void Start()
    {
        mainCamera = Camera.main;
        carManager = this.GetComponent<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < carManager.raceCars.Count; i++)
        {
            PositionUpdate currentCar = carManager.raceCars[i];
            destroyPlayer = false;
            carCameraPos = mainCamera.WorldToViewportPoint(currentCar.transform.position);
            bool boundaryCheck = checkBoundaries(carCameraPos);

            if (boundaryCheck == true)
            {
                if (currentCar.offScreenTimer < waitTimer/* && boundaryCheck == true*/)
                {
                    currentCar.offScreenTimer += Time.deltaTime;
                    //boundaryCheck = checkBoundaries(carCameraPos);
                }
                
                //boundaryCheck = checkBoundaries(carCameraPos);
                if (/*boundaryCheck == true &&*/ currentCar.offScreenTimer >= waitTimer)
                {
                    Destroy(currentCar.gameObject);
                }
            }
            //Debug.Log(carCameraPos.x + "," + carCameraPos.y);
            Debug.Log(currentCar.name.ToString() + ", " + currentCar.offScreenTimer.ToString());
         }
       //go through eachcar 
       //check if outside of boundaries
       //if yes, give 2 and a half second window of checking
       //after, if yes then destroy
    }

    private IEnumerator CheckIfStillOut()
    {
        yield return new WaitForSeconds(2.5f);
        bool finalCheck = checkBoundaries(carCameraPos);
        
        if (finalCheck == true)
        {
            destroyPlayer = true;
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