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

	public float TotalTrackLength;
	public float[] SegmentLengths;

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

		TotalTrackLength = 0.0f;
		SegmentLengths = new float[Segments.Length];
		for(int i = 0; i < Segments.Length; i++)
		{
			SegmentLengths[i] = Segments[i].ArcLength;
			Segments[i].DistanceOnTrackBeforeCurrentSegment = TotalTrackLength;
			TotalTrackLength += Segments[i].ArcLength;
		}
	}

	public Vector3 GetNearestPositionOnSpline(Vector3 point, int steps, int depth)
	{
		float shortestDistance = float.MaxValue;
		RoadSegment nearest = null;

		foreach(RoadSegment segment in Segments)
		{
			Vector3 position = segment.transform.position;
			float distance = Vector3.Distance(position, point);
			if(distance < shortestDistance) 
			{
				shortestDistance = distance;
				nearest = segment;
			} 
		}
		
		// THIS COULD BE CLEANED UP
		float t1 = nearest.GetBezierRepresentation(Space.World).GetClosestTimeToPoint(point, steps, depth);
		RoadSegment previous = nearest.TryGetPreviousSegment();
		float t2 = float.MaxValue;
		if(previous != null) t2 = previous.GetBezierRepresentation(Space.World).GetClosestTimeToPoint(point, steps, depth);

		Vector3 p1 = nearest.GetBezierRepresentation(Space.World).GetPoint(t1);
		Vector3 p2 = previous.GetBezierRepresentation(Space.World).GetPoint(t2);
		if(Vector3.Distance(p1, point) < Vector3.Distance(p2, point)) return p1;
		else return p2;
	}

	public float GetNearestTimeOnSpline(Vector3 point, int steps, int depth)
	{
		float shortestDistance = float.MaxValue;
		RoadSegment nearest = null;

		foreach(RoadSegment segment in Segments)
		{
			Vector3 position = segment.transform.position;
			float distance = Vector3.Distance(position, point);
			if(distance < shortestDistance) 
			{
				shortestDistance = distance;
				nearest = segment;
			} 
		}

		// THIS COULD BE CLEANED UP
		float t1 = nearest.GetBezierRepresentation(Space.World).GetClosestTimeToPoint(point, steps, depth);
		RoadSegment previous = nearest.TryGetPreviousSegment();
		float t2 = float.MaxValue;
		if(previous != null) t2 = previous.GetBezierRepresentation(Space.World).GetClosestTimeToPoint(point, steps, depth);

		Vector3 p1 = nearest.GetBezierRepresentation(Space.World).GetPoint(t1);
		Vector3 p2 = previous.GetBezierRepresentation(Space.World).GetPoint(t2);

		t1 = ((t1 * nearest.ArcLength) + nearest.DistanceOnTrackBeforeCurrentSegment) / TotalTrackLength;
		t2 = ((t2 * previous.ArcLength) + previous.DistanceOnTrackBeforeCurrentSegment) / TotalTrackLength;

		if(Vector3.Distance(p1, point) < Vector3.Distance(p2, point)) return t1;
		else return t2;
	}


	public float GetProgress(Vector3 point)
	{
		return 0.0f;
	}

	public OrientedPoint Evaluate(float t)
	{
		if(SegmentLengths.Length != Segments.Length) UpdateMeshes();
		t = Mathf.Clamp01(t) % 1.0f;
		t *= TotalTrackLength;

	
		int index = 0;
		float x = 0;
		while(x + SegmentLengths[index] < t)
		{
			x += SegmentLengths[index];
			if(x < t) index += 1;
		}

		float remainder = t - x;
		float t2 = (remainder / SegmentLengths[index]) % 1.0f;
		return Segments[index].Evaluate(t2, Space.World);
	}
}
