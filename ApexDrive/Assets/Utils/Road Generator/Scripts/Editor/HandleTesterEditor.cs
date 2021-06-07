using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HandleTester))]
public class HandleTesterEditor : Editor
{
    private HandleTester m_HandleTester;

    private void OnEnable()
    {
        m_HandleTester = (HandleTester) target;
        Tools.current = Tool.None;
        Tools.hidden = true;
    }

    private void OnSceneGUI()
    {
        Handles.SphereHandleCap(0, m_HandleTester.transform.position, Quaternion.identity, 1.0f, EventType.Repaint);
        m_HandleTester.transform.position = Handles.PositionHandle(m_HandleTester.transform.position + Vector3.up * 2.0f, Quaternion.identity) - Vector3.up * 2.0f;
    }
}
