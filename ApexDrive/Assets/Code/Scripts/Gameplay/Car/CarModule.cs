using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputHandler))]
[RequireComponent(typeof(CoreCarModule))]
// [RequireComponent(typeof(PositionUpdate))]
public abstract class CarModule : MonoBehaviour
{
    private CoreCarModule m_Core;
    // private PositionUpdate m_Position;
    private CarInputHandler m_Input;

    public CoreCarModule Core { get { return m_Core; } }
    // public PositionUpdate Position { get { return m_Position; } }
    public CarInputHandler Input { get { return m_Input; } }

    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if(m_Core == null) m_Core = GetComponent<CoreCarModule>();
        if(m_Input == null) m_Input = GetComponent<CarInputHandler>();
        // if(m_Position == null) m_Position = GetComponent<PositionUpdate>();
    }
}
