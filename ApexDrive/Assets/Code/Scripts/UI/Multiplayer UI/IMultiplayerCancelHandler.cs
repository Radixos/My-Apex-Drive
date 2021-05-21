//Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMultiplayerCancelHandler : IEventSystemHandler
{
    void OnCancel(MultiplayerEventData eventData);
}
