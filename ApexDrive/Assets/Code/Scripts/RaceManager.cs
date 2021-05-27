using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    #region Singleton
    public static RaceManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion

    public List<PositionUpdate> raceCars;

    public int totalColliders;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    //private void Update()
    //{       
    //}

    void Initialise()
    {
        foreach (PositionUpdate positionUpdate in FindObjectsOfType<PositionUpdate>())
        {
            raceCars.Add(positionUpdate);
        }

        totalColliders = GameObject.FindGameObjectsWithTag("Waypoint").Length;
    }

    void LateUpdate()
    {
        for (int i = 0; i < raceCars.Count; i++)
        {
            for (int j = 0; j < raceCars.Count; j++)
            {
                if (raceCars[i].gameObject.GetInstanceID() !=
                    raceCars[j].gameObject.GetInstanceID())
                {
                    if (raceCars[i].laps > raceCars[j].laps)
                    {
                        SwapRacers(i, j);
                        continue;
                    }

                    if (raceCars[i].collidersHit > raceCars[j].collidersHit)
                    {
                        if (raceCars[i].laps == raceCars[j].laps)
                        {
                            SwapRacers(i, j);
                            continue;
                        }
                    }

                    if (raceCars[i].distanceCollider.GetInstanceID() == raceCars[j].distanceCollider.GetInstanceID() &&
                        raceCars[i].distanceFromCollider > raceCars[j].distanceFromCollider)
                    {
                        // Helping camera to focus on the car in first place
                        // with a bit of leisure space
                        if (raceCars[i].distanceFromCollider - raceCars[j].distanceFromCollider >= 1.0f)
                            SwapRacers(i, j);
                    }

                }
            }
        }

    }

    private void SwapRacers(int a, int b)
    {

        if (raceCars.IndexOf(raceCars[a]) > raceCars.IndexOf(raceCars[b]))
        {
            PositionUpdate temp = raceCars[b];
            raceCars[b] = raceCars[a];
            raceCars[a] = temp;
        }

    }
}