// Alec Gamble
// Code Adapted From Senshi's Answer on UnityAnswers at: https://forum.unity.com/threads/handling-multiple-inputs-controllers.355711/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(MultiplayerInputModule))]
public class MultiplayerSelectionEventSystem : EventSystem
{	
	private MultiplayerInputModule m_InputModule;
	private Selectable[] m_CurrentSelectedObjects = new Selectable[GameManager.MaxPlayers];
	private bool[] m_ControllerLocked = new bool[GameManager.MaxPlayers];

	protected override void Awake()
	{
		base.Awake();
		m_InputModule = GetComponent<MultiplayerInputModule>();
	}

    protected override void OnEnable()
    {
        base.OnEnable();
		GameManager.OnPlayerConnected += OnPlayerConnected;
		GameManager.OnPlayerDisconnected += OnPlayerDisconnected;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
		GameManager.OnPlayerConnected -= OnPlayerConnected;
		GameManager.OnPlayerDisconnected -= OnPlayerDisconnected;
    }

	private void OnPlayerConnected(Player player)
	{
		SetSelected(player.PlayerID, firstSelectedGameObject.GetComponent<Selectable>());
		m_InputModule.UpdateCursorPositions();
	}

	private void OnPlayerDisconnected(Player player)
	{
		m_CurrentSelectedObjects[player.PlayerID] = null;
		m_ControllerLocked[player.PlayerID] = false;
	}

	public void SetSelected(int playerID, Selectable selected){
		m_CurrentSelectedObjects[playerID] = selected;
	}

	public Selectable GetSelected(int playerID){
		return m_CurrentSelectedObjects[playerID];
	}

	public void LockController(int playerID){
		m_ControllerLocked[playerID] = true;
		m_InputModule.GetCursor(playerID).Lock();
	}

	public void UnlockController(int playerID){
		m_ControllerLocked[playerID] = false;
		m_InputModule.GetCursor(playerID).Unlock();
	}

	public bool LockedController(int playerID){
		return m_ControllerLocked[playerID];
	}
}

