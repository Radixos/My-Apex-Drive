using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour
{
    [SerializeField] private SplineMesh.Spline m_Spline;

    public Vector3 GetNearestPointOnSpline(Vector3 v)
    {
        return m_Spline.GetNearestPointOnSpline(v);
    }

    private void OnTriggerEnter(Collider other)
    {
        ComboAnalyser ca = other.GetComponent<ComboAnalyser>();
        if (ca != null) ca.SetCorner(this);
    }

    private void OnTriggerExit(Collider other)
    {
        ComboAnalyser ca = other.GetComponent<ComboAnalyser>();
        if(ca != null) ca.SetCorner(null);
    }
}
