// Alec Gamble

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiplayerButton : Button, IMultiplayerSubmitHandler, IMultiplayerCancelHandler
{
    public MultiplayerEvent OnSubmitEvent;
    public MultiplayerEvent OnCancelEvent;
    public bool DisableControllerOnSubmit = false;

    public bool OnSubmit(MultiplayerEventData eventData)
    {
        OnSubmitEvent.Invoke(eventData);
        return DisableControllerOnSubmit;
    }

    public void OnCancel(MultiplayerEventData eventData)
    {
        OnCancelEvent.Invoke(eventData);
    }
}
