// Alec Gamble
// Code Adapted From Senshi's Answer on UnityAnswers at: https://forum.unity.com/threads/handling-multiple-inputs-controllers.355711/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[DefaultExecutionOrder(100)]
[RequireComponent(typeof(MultiplayerEventSystem))]
public class MultiplayerInputModule : BaseInputModule
{
    [SerializeField] private MultiplayerCursor m_CursorPrefab;
    [SerializeField] private RectTransform m_CursorPool;
	[SerializeField] private Vector2 m_CursorOffsetFromSelection = new Vector2(-20.0f, 0.0f);
	[SerializeField] private Vector2 m_CursorStagger = new Vector2(-30.0f, 0.0f);
    [SerializeField] private float m_AxisRepeatDelay = 0.25f;

	[SerializeField] private InputAction m_VerticalAxisAction = InputAction.Axis_Vertical;
	[SerializeField] private InputAction m_HorizontalAxisAction = InputAction.Axis_Horizontal;
	[SerializeField] private InputAction m_SubmitAction = InputAction.Button_Face_1;
	[SerializeField] private InputAction m_CancelAction = InputAction.Button_Face_2;
	
	private MultiplayerEventSystem m_MultiplayerEventSystem;
	private MultiplayerCursor[] m_Cursors;
	private float[] m_RepeatDelay;

	protected override void Awake()
	{
		base.Awake();
		Initialise();
	}

	private void Initialise()
	{
		if(m_CursorPool == null) Debug.LogWarning("[MultiplayerInputModule::Initialise()] Please assign a rect transform to be the pool for cursors");
		m_MultiplayerEventSystem = GetComponent<MultiplayerEventSystem>();

		foreach(Transform cursor in m_CursorPool)
		{
			Destroy(cursor);
		}

		m_Cursors = new MultiplayerCursor[GameManager.MaxPlayers];
		m_RepeatDelay = new float[GameManager.MaxPlayers];

		foreach(Player player in GameManager.Instance.Players)
        {
            m_Cursors[player.PlayerID] = Instantiate(m_CursorPrefab, m_CursorPool);
			m_Cursors[player.PlayerID].SetColor(GameManager.Instance.PlayerColors[player.PlayerID]);
			m_Cursors[player.PlayerID].name  = "Cursor (Player " + player.PlayerReadableID + ")";
        }
	}

	public MultiplayerCursor GetCursor(int index)
	{
		return m_Cursors[index];
	}

	public override void Process(){
		AxisEventData[] axisEventData = new AxisEventData[GameManager.MaxPlayers];
		foreach(Player player in GameManager.Instance.ConnectedPlayers)
		{
			int i = player.PlayerID;
			if(!m_MultiplayerEventSystem.LockedController(i) && m_RepeatDelay[i] >= m_AxisRepeatDelay){
				if(Input.GetAxis(InputManager.GetInputManagerString(player.ControllerType, m_VerticalAxisAction, player.ControllerID)) > 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Up;
					m_RepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(InputManager.GetInputManagerString(player.ControllerType, m_VerticalAxisAction, player.ControllerID)) < 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Down;
					m_RepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(InputManager.GetInputManagerString(player.ControllerType, m_HorizontalAxisAction, player.ControllerID)) < 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Left;
					m_RepeatDelay[i] = 0f;
				}
				else if(Input.GetAxis(InputManager.GetInputManagerString(player.ControllerType, m_HorizontalAxisAction, player.ControllerID)) > 0.0){
					axisEventData[i] = new AxisEventData(m_MultiplayerEventSystem);
					axisEventData[i].moveDir = MoveDirection.Right;
					m_RepeatDelay[i] = 0f;
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

			if(Input.GetButtonDown(InputManager.GetInputManagerString(player.ControllerType, m_SubmitAction, player.ControllerID))){
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
			else if(Input.GetButtonDown(InputManager.GetInputManagerString(player.ControllerType, m_CancelAction, player.ControllerID))){
				if(m_MultiplayerEventSystem.LockedController(i)){
					m_MultiplayerEventSystem.UnlockController(i);
					FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Cancel");
				}
				MultiplayerEventData data = new MultiplayerEventData(m_MultiplayerEventSystem, player);
				IMultiplayerCancelHandler cancelHandler = m_MultiplayerEventSystem.GetSelected(i).GetComponent<IMultiplayerCancelHandler>();
				if(cancelHandler != null) cancelHandler.OnCancel(data);
			}
			m_RepeatDelay[i] += Time.deltaTime;
		}
	}

	public void UpdateCursorPositions()
	{
		for(int i = 0; i < m_Cursors.Length; i++)
		{
			if(m_Cursors[i].IsActive)
			{
				Selectable selection = m_MultiplayerEventSystem.GetSelected(i);
				Vector2 offset = m_CursorOffsetFromSelection;
				for(int j = 0; j < m_Cursors.Length; j++)
				{
					if(j < i && m_Cursors[j].IsActive && m_MultiplayerEventSystem.GetSelected(j) == selection) offset += m_CursorStagger;
				}

				RectTransform cursorRT = m_Cursors[i].transform as RectTransform;
				RectTransform selectionRT = selection.transform as RectTransform;
				cursorRT.position = new Vector3(selectionRT.position.x + offset.x, selectionRT.position.y + offset.y);
			}
		}
	}

	public void AddPlayerCursor(int playerID)
	{
		MultiplayerCursor cursor = m_Cursors[playerID];
		cursor.GetComponent<Animator>().SetBool("IsVisible", true);
		cursor.IsActive = true;
		UpdateCursorPositions();
	}

	public void RemovePlayerCursor(int playerID)
	{
		MultiplayerCursor cursor = m_Cursors[playerID];
		cursor.GetComponent<Animator>().SetBool("IsVisible", false);
		cursor.IsActive = false;
		UpdateCursorPositions();
	}
}
