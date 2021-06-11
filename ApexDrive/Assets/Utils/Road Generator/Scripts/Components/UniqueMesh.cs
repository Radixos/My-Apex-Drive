﻿//
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

// This class is used to make sure we are the owner of this mesh.
// This is to prevent this issue:
// 1. Duplicate exsting object
// 2. The new object now refers to the same mesh *asset* as the source of the duplicate
// 3. They will now both edit the same mesh, so we have to make sure the new object doesn't use the source mesh
// We solve this by keeping track of the ID of the mesh that created it. If ownerID is from a different object,
// it means this object was duplicated from another, and we need to create a new mesh for this copy
[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshCollider ) )]
public class UniqueMesh : MonoBehaviour {

	[HideInInspector][SerializeField] int ownerID;
	private MeshCollider m_MeshCollider;
	MeshFilter MeshFilter => GetComponent<MeshFilter>();
	MeshCollider MeshCollider => GetComponent<MeshCollider>();

	protected Mesh meshCached; // The actual mesh asset to generate into
	protected Mesh Mesh {
		get {
			bool isOwner = ownerID == gameObject.GetInstanceID();
			bool filterHasMesh = MeshFilter.sharedMesh != null;
			if(MeshCollider == null) gameObject.AddComponent<MeshCollider>();
			bool colliderHasMesh = MeshCollider.sharedMesh != null;
			if( !filterHasMesh || !isOwner || !colliderHasMesh || MeshCollider.sharedMesh != MeshFilter.sharedMesh ) {
				MeshCollider.sharedMesh = MeshFilter.sharedMesh = meshCached = new Mesh(); // Create new mesh and assign to the mesh filter
				ownerID = gameObject.GetInstanceID(); // Mark self as owner of this mesh
				meshCached.name = "Mesh [" + ownerID + "]";
				// meshCached.
				// meshCached.hideFlags = HideFlags.HideAndDontSave; // Ensures it isn't saved in the scene. This will prevent leaks
				// meshCached.MarkDynamic(); // Only useful for real-time bending. Don't do this if you only generate one
			} else if( isOwner && filterHasMesh && meshCached == null ) {
				// If the mesh field lost its reference, which can happen in assembly reloads
				meshCached = MeshFilter.sharedMesh;
			}
			return meshCached;
		}
	}

	// protected Mesh colliderMeshCached;
	// protected Mesh ColliderMesh {
	// 	get {
	// 		bool isOwner = ownerID == gameObject.GetInstanceID();
	// 		if(m_MeshCollider == null) m_MeshCollider = GetComponent<MeshCollider>();
	// 		if(m_MeshCollider == null) m_MeshCollider = gameObject.AddComponent<MeshCollider>();
	// 		bool colliderHasMesh = m_MeshCollider.sharedMesh != null;
	// 		if( !colliderHasMesh || !isOwner ) {
	// 			m_MeshCollider.sharedMesh = colliderMeshCached = new Mesh(); // Create new mesh and assign to the mesh filter
	// 			ownerID = gameObject.GetInstanceID(); // Mark self as owner of this mesh
	// 			meshCached.name = "Collider Mesh [" + ownerID + "]";
	// 			// colliderMeshCached.hideFlags = HideFlags.HideAndDontSave; // Ensures it isn't saved in the scene. This will prevent leaks
	// 			// meshCached.MarkDynamic(); // Only useful for real-time bending. Don't do this if you only generate one
	// 		} else if( isOwner && colliderHasMesh && colliderMeshCached == null ) {
	// 			// If the mesh field lost its reference, which can happen in assembly reloads
	// 			colliderMeshCached = m_MeshCollider.sharedMesh;
	// 		}
	// 		return colliderMeshCached;
	// 	}
	// }

	
}
