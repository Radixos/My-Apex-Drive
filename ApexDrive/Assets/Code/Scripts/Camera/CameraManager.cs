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
    [SerializeField] private Vector3 m_Offset;
    [SerializeField, Range(0.0f, 2.0f)] private float m_Position;
    [SerializeField, Range(0.0f, 1.0f)] private float m_Smoothing = 0.125f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_TrackProgress;
    [SerializeField, Range(0.0f, 10.0f)] private float m_ShakeSpeed;

    private Vector3 targetPosition = Vector3.zero;

    [SerializeField] private float m_TestShakeStrength, m_TestShakeDuration;

    [SerializeField] private Transform m_OverrideFollowTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if(m_Track != null)
        {
            transform.position = m_Track.Evaluate(m_TrackProgress).pos + m_Offset;
        }
        m_Cameras = GetComponentsInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        if(Application.isPlaying)
        {
            if(m_Track == null) return;


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

            float x = Mathf.PerlinNoise(Time.time * m_ShakeSpeed, 0.0f) * strength - strength / 2.0f;
            float y = Mathf.PerlinNoise(0.0f, Time.time * m_ShakeSpeed) * strength - strength / 2.0f;

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

    [ContextMenu("Add Test Shake")]
    public void AddTestShake()
    {
        AddShake(m_TestShakeStrength, m_TestShakeDuration, 0.25f, 0.25f);
    }

    private void Update()
    {
        if(!Application.isPlaying && m_Track != null)
        {
            transform.position = m_Track.Evaluate(m_TrackProgress).pos + m_Offset;
        }
    }

    public void AddShake(float strength, float duration, float smoothInDuration = 0.0f, float smoothOutDuration = 0.0f)
    {
        CameraShake shake = new CameraShake(strength, duration, smoothInDuration, smoothOutDuration);
        m_CameraShakes.Add(shake);
        StartCoroutine(RampShakeValues(shake));
    }

    public void AddShake(CameraShake shake)
    {
        m_CameraShakes.Add(shake);
        StartCoroutine(RampShakeValues(shake));
    }

    private IEnumerator RampShakeValues(CameraShake shake)
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
}
