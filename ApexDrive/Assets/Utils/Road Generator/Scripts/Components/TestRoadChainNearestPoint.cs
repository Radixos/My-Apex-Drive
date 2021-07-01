using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class TestRoadChainNearestPoint : MonoBehaviour
{
    [SerializeField] private RoadChain m_Track;
    [SerializeField] private int m_Precision = 10;
    [SerializeField] private int m_Depth = 5;

    [SerializeField] private int m_Itterations;

    private void Update()
    {
        m_Itterations = m_Depth * m_Precision;
        m_Track.GetNearestTimeOnSpline(transform.position, m_Precision, m_Depth);
        // Debug.DrawLine(transform.position, m_Track.GetNearestPositionOnSpline(transform.position, m_Precision, m_Depth), Color.red);
        // Debug.DrawLine(transform.position, m_Track.Evaluate(m_Track.GetNearestTimeOnSpline(transform.position, m_Precision, m_Depth)).pos, Color.green);
        // if(m_Track != null) Debug.DrawLine(transform.position, m_Track.GetNearestPositionOnSpline(transform.position, 10, 5));
    }
}