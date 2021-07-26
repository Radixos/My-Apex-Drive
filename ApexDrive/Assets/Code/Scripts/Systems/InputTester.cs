using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    public Player player;

    [SerializeField] private ControllerType[] m_Controllers;
    private void Start()
    {
        string[] joystickNames = Input.GetJoystickNames();
        m_Controllers = new ControllerType[joystickNames.Length];
        for(int i = 0; i < joystickNames.Length; i++)
        {
            if(joystickNames[i].ToLower().Contains("xbox")) m_Controllers[i] = ControllerType.Xbox;
            else m_Controllers[i] = ControllerType.Playstation;
        }
    }

    private void Update()
    {
        for(int i = 1; i <= m_Controllers.Length; i++)
        {
            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Face_1, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Face_1)+" event.");
            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Face_2, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Face_2)+" event.");
            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Face_3, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Face_3)+" event.");
            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Face_4, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Face_4)+" event.");

            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Shoulder_1, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Shoulder_1)+" event.");
            if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Shoulder_2, i))) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Shoulder_2)+" event.");
            // if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Shoulder_3, i))) 
            //     Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Shoulder_3)+" event.");
            // if(Input.GetButtonDown(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Button_Shoulder_4, i))) 
            //     Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Button_Shoulder_4)+" event.");


            if(Mathf.Abs(Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Horizontal, i))) >= 0.2) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Axis_Horizontal)+" axis value of " 
                + Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Horizontal, i)));

            if(Mathf.Abs(Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Vertical, i))) >= 0.2) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Axis_Vertical)+" axis value of "  
                + Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Vertical, i)));

            if(Mathf.Abs(Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Trigger_Left, i))) >= 0.2) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Axis_Trigger_Left)+" axis value of " 
                + Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Trigger_Left, i)));

            if(Mathf.Abs(Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Trigger_Right, i))) >= 0.2) 
                Debug.Log("[InputTester::ProcessInputs()] Controller " + i + " ("+ m_Controllers[i-1].ToString()+") just submitted a "+ InputManager.GetReadableAction(m_Controllers[i-1],InputAction.Axis_Trigger_Right)+" axis value of " 
                + Input.GetAxis(InputManager.GetInputManagerString(m_Controllers[i-1], InputAction.Axis_Trigger_Right, i)));
        }
    }
}
