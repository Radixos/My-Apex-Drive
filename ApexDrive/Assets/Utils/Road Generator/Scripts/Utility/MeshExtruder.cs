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

using System.Collections.Generic;
using UnityEngine;

// This class contains the heart of the spline extrusion code!
// You provide data and a mesh, and this will write to that mesh for you!
public class MeshExtruder {

	// Used when generating the mesh
	List<Vector3> verts = new List<Vector3>();
	List<Vector3> normals = new List<Vector3>();
	List<Vector2> uvs0 = new List<Vector2>();
	List<Vector2> uvs1 = new List<Vector2>();
	List<int> triIndices = new List<int>();

	
	public void Extrude( Mesh mesh, Mesh2D mesh2D, OrientedCubicBezier3D bezier, Ease rotationEasing, UVMode uvMode, Vector2 nrmCoordStartEnd, float edgeLoopsPerMeter, float tilingAspectRatio ) {

		// Clear all data. This could be optimized by using arrays and only reconstruct when lengths change
		mesh.Clear();
		verts.Clear();
		normals.Clear();
		uvs0.Clear();
		uvs1.Clear();
		triIndices.Clear();

		// UVs/Texture fitting
		LengthTable table = null;
		if(uvMode == UVMode.TiledDeltaCompensated)
			table = new LengthTable( bezier, 12 );
		float curveArcLength = bezier.GetArcLength();
		
		// Tiling
		float tiling = tilingAspectRatio;
		if( uvMode == UVMode.Tiled || uvMode == UVMode.TiledDeltaCompensated ) {
			float uSpan = mesh2D.CalcUspan(); // World space units covered by the UVs on the U axis
			tiling *= curveArcLength / uSpan;
			tiling = Mathf.Max( 1, Mathf.Round( tiling ) ); // Snap to nearest integer to tile correctly
		}

		// Generate vertices
		// Foreach edge loop
		// Calc edge loop count
		int targetCount = Mathf.RoundToInt( curveArcLength * edgeLoopsPerMeter );
		int edgeLoopCount = Mathf.Max( 2, targetCount ); // Make sure it's at least 2
		for( int ring = 0; ring < edgeLoopCount; ring++ ) {
			float t = ring / (edgeLoopCount-1f);
			OrientedPoint op = bezier.GetOrientedPoint( t, rotationEasing );

			// Prepare UV coordinates. This branches lots based on type
			float tUv = t;
			if( uvMode == UVMode.TiledDeltaCompensated )
				tUv = table.TToPercentage( tUv );
			float uv0V = tUv * tiling;
			float uv1U = Mathf.Lerp( nrmCoordStartEnd.x, nrmCoordStartEnd.y, tUv ); // Normalized coordinate for entire chain

			// Foreach vertex in the 2D shape
			for( int i = 0; i < mesh2D.VertexCount; i++ ) {
				verts.Add( op.LocalToWorldPos( mesh2D.vertices[i].point ) );
				normals.Add( op.LocalToWorldVec( mesh2D.vertices[i].normal ) );
				uvs0.Add( new Vector2( mesh2D.vertices[i].u, uv0V ) );
				uvs1.Add( new Vector2( uv1U, 0 ) );
			}

		}

		// Generate Trianges
		// Foreach edge loop (except the last, since this looks ahead one step)
		for( int edgeLoop = 0; edgeLoop < edgeLoopCount - 1; edgeLoop++ ) {
			int rootIndex = edgeLoop * mesh2D.VertexCount;
			int rootIndexNext = (edgeLoop+1) * mesh2D.VertexCount;

			// Foreach pair of line indices in the 2D shape
			for( int line = 0; line < mesh2D.LineCount; line += 2 ) {
				int lineIndexA = mesh2D.lineIndices[line];
				int lineIndexB = mesh2D.lineIndices[line+1];
				int currentA = rootIndex + lineIndexA;
				int currentB = rootIndex + lineIndexB;
				int nextA = rootIndexNext + lineIndexA;
				int nextB = rootIndexNext + lineIndexB;
				triIndices.Add( currentA );
				triIndices.Add( nextA );
				triIndices.Add( nextB );
				triIndices.Add( currentA );
				triIndices.Add( nextB );
				triIndices.Add( currentB );
			}
		}

		// Assign it all to the mesh
		mesh.SetVertices( verts );
		mesh.SetNormals( normals );
		mesh.SetUVs( 0, uvs0 );
		mesh.SetUVs( 1, uvs1 );
		mesh.SetTriangles( triIndices, 0 );

	}


}
