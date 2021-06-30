// Alec Gamble
// Code Adapted From Senshi's Answer on UnityAnswers at: https://forum.unity.com/threads/handling-multiple-inputs-controllers.355711/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[DefaultExecutionOrder(100)]
[RequireComponent(typeof(MultiplayerSelectionEventSystem))]
public class MultiplayerInputModule : BaseInputModule
{
    [SerializeField] private MultiplayerCursor m_CursorPrefab;
    [SerializeField] private RectTransform m_CursorPool;
	[SerializeField] private Vector2 m_CursorOffsetFromSelection = new Vector2(-20.0f, 0.0f);
	[SerializeField] private Vector2 m_CursorStagger = new Vector2(-30.0f, 0.0f);
    [SerializeField] private float m_AxisRepeatDelay = 0.25f;

	[SerializeField] private string m_VerticalAxisPrefix = "Vertical ";
	[SerializeField] private string m_HorizontalAxisPrefix = "Horizontal ";
	[SerializeField] private string m_SubmitButtonPrefix = "Submit ";
	[SerializeField] private string m_CancelButtonPrefix = "Cancel ";
	
	private MultiplayerSelectionEventSystem m_MultiplayerEventSystem;
	private MultiplayerCursor[] m_Cursors = new MultiplayerCursor[GameManager.MaxPlayers];
	private float[] currentRepeatDelay = new float[GameManager.MaxPlayers];

	protected override void Awake()
    {
        if(m_CursorPool == null) Debug.LogWarning("[MultiplayerInputModule] :: Please assign a rect transform to be the pool for cursors");
		m_MultiplayerEventSystem = GetComponent<MultiplayerSelectionEventSystem>();
		foreach(Player player in GameManager.Instance.Players)
        {
            m_Cursors[player.PlayerID] = Instantiate(m_CursorPrefab, m_CursorPool);
			m_Cursors[player.PlayerID].SetColor(GameManager.Instance.PlayerColors[player.PlayerID]);
			m_Cursors[player.PlayerID].name  = "Cursor (Player " + player.PlayerReadableID + ")";
        }

		foreach(Player player in GameManager.Instance.ConnectedPlayers)
		{
			m_Cursors[player.PlayerID].GetComponent<Animator>().SetBool("IsVisible", true);
		}
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

	public MultiplayerCursor GetCursor(int index){
		return m_Cursors[index];
	}

	public override void Process(){
		AxisEventData[] axisEventData = new AxisEventData[GameManager.MaxPlayers];
		foreach(Player player in GameManager.Instance.ConnectedPlayers)
		{
			int i = player.PlayerID;
			if(!m_MultiplayerEventSystem.LockedController(i) && currentRepeatDelay[i] >= m_AxisRepeatDelay){
				if(Input.GetAxis(m_VerticalAxisPrefix + player.ControllerID) > 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Up;
					currentRepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(m_VerticalAxisPrefix + player.ControllerID) < 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Down;
					currentRepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(m_HorizontalAxisPrefix + player.ControllerID) < 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Left;
					currentRepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(m_HorizontalAxisPrefix + player.ControllerID) > 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Right;
					currentRepeatDelay[i] = 0f;
				}
				else
				{
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.None;
				}
			}

			Selectable nextSelectable = null;
			Selectable oldSelectable = m_MultiplayerEventSystem.GetSelected(i);

			if(oldSelectable != null && axisEventData[i] != null)
			{
				switch(axisEventData[i].moveDir)
				{
					case MoveDirection.Down:
						nextSelectable = oldSelectable.FindSelectableOnDown();
						break;
					case MoveDirection.Up:
						nextSelectable = oldSelectable.FindSelectableOnUp();
						break;
					case MoveDirection.Left:
						nextSelectable = oldSelectable.FindSelectableOnLeft();
						break;
					case MoveDirection.Right:
						nextSelectable = oldSelectable.FindSelectableOnRight();
						break;
					case MoveDirection.None:
						break;
				}

			}

			if(nextSelectable != null)
			{
				m_MultiplayerEventSystem.SetSelected(i, nextSelectable);
				UpdateCursorPositions();
				if(axisEventData[i] != null) ExecuteEvents.Execute(oldSelectable.gameObject, axisEventData[i], ExecuteEvents.moveHandler);
				FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI Move");
			}

			if(Input.GetButtonDown(m_SubmitButtonPrefix + player.ControllerID)){
				if(!m_MultiplayerEventSystem.LockedController(i)){
					MultiplayerEventData data = new MultiplayerEventData(m_MultiplayerEventSystem, player);
					IMultiplayerSubmitHandler submitHandler = m_MultiplayerEventSystem.GetSelected(i).GetComponent<IMultiplayerSubmitHandler>();
					if(submitHandler != null && submitHandler.OnSubmit(data))
					{
						m_MultiplayerEventSystem.LockController(i);						
					}
					FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Submit");
				}
			}
			else if(Input.GetButtonDown(m_CancelButtonPrefix + player.ControllerID)){
				if(m_MultiplayerEventSystem.LockedController(i)){
					m_MultiplayerEventSystem.UnlockController(i);
					FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Cancel");
				}
				MultiplayerEventData data = new MultiplayerEventData(m_MultiplayerEventSystem, player);
				IMultiplayerCancelHandler cancelHandler = m_MultiplayerEventSystem.GetSelected(i).GetComponent<IMultiplayerCancelHandler>();
				if(cancelHandler != null) cancelHandler.OnCancel(data);
			}
			currentRepeatDelay[i] += Time.deltaTime;
		}
	}

	public void UpdateCursorPositions()
	{
		foreach(Player player in GameManager.Instance.ConnectedPlayers)
		{	int playerID = player.PlayerID;
			Selectable selection = m_MultiplayerEventSystem.GetSelected(playerID);
			Vector2 offset = m_CursorOffsetFromSelection;
			for(int i = 0; i < GameManager.MaxPlayers; i++)
			{
				if(i < playerID && GameManager.Instance.Players[i].IsConnected && m_MultiplayerEventSystem.GetSelected(i) == selection) offset += m_CursorStagger;
			}
			RectTransform cursorRT = m_Cursors[playerID].transform as RectTransform;
			RectTransform selectionRT = selection.transform as RectTransform;

			cursorRT.position = new Vector3(selectionRT.position.x + offset.x - selectionRT.sizeDelta.x / 2.0f, selectionRT.position.y + offset.y);
		}
	}

	public void OnPlayerConnected(Player player)
	{
		MultiplayerCursor cursor = m_Cursors[player.PlayerID];
		cursor.GetComponent<Animator>().SetBool("IsVisible", true);
	}

	public void OnPlayerDisconnected(Player player)
	{
		MultiplayerCursor cursor = m_Cursors[player.PlayerID];
		cursor.GetComponent<Animator>().SetBool("IsVisible", false);
	}
}
