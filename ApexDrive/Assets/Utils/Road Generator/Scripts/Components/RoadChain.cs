//
// Dedicated to all my Patrons on Patreon,
// as a thanks for your continued support 💖
//
// Source code © Freya Holmér, 2019
// This code is provided exclusively to supporters,
// under the Attribution Assurance License
// "https://tldrlegal.com/license/attribution-assurance-license-(aal)"
// 
// You can basically do whatever you want with this code,
// as long as you include this license and credit me for it,
// in both the source code and any released binaries using this code
//
// Thank you so much again <3
//
// Freya
//

using UnityEngine;
using System.Linq;

// This is the parent container for all the road segments
[ExecuteInEditMode]
public class RoadChain : MonoBehaviour {

	public GameObject RoadPrefab;
	public Mesh2D mesh2D = null; // The 2D shape to be extruded
	public bool loop = false; // Whether or not the last segment should connect to the first
	public float edgeLoopsPerMeter = 2; // Triangle density, in loops per meter!
	public float ColliderEdgeLoopsPerMeter = 2; // Triangle density, in loops per meter!
	public bool GenerateColliders = true;
	public UVMode uvMode = UVMode.TiledDeltaCompensated; // More info on what this is in the enum!
	public RoadSegment[] Segments;

	public Transform TestNearestPointObject;
	public Transform TestNearestPointObject2;

	// Regenerate mesh on instantiation.
	// If you save the mesh in the scene you don't have to do this, but, it's pretty fast anyway so whatevs!
	private void Awake() 
	{
		UpdateMeshes();
	}

	private void Update()
	{
		if(TestNearestPointObject != null) GetNearestPositionOnSpline(TestNearestPointObject.position, 100, true);
		if(TestNearestPointObject2 != null) GetNearestPositionOnSpline(TestNearestPointObject2.position, 100, true);
	}

	// Iterates through all children / road segments, and updates their meshes!
	public void UpdateMeshes() {

		Segments = GetComponentsInChildren<RoadSegment>();
		RoadSegment[] segmentsWithMesh = Segments.Where( s => s.HasValidNextPoint ).ToArray();
		RoadSegment[] segmentsWithoutMesh = Segments.Where( s => s.HasValidNextPoint == false ).ToArray();

		// We calculate the total length of the road, in order for us to be able to supply a normalized
		// coordinate for how far along the track you are, where
		// 0 = start of the track
		// 0.5 = halfway through the track
		// 1.0 = end of the track
		float[] lengths = segmentsWithMesh.Select( x => x.GetBezierRepresentation( Space.Self ).GetArcLength() ).ToArray();
		float totalRoadLength = lengths.Sum();

		float startDist = 0f;
		for( int i = 0; i < segmentsWithMesh.Length; i++ ) {
			float endDist = startDist + lengths[i];
			Vector2 uvzStartEnd = new Vector2(
				startDist / totalRoadLength,		// Percentage along track start
				endDist / totalRoadLength			// Percentage along track end
			);
			segmentsWithMesh[i].UpdateMesh( uvzStartEnd );
			startDist = endDist;
		}

		// Clear all segments without meshes
		foreach( RoadSegment seg in segmentsWithoutMesh ) {
			seg.UpdateMesh( Vector2.zero );
		}
	}

	public RoadSegment GetNearestSegmentToPoint(Vector3 point, bool ignoreHeight = false)
	{
		float shortestDistance = -1.0f;
		RoadSegment result = null;
		if(ignoreHeight) point.y = 0.0f;
		foreach(RoadSegment segment in Segments)
		{
			Vector3 position = segment.transform.position;
			if(ignoreHeight) position.y = 0.0f;
			if(shortestDistance < 0.0f || Vector3.Distance(position, point) < shortestDistance) 
			{
				shortestDistance = Vector3.Distance(segment.transform.position, point);
				result = segment;
			} 
		}
		return result;
	}

	///<param name="precision">The number of subdivisions to check between road segments for the nearest point. Lower number = more performant, Higher number = more precise.</param>
	public Vector3 GetNearestPositionOnSpline(Vector3 point, int precision, bool ignoreHeight = false)
	{
		if(ignoreHeight) point.y = 0.0f;
		Vector3 result = Vector3.zero;
		float lowestDistance = -1.0f;
		RoadSegment segment = GetNearestSegmentToPoint(point);
		OrientedCubicBezier3D bezierA = segment.GetBezierRepresentation(Space.World), bezierB = segment.TryGetPreviousSegment().GetBezierRepresentation(Space.World);
		for(int i = 0; i < precision; i++)
		{
			Vector3 bezierPoint = bezierA.GetPoint((float)i/(float)precision);
			if(ignoreHeight) bezierPoint.y = 0.0f;
			float distance = Vector3.Distance(bezierPoint, point);
			if(lowestDistance < 0.0f || distance < lowestDistance)
			{
				result = bezierPoint;
				lowestDistance = distance;
			}

			bezierPoint = bezierB.GetPoint((float)i/(float)precision);
			if(ignoreHeight) bezierPoint.y = 0.0f;
			distance = Vector3.Distance(bezierPoint, point);
			if(lowestDistance < 0.0f || distance < lowestDistance)
			{
				result = bezierPoint;
				lowestDistance = distance;
			}
		}
		Debug.DrawLine(result, point, Color.red);
		return result;
	}
}
