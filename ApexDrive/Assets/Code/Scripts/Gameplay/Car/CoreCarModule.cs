using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoreCarModule))]
[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Abilities))]
[RequireComponent(typeof(CarStats))]
[RequireComponent(typeof(ComboAnalyser))]
public class CoreCarModule : CarModule
{
    [SerializeField] private Player m_Player;
    private CarController m_Controller;
    private Abilities m_Abilities;
    private CarStats m_Stats;
    private ComboAnalyser m_ComboAnalyser;

    [SerializeField] private Rigidbody m_Rigidbody;

    public override Player Player { get { return m_Player; } }
    public override CarController Controller { get { return m_Controller; } }
    public override Abilities Abilities { get { return m_Abilities; } }
    public override CarStats Stats { get { return m_Stats; } }
    public override ComboAnalyser ComboAnalyser { get { return m_ComboAnalyser; } }
    public override Rigidbody Rigidbody { get { return m_Rigidbody; } }

    private GameplayInput m_Input;
    public override GameplayInput PlayerInput { get { return m_Input; } }

    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }

    [HideInInspector] public Checkpoint LastCheckpoint;

    protected override void Awake()
    {
        base.Awake();
        if(m_Player == null) m_Player = new Player(0,Color.blue);
        SetPlayer(m_Player);
    }

    public void SetPlayer(Player player)
    {
        m_Player = player;
        m_Input.SetPlayer(player);
    }

    protected override void ValidateComponents()
    {
        if(m_Controller == null) m_Controller = GetComponent<CarController>();
        if(m_Abilities == null) m_Abilities = GetComponent<Abilities>();
        if(m_Stats == null) m_Stats = GetComponent<CarStats>();
        if(m_ComboAnalyser == null) m_ComboAnalyser = GetComponent<ComboAnalyser>();

        if(m_Controller == null) m_Controller = gameObject.AddComponent<CarController>();
        if(m_Abilities == null) m_Abilities = gameObject.AddComponent<Abilities>();
        if(m_Stats == null) m_Stats = gameObject.AddComponent<CarStats>();
        if(m_ComboAnalyser == null) m_ComboAnalyser = gameObject.AddComponent<ComboAnalyser>();
    }
}
