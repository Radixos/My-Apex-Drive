using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadChain))]
public class RoadChainEditor : Editor
{
    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;
    private static GUIStyle SubheadingStyle = null;
    private static GUIStyle ParagraphStyle = null;

    private RoadChain m_RoadChain;
    private RoadSegment m_SelectedSegment;

    private Texture2D m_AddIcon;
    private Texture2D m_DeleteIcon;
    private Texture2D m_SelectIcon;
    private Texture2D m_BezierControlIcon;
    private Texture2D m_RotationIcon;
    private Texture2D m_MoveIcon;
    private Texture2D m_BeginningIcon;
    private Texture2D m_EndIcon;

    private enum ToolMode {Add, Delete, Selection, BezierControl, Rotation, Move}
    private ToolMode m_ActiveTool = ToolMode.Selection;

    private Tool m_PreviousEditorTool;

    private bool m_AddToEnd = true;
    private bool m_MovingSegment = false;
    private bool m_RotatingSegment = false;
    private bool m_ModifyingBezier = false;

    private float m_EvaluationTest;

    private Quaternion m_OriginalCameraOrientation;
    private bool m_OriginalCameraOrthographicProjection;

    private bool m_ViewStats = false;

    private void Awake()
    {
        m_RoadChain = (RoadChain) target;
        foreach(RoadSegment segment in m_RoadChain.Segments)
        {
            segment.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    private void OnEnable()
    {
        Tools.hidden = true;
        m_PreviousEditorTool = Tools.current;
        Tools.current = Tool.None;

        m_RoadChain = (RoadChain) target;

        m_AddIcon = (Texture2D) EditorGUIUtility.Load("Add.png");
        m_DeleteIcon = (Texture2D) EditorGUIUtility.Load("Delete.png");
        m_MoveIcon = (Texture2D) EditorGUIUtility.Load("Move.png");
        m_SelectIcon = (Texture2D) EditorGUIUtility.Load("SelectNode.png");
        m_RotationIcon = (Texture2D) EditorGUIUtility.Load("Rotation.png");
        m_BezierControlIcon = (Texture2D) EditorGUIUtility.Load("BezierRotation.png");
        m_BeginningIcon = (Texture2D) EditorGUIUtility.Load("Beginning.png");
        m_EndIcon = (Texture2D) EditorGUIUtility.Load("End.png");

        foreach(RoadSegment segment in m_RoadChain.Segments)
        {
            segment.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    private void OnDisable()
    {
        Tools.hidden = false;
        Tools.current = m_PreviousEditorTool;
    }

    private void OnSceneGUI()
    {
        // Disable scene view deselect on click
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }

        // Set up GUISTyles for toggle buttons 
        if (ToggleButtonStyleNormal == null || ToggleButtonStyleToggled == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button.normal.background;
            
        }

        Handles.BeginGUI();
        DrawToolMenu();
        Handles.EndGUI();
        DrawRoadHandles();

        ProcessActiveTool();
        EditorUtility.SetDirty(m_RoadChain);
        SceneView.RepaintAll();
    }

    public override void OnInspectorGUI()
    {
        if (SubheadingStyle == null || ParagraphStyle == null)
        {
            SubheadingStyle = new GUIStyle();
            SubheadingStyle.fontSize = 16;
            SubheadingStyle.fontStyle = FontStyle.Bold;
            SubheadingStyle.normal.textColor = Color.white;
            ParagraphStyle = new GUIStyle();
            ParagraphStyle.normal.textColor = Color.white;
            ParagraphStyle.wordWrap = true;
        }
        GUILayoutOption height  = GUILayout.Height(40.0f);
        GUILayoutOption width = GUILayout.Width(40.0f);
        GUILayout.BeginVertical("Box");
            GUILayout.Label("Road Generator", SubheadingStyle);
            GUILayout.Space(5.0f);

            GUILayout.Label(
            "This is a tool to quickly generate tracks. Currently there is an issue if you try to use Control-Z to undo anything, it will prevent interaction with the node editing gizmos. If this happens just close and re-open Unity. " +
            "Below is a key for all of the tools in the scene view."
            , ParagraphStyle);

            GUILayout.Space(10.0f);
            GUILayout.Label("Tools", SubheadingStyle);
            GUILayout.Space(5.0f);

            GUILayout.BeginHorizontal();
            GUILayout.Button(m_SelectIcon, height, width);
            GUILayout.Label("Select Node", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40.0f);
            GUILayout.Button(m_MoveIcon, height, width);
            GUILayout.Label("Move Node", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40.0f);
            GUILayout.Button(m_BezierControlIcon, height, width);
            GUILayout.Label("Segment Length", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40.0f);
            GUILayout.Button(m_RotationIcon, height, width);
            GUILayout.Label("Segment Orientation", height);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Button(m_AddIcon, height, width);
            GUILayout.Label("Add Node", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40.0f);
            GUILayout.Button(m_BeginningIcon, height, width);
            GUILayout.Label("Add Node At Beginning", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40.0f);
            GUILayout.Button(m_EndIcon, height, width);
            GUILayout.Label("Add Node At End", height);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Button(m_DeleteIcon, height, width);
            GUILayout.Label("Delete Node", height);
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(10.0f);
        m_RoadChain.RoadPrefab = (GameObject)EditorGUILayout.ObjectField("Road Segment Prefab", m_RoadChain.RoadPrefab, typeof(GameObject), false);
        m_RoadChain.mesh2D =  (Mesh2D)EditorGUILayout.ObjectField("Mesh 2D", m_RoadChain.mesh2D, typeof(Mesh2D), false);
        bool wasLooped = m_RoadChain.loop;
        m_RoadChain.loop = EditorGUILayout.Toggle("Loop", m_RoadChain.loop);
        float initialEdgeLoopCount = m_RoadChain.edgeLoopsPerMeter;
        m_RoadChain.edgeLoopsPerMeter = EditorGUILayout.Slider("Edge Loops Per Meter", m_RoadChain.edgeLoopsPerMeter, 0.25f, 1.0f);

        float initialColliderEdgeLoopCount = m_RoadChain.ColliderEdgeLoopsPerMeter;
        m_RoadChain.GenerateColliders = EditorGUILayout.Toggle("Generate Colliders", m_RoadChain.GenerateColliders);
        if(m_RoadChain.GenerateColliders) m_RoadChain.ColliderEdgeLoopsPerMeter = EditorGUILayout.Slider("Collider Edge Loops Per Meter", m_RoadChain.ColliderEdgeLoopsPerMeter, 0.25f, 1.0f);
        

        if(GUILayout.Button("Generate Mesh"))
        {
            UpdateMeshes();
        }
        GUILayout.BeginVertical("Box");
        GUILayout.Label(
            "If collisions aren't working properly with the track try this:"
            , ParagraphStyle);
        if(GUILayout.Button("Force Road Colliders On"))
        {
            foreach(RoadSegment segment in m_RoadChain.Segments) segment.GetComponent<Collider>().enabled = true;
        }

        GUILayout.EndVertical();

        EditorGUILayout.BeginFoldoutHeaderGroup(m_ViewStats, "Extra Info");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField(m_RoadChain.TotalTrackLength);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SegmentLengths"));
        foreach(RoadSegment segment in m_RoadChain.Segments) EditorGUILayout.FloatField(segment.DistanceOnTrackBeforeCurrentSegment);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndFoldoutHeaderGroup();
        if(initialEdgeLoopCount != m_RoadChain.edgeLoopsPerMeter || (initialColliderEdgeLoopCount != m_RoadChain.ColliderEdgeLoopsPerMeter && m_RoadChain.GenerateColliders ) || m_RoadChain.loop != wasLooped) UpdateMeshes();
        
    }

    private void UpdateMeshes()
    {
        m_RoadChain.UpdateMeshes();
        EditorUtility.SetDirty(m_RoadChain);
        foreach(RoadSegment segment in m_RoadChain.Segments) EditorUtility.SetDirty(segment);
    }

    private void DrawRoadHandles()
    {
        Handles.color = Color.cyan;
        int handleID = GUIUtility.GetControlID(FocusType.Passive);
        for(int i = 0; i < m_RoadChain.Segments.Length; i++)
        {
            RoadSegment segment = m_RoadChain.Segments[i];
            if(segment != m_SelectedSegment)
                Handles.SphereHandleCap(handleID, segment.transform.position, Quaternion.identity, 1.0f, EventType.Repaint);
            else
                Handles.SphereHandleCap(handleID, segment.transform.position, Quaternion.identity, 1.5f, EventType.Repaint);

        }
        
        for(int i = 0; i < m_RoadChain.Segments.Length - (m_RoadChain.loop ? 0 : 1); i++)
        {
            RoadSegment segment = m_RoadChain.Segments[i];
            Handles.DrawBezier(segment.GetControlPoint(0, Space.World),segment.GetControlPoint(3, Space.World),segment.GetControlPoint(1, Space.World),segment.GetControlPoint(2, Space.World), Color.cyan, Texture2D.whiteTexture, 2.0f);
        }
    }

    private void DrawToolMenu()
    {

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool activeButton = (m_ActiveTool == ToolMode.Selection || m_ActiveTool == ToolMode.Move || m_ActiveTool == ToolMode.BezierControl || m_ActiveTool == ToolMode.Rotation);
        if(GUILayout.Button(m_SelectIcon, activeButton ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40)))
        {
            m_ActiveTool = ToolMode.Selection;
            return;
        }
        activeButton = m_ActiveTool == ToolMode.Add;
        if(GUILayout.Button(m_AddIcon, activeButton ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40)))
        {
            m_ActiveTool = ToolMode.Add;
            return;
        }
        activeButton = m_ActiveTool == ToolMode.Delete;
        if(GUILayout.Button(m_DeleteIcon, activeButton ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40)))
        {
            m_ActiveTool = ToolMode.Delete;
            return;
        }
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // Add Nodes Submenu
        if(m_ActiveTool == ToolMode.Add)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button( m_BeginningIcon, !m_AddToEnd ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40) ) )
            {
                m_AddToEnd = false;
                return;
            }
            if (GUILayout.Button( m_EndIcon, m_AddToEnd ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40) ) )
            {
                m_AddToEnd = true;
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        // Transform Node Submenu
        else if((m_ActiveTool == ToolMode.Selection || m_ActiveTool == ToolMode.Move || m_ActiveTool == ToolMode.BezierControl || m_ActiveTool == ToolMode.Rotation) && m_SelectedSegment != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button( m_MoveIcon, m_ActiveTool == ToolMode.Move ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40) ) )
            {
                m_ActiveTool = ToolMode.Move;
                return;
            }
            if (GUILayout.Button( m_BezierControlIcon, m_ActiveTool == ToolMode.BezierControl ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40) ) )
            {
                m_ActiveTool = ToolMode.BezierControl;
                return;
            }
            if (GUILayout.Button( m_RotationIcon, m_ActiveTool == ToolMode.Rotation ? ToggleButtonStyleToggled : ToggleButtonStyleNormal, GUILayout.Width(40), GUILayout.Height(40) ) )
            {
                m_ActiveTool = ToolMode.Rotation;
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
         if(!(m_ActiveTool == ToolMode.Selection || m_ActiveTool == ToolMode.Move || m_ActiveTool == ToolMode.BezierControl || m_ActiveTool == ToolMode.Rotation)) m_SelectedSegment = null;
    }

    private void ProcessActiveTool()
    {
        Vector2 screenSpaceMousePosition = Event.current.mousePosition;
        RoadSegment nearestSegment = null;
        Vector2 nearestScreenSpacePoint = Vector2.zero;
        Vector3 nearestPoint = Vector3.zero;

        { // get nearest node
            float minimumDistance = -1.0f;
            for(int i = 0; i < m_RoadChain.Segments.Length; i++)
            {
                Vector2 screenSpaceSegmentPosition = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(m_RoadChain.Segments[i].transform.position);
                screenSpaceSegmentPosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - screenSpaceSegmentPosition.y;
                float distance = Vector2.Distance(screenSpaceSegmentPosition, screenSpaceMousePosition);
                if(minimumDistance < 0.0f || distance < minimumDistance )
                {
                    minimumDistance = distance;
                    nearestSegment = m_RoadChain.Segments[i];
                    nearestScreenSpacePoint = screenSpaceSegmentPosition;
                    nearestPoint = m_RoadChain.Segments[i].transform.position;
                }
            }
        }

        bool mousePositionInSceneView = true;
        if(screenSpaceMousePosition.x < 0 || screenSpaceMousePosition.y < 0 || screenSpaceMousePosition.y > Camera.current.pixelHeight || screenSpaceMousePosition.x > Camera.current.pixelWidth) mousePositionInSceneView = false;

        switch(m_ActiveTool)
        {
            case ToolMode.Add:
                if(!mousePositionInSceneView) return;
                Handles.color = new Color(0.0f, 1.0f, 1.0f, 0.5f);
                if(mousePositionInSceneView) Handles.DrawSolidDisc(screenSpaceMousePosition, Vector3.up, 0.4f);
                Handles.BeginGUI();

                if(m_RoadChain.Segments.Length > 0)
                {
                    int previousNodeIndex = m_AddToEnd ? m_RoadChain.Segments.Length - 1 : 0;
                    Vector2 previousNode = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(m_RoadChain.Segments[previousNodeIndex].transform.position);
                    previousNode.y = SceneView.lastActiveSceneView.camera.pixelHeight - previousNode.y;
                    Handles.DrawLine(previousNode, screenSpaceMousePosition);
                    Handles.SphereHandleCap(0, screenSpaceMousePosition, Quaternion.identity, 1.5f, EventType.Repaint);
                    if(m_RoadChain.loop) 
                    {
                        int nextNodeIndex = m_AddToEnd ? 0 : m_RoadChain.Segments.Length - 1;
                        Vector2 nextNode = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(m_RoadChain.Segments[nextNodeIndex].transform.position);
                        nextNode.y = SceneView.lastActiveSceneView.camera.pixelHeight - nextNode.y;
                        Handles.DrawLine(nextNode, screenSpaceMousePosition);
                    }
                    Handles.EndGUI();
                }
                if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Vector2 invertedYMousePosition = screenSpaceMousePosition;
                    invertedYMousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - invertedYMousePosition.y;
                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(invertedYMousePosition);
                    Plane horizontalPlane = new Plane(Vector3.up, m_RoadChain.transform.position);
                    float distance = 0.0f;
                    if(horizontalPlane.Raycast(ray, out distance)) 
                    {
                        GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(m_RoadChain.RoadPrefab);
                        g.name = "Control Point";
                        g.transform.position = ray.GetPoint(distance);
                        g.transform.SetParent(m_RoadChain.transform);
                        if(m_AddToEnd) g.transform.SetAsLastSibling();
                        else g.transform.SetAsFirstSibling();
                        UpdateMeshes();
                    }
                    
                }
                if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    m_ActiveTool = ToolMode.Selection;
                }
                Handles.color = Color.white;
                break;

            case ToolMode.Delete:
                if(m_RoadChain.Segments.Length > 0)
                {
                    if(Vector2.Distance(nearestScreenSpacePoint, screenSpaceMousePosition) < 100.0f)
                    {
                        Handles.color = Color.red;
                        Handles.SphereHandleCap(0, nearestPoint, Quaternion.identity, 1.5f, EventType.Repaint);
                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            DestroyImmediate(nearestSegment.gameObject);
                        }
                        UpdateMeshes();
                    }
                }
                break;

            case ToolMode.Selection:
                if(m_RoadChain.Segments.Length > 0)
                {
                    if(Vector2.Distance(nearestScreenSpacePoint, screenSpaceMousePosition) < 100.0f)
                    {
                        Handles.color = Color.cyan;
                        Handles.SphereHandleCap(0, nearestPoint, Quaternion.identity, 1.5f, EventType.Repaint);
                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            m_SelectedSegment = nearestSegment;
                            m_ActiveTool = ToolMode.Move;
                        }
                    }
                }
                break;

            case ToolMode.Move:
                if(m_RoadChain.Segments.Length > 0)
                {
                    if(m_SelectedSegment != null)
                    {
                        Vector3 startingPos = m_SelectedSegment.transform.position;
                        m_SelectedSegment.transform.position = Handles.PositionHandle(m_SelectedSegment.transform.position, Quaternion.identity);
                        if(m_SelectedSegment.transform.position != startingPos) UpdateMeshes();
                    }
                    if(nearestSegment != m_SelectedSegment && Vector2.Distance(nearestScreenSpacePoint, screenSpaceMousePosition) < 100.0f)
                    {
                        Handles.color = Color.cyan;
                        Handles.SphereHandleCap(0, nearestPoint, Quaternion.identity, 1.5f, EventType.Repaint);
                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            m_SelectedSegment = nearestSegment;
                            m_ActiveTool = ToolMode.Move;
                        }
                    }
                }
                break;

            case ToolMode.Rotation:
                if(m_RoadChain.Segments.Length > 0)
                {
                    if(m_SelectedSegment != null)
                    {
                        Quaternion startRot = m_SelectedSegment.transform.rotation;
                        m_SelectedSegment.transform.rotation = Handles.RotationHandle(m_SelectedSegment.transform.rotation, m_SelectedSegment.transform.position);
                        if(startRot != m_SelectedSegment.transform.rotation) UpdateMeshes();
                    }
                    if(nearestSegment != m_SelectedSegment && Vector2.Distance(nearestScreenSpacePoint, screenSpaceMousePosition) < 100.0f)
                    {
                        Handles.color = Color.cyan;
                        Handles.SphereHandleCap(0, nearestPoint, Quaternion.identity, 1.5f, EventType.Repaint);
                        if (!Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            m_SelectedSegment = nearestSegment;
                            m_ActiveTool = ToolMode.Move;
                        }
                    }
                }
                break;

            case ToolMode.BezierControl:
                // Lifted from Freya Holmer's implementation of bezier curve extrapolation
                if(m_SelectedSegment != null)
                {
                    int arrowIDFrw  = GUIUtility.GetControlID( "Arrow Frw".GetHashCode(),  FocusType.Passive );
                    int arrowIDBack = GUIUtility.GetControlID( "Arrow Back".GetHashCode(), FocusType.Passive );

                    // Gather data on tangent locations, directions and the origin
                    Vector3 origin = m_SelectedSegment.GetControlPoint( 0, Space.World );
                    Vector3 tangentFrw = m_SelectedSegment.GetControlPoint( 1, Space.World );
                    Vector3 tangentBack = origin * 2 - tangentFrw; // Mirror tangent point around origin
                    Vector3 tangentDir = m_SelectedSegment.transform.forward;

                    // Calculate a plane to project against with the mouse ray while dragging
                    Vector3 camUp = SceneView.lastActiveSceneView.camera.transform.up;
                    Vector3 pNormal = Vector3.Cross( tangentDir, camUp ).normalized;
                    Plane draggingPlane = new Plane( pNormal, origin );

                    // This function had so many shared parameters, might as well shorten it a bit
                    float newDistance = 0;
                    bool TangentHandle( int id, Vector3 handlePos, Vector3 direction ) => DrawTangentHandle( id, handlePos, origin, direction, draggingPlane, ref newDistance );

                    // Draw handles, and modify tangent if you dragged them
                    bool changedFrw  = TangentHandle( arrowIDFrw,  tangentFrw,   tangentDir );
                    bool changedBack = TangentHandle( arrowIDBack, tangentBack, -tangentDir );

                    // If any of the two were changed, assign the new distance to the tangent length!
                    if( changedFrw || changedBack ) {
                        m_SelectedSegment.tangentLength = newDistance;
                        UpdateMeshes();
                    }
                }
                break;
        }
    }
    
    // Lifted from Freya Holmer's implementation of bezier curve etrapolation.
    private bool DrawTangentHandle( int id, Vector3 handlePos, Vector3 origin, Vector3 direction, Plane draggingPlane, ref float newDistance ) {

		bool wasChanged = false;
		float size = HandleUtility.GetHandleSize( handlePos ); // For screen-relative size
		float handleRadius = size * 0.1f;
		float cursorDistancePx = HandleUtility.DistanceToCircle( handlePos, handleRadius * 0.5f );

		// Input states
		Event e = Event.current;
		bool leftMouseButtonDown = e.button == 0;
		bool isDraggingThis = GUIUtility.hotControl == id && leftMouseButtonDown;
		bool isHoveringThis = HandleUtility.nearestControl == id;

		// IMGUI branching! DrawTangentHandle is run once per event type
		switch( e.type ) {
			case EventType.Layout:
				// Layout is called very early, and is where we set up interactable controls
				HandleUtility.AddControl( id, cursorDistancePx );
				break;
			case EventType.MouseDown:
				// Focus this control if we clicked it
				if( isHoveringThis && leftMouseButtonDown ) {
					GUIUtility.hotControl = id;
					GUIUtility.keyboardControl = id;
					e.Use();
				}
				break;
			case EventType.MouseDrag:
				if( isDraggingThis ) {
					Ray r = HandleUtility.GUIPointToWorldRay( e.mousePosition );
					if( draggingPlane.Raycast( r, out float dist ) ) {
						Vector3 intersectionPt = r.GetPoint( dist );
						float projectedDistance = Vector3.Dot( intersectionPt - origin, direction ).AtLeast(0.5f);
						newDistance = projectedDistance;
						wasChanged = true;
					}
					e.Use();
				}
				break;
			case EventType.MouseUp:
				// Defocus control on release
				if( isDraggingThis ) {
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;
			case EventType.Repaint:
                Handles.color = Color.cyan;
                Handles.DrawAAPolyLine( origin, handlePos );
                Quaternion rot = Quaternion.LookRotation( direction );
                Handles.SphereHandleCap( id, handlePos, rot, handleRadius, EventType.Repaint );
                Handles.color = Color.white;
				break;
		}

		return wasChanged;
	}
}
