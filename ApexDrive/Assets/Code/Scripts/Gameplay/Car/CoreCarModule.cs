﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoreCarModule))]
[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Abilities))]
[RequireComponent(typeof(CarStats))]
[RequireComponent(typeof(ComboAnalyser))]
[RequireComponent(typeof(CarInputHandler))]
public class CoreCarModule : CarModule
{
    private Player m_Player;
    private CoreCarModule m_Core;
    private CarController m_Controller;
    private Abilities m_Abilities;
    private CarStats m_Stats;
    private ComboAnalyser m_ComboAnalyser;

    public override Player Player { get { return m_Player; } }
    public override CarController Controller { get { return m_Controller; } }
    public override Abilities Abilities { get { return m_Abilities; } }
    public override CarStats Stats { get { return m_Stats; } }
    public override ComboAnalyser ComboAnalyser { get { return m_ComboAnalyser; } }

    private GameplayInput m_Input;
    public override GameplayInput PlayerInput { get { return m_Input; } }

    public void SetPlayer(Player player)
    {
        m_Player = player;
        m_Input.SetPlayer(player);
    }

    protected override void ValidateComponents()
    {
        base.ValidateComponents();
        if(m_Controller == null) m_Controller = GetComponent<CarController>();
        if(m_Abilities == null) m_Abilities = GetComponent<Abilities>();
        if(m_Stats == null) m_Stats = GetComponent<CarStats>();
        if(m_ComboAnalyser == null) m_ComboAnalyser = GetComponent<ComboAnalyser>();

        if(m_Core == null) m_Core = gameObject.AddComponent<CoreCarModule>();
        if(m_Controller == null) m_Controller = gameObject.AddComponent<CarController>();
        if(m_Abilities == null) m_Abilities = gameObject.AddComponent<Abilities>();
        if(m_Stats == null) m_Stats = gameObject.AddComponent<CarStats>();
        if(m_ComboAnalyser == null) m_ComboAnalyser = gameObject.AddComponent<ComboAnalyser>();
    }
}
