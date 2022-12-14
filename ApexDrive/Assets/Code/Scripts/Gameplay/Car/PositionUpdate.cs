using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdate : MonoBehaviour
{
    public int laps, collidersHit;
    public float offScreenTimer;
    public bool eliminated, winner;

    private List<GameObject> hitColliders = new List<GameObject>();

    private RaceManager raceManager;

    public Transform distanceCollider;

    public float distanceFromCollider;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();

        distanceFromCollider = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (distanceCollider != null)
            distanceFromCollider = Vector3.Distance(transform.position,
                new Vector3(distanceCollider.position.x, transform.position.y, distanceCollider.position.z));
    }

    public int GetPosition()
    {
        for(int i = 0; i < raceManager.raceCars.Count; i++)
        {
            if (GetInstanceID() == raceManager.raceCars[i].GetInstanceID())
                return i + 1;
        }
        return 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Waypoint"))
        {
            if (hitColliders.Count == 0)
            {
                hitColliders.Add(other.gameObject);
                collidersHit++;

                distanceCollider = other.gameObject.transform;
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

                distanceCollider = other.gameObject.transform;

            }

        }


    }
}
