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

    [SerializeField] private GameObject m_CarPrefab;
    [SerializeField] private Transform[] m_InitialSpawnPoints;
    public delegate void SpawnPlayerEvents();
    public static SpawnPlayerEvents OnSpawnPlayers;


    public List<PositionUpdate> raceCars;

    public int totalColliders;

    void Start()
    {
        Initialise();
    }

    void Initialise()
    {
        for(int i = 0; i < GameManager.Instance.PlayerCount; i++)
        {
            if(m_InitialSpawnPoints.Length < i) break;
            GameObject car = GameObject.Instantiate(m_CarPrefab, m_InitialSpawnPoints[i].position, m_InitialSpawnPoints[i].rotation);
            CoreCarModule core = car.GetComponentInChildren<CoreCarModule>();
            core.SetPlayer(GameManager.Instance.Players[i]);
            raceCars.Add(car.GetComponentInChildren<PositionUpdate>());
        }
        if(OnSpawnPlayers != null) OnSpawnPlayers();
        // foreach (PositionUpdate positionUpdate in FindObjectsOfType<PositionUpdate>())
        // {
        //     raceCars.Add(positionUpdate);
        // }

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

                    if(raceCars[i].distanceCollider.GetInstanceID() == raceCars[j].distanceCollider.GetInstanceID() &&
                        raceCars[i].distanceFromCollider > raceCars[j].distanceFromCollider)
                    {
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