using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------------------------------------------------------------------------------------
//                
//This script needs to be attached to the Scene Manager or relevant object
//
//Number of pointers must be equal to number of players for the script to work correctly! Or does it?
//           
//---------------------------------------------------------------------------------------

public class PopoutDisplay : MonoBehaviour
{
    private Collider[] objCollider;
    private Plane[] planes;
    private int playersCount;

    [SerializeField] private GameObject arrow; 
    private RaceManager rm;

    private void Awake()
    {
        //for (int i = 0; i < pointers.Length; i++)
        //{
        //    pointers[i] = GetComponent<RectTransform>();    //Potential error
        //}
    }

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
        for (int i = 0; i < playersCount; i++)
        {
            foreach (var player in rm.raceCars)
            {
                Vector3 screenpos = Camera.main.WorldToScreenPoint(player.transform.position);

                    //if (screenpos.z>0 &&
                    //    screenpos.x>0 && screenpos.x<Screen.width &&
                    //    screenpos.y>0 && screenpos.y<Screen.height)
                    //        arrow.transform.localPosition = screenpos;    //Do nothing as nothing has to be done when players are on screen


                if (screenpos.z < 0)
                    screenpos *= -1;

                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
                screenpos -= screenCenter;
                float angle = Mathf.Atan2(screenpos.y, screenpos.x);
                angle -= 90 * Mathf.Deg2Rad;

                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                screenpos = screenCenter + new Vector3(sin * 150f, cos * 150f, 0f);
                float m = cos / sin;

                Vector3 screenBounds = screenCenter * 0.9f;

                if (cos > 0)
                {
                    screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0f);
                }
                else
                {
                    screenpos = new Vector3(-screenBounds.y/m, -screenBounds.y, 0f);
                }

                if (screenpos.x > screenBounds.x)
                {
                    screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0f);
                }
                else if(screenpos.x < -screenBounds.x)
                {
                    screenpos = new Vector3(-screenBounds.x, -screenBounds.x*m, 0f);
                }

                screenpos += screenCenter;

                arrow.transform.localPosition = screenpos;
                arrow.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
            }
        }
    }

    void ArrowDisplay()
    {
        
    }
}