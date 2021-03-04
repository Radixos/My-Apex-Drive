using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{

    public List<PositionUpdate> raceCars;

    public int totalColliders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for(int i = 0; i < raceCars.Count; i++)
        {
            for (int j = 0; j < raceCars.Count; j++)
            {
                if(raceCars[i].gameObject.GetInstanceID() != 
                    raceCars[j].gameObject.GetInstanceID())
                {
                    //if(raceCars[i].laps > raceCars[j].laps)
                    //{

                    //}
                    
                    if(raceCars[i].collidersHit > raceCars[j].collidersHit)
                    {

                        if (raceCars.IndexOf(raceCars[i]) > raceCars.IndexOf(raceCars[j]))
                        {
                            PositionUpdate temp = raceCars[j];
                            raceCars[j] = raceCars[i];
                            raceCars[i] = temp;
                        }
                    }
                }
            }
        }
    }
}
