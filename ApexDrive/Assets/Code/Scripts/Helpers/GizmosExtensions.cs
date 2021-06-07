using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawWireCircle( Vector3 position, Quaternion rotation, float radius, int detail = 32)
    {
        Vector3[] points = new Vector3[detail];
        for(int i = 0; i < detail; i++)
        {
            float t = (float)i / (float)detail;
            float angle = t * MathfExtensions.TAU;

            points[i] = MathfExtensions.GetVectorByAngle(angle) * radius;
        }
        for(int i = 0; i < detail-1; i++)
        {
            Gizmos.DrawLine(points[i], points[i+1]);
        }
        Gizmos.DrawLine(points[0], points[detail - 1]);
    }
}
