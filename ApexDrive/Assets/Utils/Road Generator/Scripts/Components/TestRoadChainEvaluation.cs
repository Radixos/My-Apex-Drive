using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class TestRoadChainEvaluation : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] private float m_Evaluate = 0.0f;
    [SerializeField] private RoadChain m_Track;

    private void Update()
    {
        if(m_Track != null) transform.position = m_Track.Evaluate(m_Evaluate).pos;
    }
}
