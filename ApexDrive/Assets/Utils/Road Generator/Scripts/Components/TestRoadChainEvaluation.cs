using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoadChainEvaluation : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] private float m_Evaluate = 0.0f;
    [SerializeField] private RoadChain m_Track;

    private void OnValidate()
    {
        if(m_Track != null)
        {
            OrientedPoint point = m_Track.Evaluate(m_Evaluate);
            transform.position = point.pos;
            transform.rotation = point.rot;
        }
    }
}
