using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode()]
[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private RoadChain m_Track;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField, Range(0.0f, 2.0f)] private float m_Position;
    [SerializeField, Range(0.0f, 1.0f)] private float m_Smoothing = 0.125f;

    [SerializeField] private Transform m_DebugFollowTransform;


    private void FixedUpdate()
    {
        if(m_Camera == null) m_Camera = GetComponent<Camera>();
        if(m_Camera == null || m_Track == null) return;

        Vector3 targetPosition = m_Track.GetNearestPositionOnSpline(m_DebugFollowTransform.position, 10, 5);
        Debug.DrawLine(targetPosition, m_DebugFollowTransform.position);
        // Vector3 targetPosition = m_Track.Evaluate(m_Position % 1.0f).pos;
        Vector3 desiredPosition =  targetPosition + m_Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_Smoothing);
        transform.position = smoothedPosition;
    }
}
