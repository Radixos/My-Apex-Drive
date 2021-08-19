//Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool m_LockX = false;
    [SerializeField] private bool m_LockY = false;
    [SerializeField] private bool m_LockZ = false;

    private void LateUpdate()
    {
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        if(m_LockX) lookPos.x = 0.0f;
        if(m_LockY) lookPos.y = 0.0f;
        if(m_LockZ) lookPos.z = 0.0f;
        transform.rotation = Quaternion.LookRotation(lookPos);
        // transform.LookAt(Camera.main.transform, Camera.main.transform.rotation * Vector3.up);
    }
}
