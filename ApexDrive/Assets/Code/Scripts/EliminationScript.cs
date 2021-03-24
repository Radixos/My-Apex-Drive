using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminationScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 carCameraPos = mainCamera.WorldToViewportPoint(gameObject.transform.position);
        if ((carCameraPos.x > 1.0f || carCameraPos.x < 0.0f) || (carCameraPos.y > 1.0f || carCameraPos.y < 0.0f))
        {
            Destroy(this);
        }
        Debug.Log(carCameraPos.x + "," + carCameraPos.y);
    }

    void OnBecomeInvisible()
    {
        Destroy(this);
    }
}
