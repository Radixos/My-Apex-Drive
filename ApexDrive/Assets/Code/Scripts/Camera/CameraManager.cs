using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private RoadChain m_Track;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField, Range(0.0f, 2.0f)] private float m_Position;
    [SerializeField, Range(0.0f, 1.0f)] private float m_Smoothing = 0.125f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_TrackProgress;

    private Vector3 targetPosition = Vector3.zero;

    [SerializeField] private Transform m_OverrideFollowTarget;


    private void FixedUpdate()
    {
        if(Application.isPlaying)
        {
            if(m_Camera == null) m_Camera = GetComponent<Camera>();
            if(m_Camera == null || m_Track == null) return;


            if(RaceManager.State == RaceManager.RaceState.Racing) 
            {
                Player leadPlayer = RaceManager.Instance.FirstPlayer;
                if(leadPlayer != null) targetPosition = m_Track.GetNearestPositionOnSpline(leadPlayer.Car.Position, 10, 5);
            }
            if(m_OverrideFollowTarget != null) targetPosition = m_Track.GetNearestPositionOnSpline(m_OverrideFollowTarget.position, 10, 5);
            Vector3 desiredPosition =  targetPosition + m_Offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_Smoothing);
            transform.position = smoothedPosition;
        }
    }

    private void Update()
    {
        if(!Application.isPlaying && m_Track != null)
        {
            transform.position = m_Track.Evaluate(m_TrackProgress).pos + m_Offset;
        }
    }
}
