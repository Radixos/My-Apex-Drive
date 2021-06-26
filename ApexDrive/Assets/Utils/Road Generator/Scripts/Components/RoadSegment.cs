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
using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider)), ExecuteInEditMode]
public class RoadSegment : MonoBehaviour {

	[HideInInspector][SerializeField] int ownerID;

	private MeshCollider m_Collider;
	private MeshFilter m_Filter;

	private void Awake() => ValidateComponents();


	private Mesh m_Mesh; // The actual mesh asset to generate into
	private Mesh Mesh {
		get {
			if(m_Collider.sharedMesh == m_Filter.sharedMesh)
			{
				DestroyImmediate(m_Filter.sharedMesh);
			}
			bool isOwner = ownerID == gameObject.GetInstanceID();
			bool filterHasMesh = m_Filter.sharedMesh != null;
			if( !filterHasMesh || !isOwner) {
				m_Filter.sharedMesh = m_Mesh = new Mesh(); // Create new mesh and assign to the mesh filter
				ownerID = gameObject.GetInstanceID(); // Mark self as owner of this mesh
				m_Mesh.name = "Mesh [" + ownerID + "]";
			} else if( isOwner && filterHasMesh && m_Mesh == null ) {
				// If the mesh field lost its reference, which can happen in assembly reloads
				m_Mesh = m_Filter.sharedMesh;
			}
			return m_Mesh;
		}
	}

	private Mesh m_ColliderMesh;
	private Mesh ColliderMesh{
		get{
			if(m_Collider.sharedMesh == m_Filter.sharedMesh)
			{
				DestroyImmediate(m_Collider.sharedMesh);
			}
			bool isOwner = ownerID == gameObject.GetInstanceID();
			bool colliderHasMesh = m_Collider.sharedMesh != null;
			if(!colliderHasMesh || !isOwner)
			{
				m_Collider.sharedMesh = m_ColliderMesh = new Mesh();
				ownerID = gameObject.GetInstanceID();
				m_ColliderMesh.name = "Collider Mesh ["+ ownerID +"]";
			}
			else if(isOwner && colliderHasMesh && m_ColliderMesh == null)
			{
				m_ColliderMesh = m_Collider.sharedMesh;
			}
			return m_ColliderMesh;
		}
	}

	// Serialized stuff, like settings
	public float tangentLength = 3; // Tangent size. Note that it's only the tangent of the first point. The next segment controls the endpoint tangent length
	public Ease rotationEasing = Ease.InOut;
	
	// Non-serialized stuff
	MeshExtruder meshExtruder = new MeshExtruder();

	// Properties
	public bool HasValidNextPoint => TryGetNextSegment() != null;
	bool IsInValidChain => transform.parent.Ref()?.GetComponent<RoadChain>() != null;
	public RoadChain RoadChain => transform.parent == null ? null : transform.parent.GetComponent<RoadChain>();
	Mesh2D Mesh2D => RoadChain.mesh2D;

