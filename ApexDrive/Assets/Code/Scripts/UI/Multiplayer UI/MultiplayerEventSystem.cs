// Alec Gamble
// Code Adapted From Senshi's Answer on UnityAnswers at: https://forum.unity.com/threads/handling-multiple-inputs-controllers.355711/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(MultiplayerInputModule))]
public class MultiplayerEventSystem : EventSystem
{	
	public static MultiplayerEventSystem Current;
	private MultiplayerInputModule m_InputModule;
	private Selectable[] m_CurrentSelectedObjects = new Selectable[GameManager.MaxPlayers];
	private bool[] m_ControllerLocked = new bool[GameManager.MaxPlayers];

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		m_InputModule = GetComponent<MultiplayerInputModule>();
	}

	public void AddPlayer(int playerID)
	{
		SetSelected(playerID, firstSelectedGameObject.GetComponent<Selectable>());
		m_InputModule.AddPlayerCursor(playerID);
	}

	public void RemovePlayer(int playerID)
	{
		m_InputModule.RemovePlayerCursor(playerID);
		m_CurrentSelectedObjects[playerID] = null;
		m_ControllerLocked[playerID] = false;
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

	public void UpdateCursorPositions()
	{
		m_InputModule.UpdateCursorPositions();
	}
}

