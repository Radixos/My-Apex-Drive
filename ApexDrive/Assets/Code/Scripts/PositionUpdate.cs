using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdate : MonoBehaviour
{
    public int laps;
    public int collidersHit;
    public float offScreenTimer;

    private List<GameObject> hitColliders = new List<GameObject>();

    private RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(gameObject.CompareTag("Player"))
    //    {
    //        for (int i = 0; i < raceManager.raceCars.Count; i++)
    //        {
    //            if (raceManager.raceCars[i].gameObject == gameObject)
    //            {
    //                Debug.Log("Position: " + (i + 1));
    //            }
    //        }
    //    }
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Waypoint"))
        {
            if (hitColliders.Count == 0)
            {
                hitColliders.Add(other.gameObject);
                collidersHit++;
            }
            else if (hitColliders.Contains(other.gameObject) == false)
            {

                collidersHit++;
                hitColliders.Add(other.gameObject);


                if (collidersHit >= raceManager.totalColliders)
                {
                    collidersHit = 0;
                    laps++;
                    hitColliders.Clear();
                }

            }
        }


    }
}
