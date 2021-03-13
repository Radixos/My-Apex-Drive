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

    public float eliminationTimer;
    //private float countDownTimer;

    public Text eliminationTimeDisplay;

    // Start is called before the first frame update
    //void Start()
    //{
        //countDownTimer = eliminationTimer;
    //}

    // Update is called once per frame
    //private void Update()
    //{
    //    if(raceCars.Count > 1)
    //    {
    //        countDownTimer -= Time.deltaTime;

    //        if (countDownTimer <= 0)
    //        {
    //            countDownTimer = eliminationTimer;
    //            raceCars[raceCars.Count - 1].gameObject.SetActive(false);
    //            raceCars.Remove(raceCars[raceCars.Count - 1]);
    //        }

    //        eliminationTimeDisplay.text = Mathf.RoundToInt(countDownTimer).ToString();
    //    }
    //    else
    //    {
    //        eliminationTimeDisplay.text = raceCars[0].name.ToString() + " wins!";
    //    }
        
 
    //}

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
                        }
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
