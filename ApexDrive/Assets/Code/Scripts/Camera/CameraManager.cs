using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
[DefaultExecutionOrder(1000)]
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private List<CameraShake> m_CameraShakes = new List<CameraShake>();
    private Camera[] m_Cameras;
    [SerializeField] private RoadChain m_Track;
    [SerializeField] private float m_Offset;
    [SerializeField] private AnimationCurve m_ZoomCurve;
    [SerializeField, Range(0.0f, 1.0f)] private float m_Zoom = 0.0f;
    private float m_ZoomDuration = 10.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_Smoothing = 0.125f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_TrackProgress;

    private Vector3 targetPosition = Vector3.zero;

    [SerializeField] private Transform m_OverrideFollowTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        m_Cameras = GetComponentsInChildren<Camera>();
        RaceManager.OnRoundEnd += UpdateTrackProgress;
        UpdateTrackProgress();
        if(m_Track != null)
        {
            transform.position = m_Track.Evaluate(m_TrackProgress).pos - transform.forward * m_Offset;
        }
    }

    private void OnDisable()
    {
        RaceManager.OnRoundEnd -= UpdateTrackProgress;
    }

    private void UpdateTrackProgress()
    {
        m_TrackProgress = RaceManager.Instance.TrackProgress;
    }

    public void StartZoom()
    {

    }


    private void FixedUpdate()
    {
        if(Application.isPlaying)
        {
            if(m_Track == null) return;

            Player leadPlayer = RaceManager.Instance.FirstPlayer;
            if(leadPlayer != null && leadPlayer.Car != null && leadPlayer.Car.gameObject.activeSelf) targetPosition = m_Track.GetNearestPositionOnSpline(leadPlayer.Car.Position, 10, 5);
            else if(m_OverrideFollowTarget != null) targetPosition = m_Track.GetNearestPositionOnSpline(m_OverrideFollowTarget.position, 10, 5);
            else targetPosition = m_Track.Evaluate(m_TrackProgress).pos;
            Vector3 desiredPosition =  targetPosition - transform.forward * m_Offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_Smoothing);
            transform.position = smoothedPosition;
        }

        if(m_CameraShakes.Count > 0)
        {
            float strength = 0.0f;

            for(int i = 0; i < m_CameraShakes.Count; i++)
            {
                if(m_CameraShakes[i].Strength > strength)
                {
                    strength = m_CameraShakes[i].Strength;
                }
            }

            float x = Mathf.PerlinNoise(Time.time * 10.0f, 0.0f) * strength - strength / 2.0f;
            float y = Mathf.PerlinNoise(0.0f, Time.time * 10.0f) * strength - strength / 2.0f;

            foreach(Camera camera in m_Cameras)
            {
                camera.transform.localPosition = new Vector3(x,y);
            }
        }
        else
        {
            foreach(Camera camera in m_Cameras)
            {
                camera.transform.localPosition = Vector3.zero;
                
                
            }
        }
    }

    private void Update()
    {
        if(!Application.isPlaying && m_Track != null)
        {
            transform.position = m_Track.Evaluate(m_TrackProgress).pos - transform.forward * m_Offset;
        }
    }

    public void AddShake(float strength, float duration, float smoothInDuration = 0.0f, float smoothOutDuration = 0.0f)
    {
        CameraShake shake = new CameraShake(strength, duration, smoothInDuration, smoothOutDuration);
        m_CameraShakes.Add(shake);
        StartCoroutine(Co_RampShakeValues(shake));
    }

    public void AddShake(CameraShake shake)
    {
        m_CameraShakes.Add(shake);
        StartCoroutine(Co_RampShakeValues(shake));
    }

    private IEnumerator Co_RampShakeValues(CameraShake shake)
    {
        // Ramp in
        float elapsed = 0.0f;
        while(elapsed < shake.SmoothInDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            shake.Strength = Mathf.Lerp(0.0f, shake.TargetStrength, elapsed / shake.SmoothInDuration);
        }
        // hold duration
        yield return new WaitForSeconds(shake.Duration);

        // ramp out
        elapsed = 0.0f;
        while(elapsed < shake.SmoothOutDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            shake.Strength = Mathf.Lerp(shake.TargetStrength, 0.0f, elapsed / shake.SmoothOutDuration);
        }
        m_CameraShakes.Remove(shake);
    }

    private IEnumerator Co_Zoom()
    {
        float elapsed = 0.0f;

        while(elapsed < m_ZoomDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            m_Zoom = m_ZoomCurve.Evaluate(elapsed / m_ZoomDuration);
        }
    }
}
