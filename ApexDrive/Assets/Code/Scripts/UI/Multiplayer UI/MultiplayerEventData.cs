// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiplayerEventData : BaseEventData
{
    public Player Player;
    public EventSystem EventSystem;
    public MultiplayerEventData(EventSystem eventSystem, Player player) : base(eventSystem)
    {
        Player = player;
        EventSystem = eventSystem;
    }
}
