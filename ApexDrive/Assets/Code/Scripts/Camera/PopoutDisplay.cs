using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------------------
//                
//This script needs to be attached to a Canvas or UI object:
//eg.
//> UI
//>>Canvas
//           
//---------------------------------------------------------------------------------------

public class PopoutDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popoutPrefab;

    [SerializeField] private Canvas uiCanvas;

    List<GameObject> popoutPool = new List<GameObject>();
    int popoutPoolCursor = 0;

    private RaceManager rm;

    void Start()
    {
        rm = FindObjectOfType<RaceManager>();
    }

    void Update()
    {
        PopoutUpdate();
    }

    void PopoutUpdate()
    {
        ResetPool();

        foreach (var obj in GameManager.Instance.ConnectedPlayers)
        {
            if (obj != GameManager.Instance.ConnectedPlayers[0])
            {
                Vector3 screenpos = Camera.main.WorldToScreenPoint(obj.Car.transform.position);

                if (screenpos.z > 0 &&
                    screenpos.x > 0 && screenpos.x < Screen.width &&
                    screenpos.y > 0 && screenpos.y < Screen.height)
                { }
                else
                {
                    if (screenpos.z < 0)
                        screenpos *= -1;

                    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

                    screenpos -= screenCenter;

                    float angle = Mathf.Atan2(screenpos.y, screenpos.x);
                    angle -= 90f * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = -Mathf.Sin(angle);

                    screenpos = screenCenter + new Vector3(sin * 150f, cos * 150f, 0f);
                    float m = cos / sin;

                    Vector3 screenBounds = screenCenter * 0.9f;

                    if (cos > 0)
                    {
                        screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0f);
                    }
                    else
                    {
                        screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0f);
                    }

                    if (screenpos.x > screenBounds.x)
                    {
                        screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0f);
                    }
                    else if (screenpos.x < -screenBounds.x)
                    {
                        screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0f);
                    }

                    //screenpos += screenCenter;

                    GameObject popout = GetPopout();
                    popout.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    popout.transform.localPosition = screenpos;
                    popout.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                }
            }
        }

        CleanPool();
    }

    GameObject GetPopout()
    {
        GameObject output;

        if (popoutPoolCursor < popoutPool.Count)
        {
            output = popoutPool[popoutPoolCursor];
        }
        else
        {
            output = Instantiate(popoutPrefab) as GameObject;
            output.transform.parent = transform;
            popoutPool.Add(output);
        }

        popoutPoolCursor++;
        return output;
    }

    void ResetPool()
    {
        popoutPoolCursor = 0;
    }

    void CleanPool()
    {
        while (popoutPool.Count > popoutPoolCursor)
        {
            GameObject obj = popoutPool[popoutPool.Count - 1];
            popoutPool.Remove(obj);
            Destroy(obj.gameObject);
        }
    }
}