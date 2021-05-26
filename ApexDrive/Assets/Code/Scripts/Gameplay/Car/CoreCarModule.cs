using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreCarModule : CarModule
{
    private Player m_Player;

    public Player Player { get { return m_Player; } }

    public void SetPlayer(Player player)
    {
        m_Player = player;
        this.Input.SetPlayer(player);
    }
}
