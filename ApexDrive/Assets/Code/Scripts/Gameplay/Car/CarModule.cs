using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CoreCarModule))]
public class CarModule : MonoBehaviour
{
    private CoreCarModule m_Core;
    public CoreCarModule Core { get { return m_Core; } }

    public virtual CarController Controller { get { return Core.Controller; } }
    public virtual Abilities Abilities { get { return Core.Abilities; } }
    public virtual CarStats Stats { get { return Core.Stats; } }
    public virtual ComboAnalyser ComboAnalyser { get { return Core.ComboAnalyser; } }
    public virtual GameplayInput PlayerInput { get { return Core.PlayerInput; } }

    public virtual Player Player { get { return Core.Player; } }

    protected virtual void Awake()
    {
        // Might be neccersary to validate components on enable - unsure.
        ValidateComponents();
    }

    protected virtual void ValidateComponents()
    {
        if(m_Core == null) m_Core = GetComponent<CoreCarModule>();
        if(m_Core == null) m_Core = gameObject.AddComponent<CoreCarModule>();
    }
}
