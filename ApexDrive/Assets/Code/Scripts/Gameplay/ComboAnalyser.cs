using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAnalyser : CarModule
{
    [SerializeField] private Transform m_FrontSensor;
    [SerializeField] private Transform m_BackSensor;
    private Corner m_Corner;

    private float m_DriftDuration;
    private float m_DriftMultiplier = 1.0f;
    private float m_DriftPoints = 0.0f;
    [SerializeField] private float m_MinimumDriftDuration = 0.5f;

    private bool m_IsComboing = false;

    public delegate void ComboEvent(string comboName, float points);
    public ComboEvent OnCombo;

    [SerializeField] private ComboUI m_ComboUIPrefab;
    private ComboUI m_CurrentComboUI;

    private string m_CurrentComboName = "";

    [SerializeField, FMODUnity.EventRef] private string m_ComboExecuteSFXPath;

    private void LateUpdate()
    {
        if(m_IsComboing)
        {
            if(OnCombo != null) OnCombo(m_CurrentComboName, m_DriftPoints);
        }
        
        if(Stats.IsDrifting && Stats.CanDrive) AnalyseDrift();
        else if(m_DriftDuration > 0.0f)
        {
            if(m_IsComboing)
            {
                // score points
                Core.ScorePoints(Mathf.Clamp01(m_DriftPoints));
                OnCombo -= m_CurrentComboUI.UpdateCombo;
                // m_CurrentComboUI.transform.SetParent(null);
                m_CurrentComboUI.Disappear();
                Destroy(m_CurrentComboUI.gameObject, 0.25f);
                m_IsComboing = false;
                FMODUnity.RuntimeManager.PlayOneShot(m_ComboExecuteSFXPath, transform.position);
            }
            m_DriftPoints = 0.0f;
            m_DriftMultiplier = 1.0f;
            m_DriftDuration = 0.0f;
        }
    }

    private void AnalyseDrift()
    {
        m_DriftDuration += Time.deltaTime;

        if(m_Corner!= null)
        {
            Vector3 front = m_FrontSensor.position;
            Vector3 back = m_BackSensor.position;
            Vector3 apex = m_Corner.transform.position;
            front.y = 0;
            back.y = 0;
            apex.y = 0;
            Vector3 apexDirection = (apex-front).normalized;
            Vector3 carDirection = (front - back).normalized;

            //use dot product to calculate the difference between the direction the car is pointing and the direction of the front of the car to the apex of the corner.
            float direction = Mathf.Clamp01(Vector3.Dot(apexDirection, carDirection) * 2f - 0.25f);
            // float distanceToApex = Vector3.Distance(front, apex);
            // float evaluatedDistanceToApex =  1.0f-Mathf.Clamp01((distanceToApex-2.0f)/10.0f);

            m_DriftMultiplier = 1 + direction * 2;

            if(m_IsComboing && m_CurrentComboName == "Drift" && direction > 0.25f) m_CurrentComboName = "Apex Drift";
        }

        m_DriftPoints += m_DriftMultiplier * Time.deltaTime * 0.25f;
        m_DriftMultiplier = 1.0f;

        if(!m_IsComboing && m_DriftDuration > m_MinimumDriftDuration)
        {
            // start combo
            m_CurrentComboName = "Drift";
            m_IsComboing = true;
            m_CurrentComboUI = GameObject.Instantiate(m_ComboUIPrefab, transform);
            OnCombo += m_CurrentComboUI.UpdateCombo;
            
        }
    }

    private void OnDisable()
    {
        CancelCombo(false);

    }

    public void CancelCombo(bool awardPoints)
    {
        if(m_CurrentComboUI != null)
        {
                OnCombo -= m_CurrentComboUI.UpdateCombo;
                m_CurrentComboUI.Disappear();
                Destroy(m_CurrentComboUI.gameObject, 0.25f);
        }
        m_IsComboing = false;
        m_DriftPoints = 0.0f;
        m_DriftMultiplier = 1.0f;
        m_DriftDuration = 0.0f;
    }

    public void SetCorner(Corner c)
    {
        m_Corner = c;
    }
}
