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

    private enum NearestPointQualifier {Time, Position}
    [SerializeField] private NearestPointQualifier Qualifier;

    private void Update()
    {
        if(m_Track == null) return;
        
        m_Itterations = m_Depth * m_Precision;
        Vector3 point = Vector3.zero;
        if(Qualifier == NearestPointQualifier.Time) point = m_Track.Evaluate(m_Track.GetNearestTimeOnSpline(transform.position, m_Precision, m_Depth)).pos;
        else if (Qualifier == NearestPointQualifier.Position) point = m_Track.GetNearestPositionOnSpline(transform.position, m_Precision, m_Depth);
        Debug.DrawLine(transform.position, point, Color.red);
    }
}