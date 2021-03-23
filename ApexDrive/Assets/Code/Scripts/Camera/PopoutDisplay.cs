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
    private Collider[] objCollider;
    private Plane[] planes;
    private int playersCount;

    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private Canvas ui;

    List<GameObject> arrowPool = new List<GameObject>();
    int arrowPoolCursor = 0;

    private RaceManager rm;

    void Start()
    {
        rm = FindObjectOfType<RaceManager>();
        playersCount = rm.raceCars.Count;

        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }

    void Update()
    {
        CameraViewUpdate();
    }

    void CameraViewUpdate()
    {
        ResetPool();

        foreach (var obj in rm.raceCars)
        {
            if (obj != rm.raceCars[0])
            {
                Debug.Log(obj.name);
                Vector3 screenpos = Camera.main.WorldToScreenPoint(obj.transform.position);

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

                    GameObject arrow = GetArrow();
                    arrow.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    arrow.transform.localPosition = screenpos;
                    arrow.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                }
            }
        }

        CleanPool();
    }

    GameObject GetArrow()
    {
        GameObject output;
        Debug.Log("arrowPoolCursor: " + arrowPoolCursor);
        Debug.Log("arrowPool.Count: " + arrowPool.Count);
        if (arrowPoolCursor < arrowPool.Count)
        {
            output = arrowPool[arrowPoolCursor];
        }
        else
        {
            output = Instantiate(arrowPrefab) as GameObject;
            output.transform.parent = transform;
            arrowPool.Add(output);
        }

        arrowPoolCursor++;
        return output;
    }

    void ResetPool()
    {
        arrowPoolCursor = 0;
    }

    void CleanPool()
    {
        while (arrowPool.Count > arrowPoolCursor)
        {
            GameObject obj = arrowPool[arrowPool.Count - 1];
            arrowPool.Remove(obj);
            Destroy(obj.gameObject);
        }
    }
}