	private void ValidateComponents()
	{
		if(m_Collider == null) m_Collider = GetComponent<MeshCollider>();
		if(m_Collider == null) m_Collider = gameObject.AddComponent<MeshCollider>();

		if(m_Filter == null) m_Filter = GetComponent<MeshFilter>();
		if(m_Filter == null) m_Filter = gameObject.AddComponent<MeshFilter>();

		if(GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();
	}

	// This will regenerate the mesh!
	// uvzStartEnd is used for the (optional) normalized coordinates along the whole track,
	// x = start coordinate, y = end coordinate
	public void UpdateMesh( Vector2 nrmCoordStartEnd ) {
		ValidateComponents();
		// Only generate a mesh if we've got a next control point
		if( HasValidNextPoint ) 
		{
			meshExtruder.Extrude(
				mesh: Mesh,
				mesh2D: RoadChain.mesh2D,
				bezier: GetBezierRepresentation( Space.Self ),
				rotationEasing: rotationEasing,
				uvMode: RoadChain.uvMode,
				nrmCoordStartEnd: nrmCoordStartEnd,
				edgeLoopsPerMeter: RoadChain.edgeLoopsPerMeter,
				tilingAspectRatio: GetTextureAspectRatio()
			);

			meshExtruder.Extrude(
				mesh: ColliderMesh,
				mesh2D: RoadChain.mesh2D,
				bezier: GetBezierRepresentation(Space.Self),
				rotationEasing: rotationEasing,
				edgeLoopsPerMeter: RoadChain.ColliderEdgeLoopsPerMeter
			);
		} 
		else 
		{
			if( m_Mesh != null ) DestroyImmediate( m_Mesh );
			if( m_ColliderMesh != null ) DestroyImmediate( m_ColliderMesh );
		}

	}

	float GetTextureAspectRatio() {
		Texture texture = GetComponent<MeshRenderer>().sharedMaterial.Ref()?.mainTexture;
		return texture != null ? texture.AspectRatio() : 1f;
	}

	// Gets one of the 4 bezier control point locations
	// This is a bit convoluted to avoid double-transforming between spaces
	public Vector3 GetControlPoint( int i, Space space ) {
		Vector3 FromLocal( Vector3 localPos ) => space == Space.Self ? localPos : transform.TransformPoint( localPos );
		Vector3 FromWorld( Vector3 worldPos ) => space == Space.World ? worldPos : transform.InverseTransformPoint( worldPos );
		if( i < 2 ) {
			if( i == 0 )
				return FromLocal( Vector3.zero );
			if( i == 1 )
				return FromLocal( Vector3.forward * tangentLength );
		} else {
			RoadSegment next = TryGetNextSegment();
			Transform nextTf = next.transform;
			if( i == 2 )
				return FromWorld( nextTf.TransformPoint( Vector3.back * next.tangentLength ) );
			if( i == 3 )
				return FromWorld( nextTf.position );
		}
		return default;
	}
	 
	// Gives you the next road segment, if it exists
	// It also branches based on whether or not this whole road forms a loop
	public RoadSegment TryGetNextSegment() {
		if( IsInValidChain == false )
			return null;
		int thisIndex = transform.GetSiblingIndex();
		bool isLast = thisIndex == transform.parent.childCount-1;
		RoadSegment GetSiblingSegment( int i ) => transform.parent.GetChild( i ).GetComponent<RoadSegment>();
		if( isLast && RoadChain.loop )
			return GetSiblingSegment( 0 ); // First segment
		else if( !isLast )
			return GetSiblingSegment( thisIndex + 1 ); // Next segment
		return null;
	}

	public RoadSegment TryGetPreviousSegment() {
		if( IsInValidChain == false )
			return null;
		int thisIndex = transform.GetSiblingIndex();
		bool isFirst = thisIndex == 0;
		RoadSegment GetSiblingSegment( int i ) => transform.parent.GetChild( i ).GetComponent<RoadSegment>();
		if( isFirst && RoadChain.loop )
			return GetSiblingSegment( transform.parent	.childCount - 1 ); // Previous segment
		else if( !isFirst )
			return GetSiblingSegment( thisIndex - 1 ); // Previous segment
		return null;
	}

	// Returns the oriented béziér representation of this segment
	// We need this in both world and local space! Meshes are in local space,
	// while gizmos/handles are in world space. This is used by the RoadSegmentInspector
	public OrientedCubicBezier3D GetBezierRepresentation( Space space ) {
		return new OrientedCubicBezier3D(
			GetUpVector( 0, space ),
			GetUpVector( 3, space ),
			GetControlPoint( 0, space ),
			GetControlPoint( 1, space ),
			GetControlPoint( 2, space ),
			GetControlPoint( 3, space )
		);
	}

	public Vector3 GetClosestPoint(Vector3 point, int steps)
	{
		steps -= 1;
		Vector3 result = Vector3.zero;
		OrientedCubicBezier3D bezier = GetBezierRepresentation(Space.World);
		for(int i = 0; i <= steps; i++)
		{
			Debug.DrawLine(bezier.GetPoint((float)i / (float)steps), point, Color.red);
		}
		return result;
	}

	public OrientedPoint Evaluate(float t, Space space)
	{
		return GetBezierRepresentation(space).GetOrientedPoint(t);
	}

	// Returns the up vector of either the first or last control point, in a given space
	Vector3 GetUpVector( int i, Space space ) {
		if( i == 0 ) {
			return space == Space.Self ? Vector3.up : transform.up;
		} else if( i == 3 ) {
			Vector3 wUp = TryGetNextSegment().transform.up;
			return space == Space.World ? wUp : transform.InverseTransformVector( wUp );
		}
		return default;
	}

}
