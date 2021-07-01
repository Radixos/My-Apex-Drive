using System.Collections.Generic;
using UnityEngine;

public class CubicBezierSpline
{
    public List<CubicBezierSplineNode> Nodes;

    public void AddNode(Vector3 position)
    {
        if(Nodes == null) Nodes = new List<CubicBezierSplineNode>();
        CubicBezierSplineNode node = new CubicBezierSplineNode(position, Quaternion.identity, 5.0f);
    }
}
