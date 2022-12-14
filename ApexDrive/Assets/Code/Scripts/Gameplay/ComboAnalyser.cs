using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAnalyser : MonoBehaviour
{
    [SerializeField] private Transform m_FrontSensor;
    [SerializeField] private Transform m_BackSensor;
    private Corner m_Corner;

    private void LateUpdate()
    {
        if(m_Corner!= null) AnalyseDrift();
    }

    private void AnalyseDrift()
    {
        Vector3 front = m_FrontSensor.position;
        Vector3 back = m_BackSensor.position;
        Vector3 apex = m_Corner.GetNearestPointOnSpline(front);
        front.y = 0;
        back.y = 0;
        apex.y = 0;
        Vector3 apexDirection = (apex-front).normalized;
        Vector3 carDirection = (front - back).normalized;

        //use dot product to calculate the difference between the direction the car is pointing and the direction of the front of the car to the apex of the corner.
        float direction = Mathf.Clamp01(Vector3.Dot(apexDirection, carDirection) * 2f - 0.25f);
        float distanceToApex = Vector3.Distance(front, apex);
        float evaluatedDistanceToApex =  1.0f-Mathf.Clamp01((distanceToApex-2.0f)/10.0f);
    }

    public void SetCorner(Corner c)
    {
        m_Corner = c;
    }
}
