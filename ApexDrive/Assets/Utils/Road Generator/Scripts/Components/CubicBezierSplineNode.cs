using UnityEngine;

public class CubicBezierSplineNode
{
    public Vector3 Position;
    public Quaternion Rotation;
    public float TangentLength;

    public CubicBezierSplineNode NextNode;
    public CubicBezierSplineNode PreviousNode;

    public CubicBezierSplineNode(Vector3 position, Quaternion rotation, float tangentLength)
    {
        Position = position;
        Rotation = rotation;
        TangentLength = tangentLength;
    }

    public CubicBezierSplineNode(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public CubicBezierSplineNode(Vector3 position)
    {
        Position = position;
    }
}
