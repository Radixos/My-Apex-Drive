using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    [SerializeField] private Player m_Player;

    public string HorizontalInput { get { return "Horizontal " + m_Player.ControllerID; } }
    public string AccelerateInput { get { return "Accelerate " + m_Player.ControllerID; } }
    public string BrakeInput { get { return "Brake " + m_Player.ControllerID; } }
    public string DriftInput { get { return "Drift " + m_Player.ControllerID; } }
    public string BoostInput { get { return "Boost " + m_Player.ControllerID; } }
    public string PowerAInput { get { return "Power A " + m_Player.ControllerID; } }
    public string PowerBInput { get { return "Power B " + m_Player.ControllerID; } }

    public void SetPlayer(Player player)
    {
        m_Player = player;
    }
}
