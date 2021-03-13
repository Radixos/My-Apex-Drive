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
    [SerializeField] private RectTransform[] pointers;
    [SerializeField] private Camera uiCamera;

    private Collider[] objCollider;
    private Plane[] planes;
    private int playersCount;

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

        //for (int i = 0; i < rm.raceCars.Count; i++)
        //    objCollider[i] = rm.raceCars[i].GetComponent<Collider>(); //NullReferenceException
    }

    void Update()
    {
        CameraViewUpdate();
    }

    void CameraViewUpdate()
    {
        for (int i = 0; i < playersCount; i++)
        {
            if (GeometryUtility.TestPlanesAABB(planes, rm.raceCars[i].GetComponent<Collider>().bounds))
            {
                Debug.Log(rm.raceCars[i].name + " has been detected!");
            }
            else
            {
                ArrowPointer(rm.raceCars[i].GetComponent<Transform>().position, i);
                Debug.Log("Nothing has been detected");
            }
        }
    }

    void ArrowPointer(Vector3 target, int playerNumber)
    {
        Vector3 toPosition = target;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360;
        pointers[playerNumber].localEulerAngles = new Vector3(0f, 0f, angle);

        //Vector3 toPosition = target;
        //Vector3 fromPosition = Camera.main.transform.position;
        //fromPosition.z = 0f;
        //Vector3 dir = (toPosition - fromPosition).normalized;
        //float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360;
        //pointers[playerNumber].localEulerAngles = new Vector3(0f, 0f, angle);

        float borderSize = 50f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(target);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize - borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointers[playerNumber].position = pointerWorldPosition;
            pointers[playerNumber].localPosition = new Vector3(pointers[playerNumber].localPosition.x, pointers[playerNumber].localPosition.y, 0f);

        }
            //Vector3 dir = rm.raceCars[playerNumber].transform.InverseTransformPoint();

    }
}