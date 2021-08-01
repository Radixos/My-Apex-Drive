using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    /// <summary>
    /// All actions (button inputs) are commented in the InputAction enum script.
    /// </summary>
    public static string GetInputManagerString(ControllerType controllerType, InputAction action, int controllerID)
    {
        string result = "";
        switch(controllerType)
        {
            case ControllerType.Xbox:
                result += "Xbox ";
                break;
            case ControllerType.Playstation:
                result += "PS4 ";
                break;
            case ControllerType.Other:
                result += "PS4 "; 
                break;
            case ControllerType.None:
                result += "PS4 ";
                break;
        }

        switch(action)
        {
            case InputAction.Axis_Horizontal:
                result += "Horizontal Axis ";
                break;
            case InputAction.Axis_Vertical:
                result += "Vertical Axis ";
                break;
            case InputAction.Axis_Trigger_Left:
                result += "Shoulder Axis 1 ";
                break;
            case InputAction.Axis_Trigger_Right:
                result += "Shoulder Axis 2 ";
                break;
            case InputAction.Button_Shoulder_1:
                result += "Shoulder Button 1 ";
                break;
            case InputAction.Button_Shoulder_2:
                result += "Shoulder Button 2 ";
                break;
            case InputAction.Button_Shoulder_3:
                result += "Shoulder Button 3 ";
                break;
            case InputAction.Button_Shoulder_4:
                result += "Shoulder Button 4 ";
                break;
            case InputAction.Button_Face_1:
                result += "Face Button 1 ";
                break;
            case InputAction.Button_Face_2:
                result += "Face Button 2 ";
                break;
            case InputAction.Button_Face_3:
                result += "Face Button 3 ";
                break;
            case InputAction.Button_Face_4:
                result += "Face Button 4 ";
                break;

        }

        result += "Controller " + controllerID;

        return result;
    }

    public static string GetReadableAction(ControllerType controllerType, InputAction action)
    {
        switch(controllerType)
        {
            case ControllerType.Playstation:
                switch(action)
                {
                    case InputAction.Axis_Horizontal:
                        return "Axis Horizontal";
                    case InputAction.Axis_Vertical:
                        return "Axis Vertical";
                    case InputAction.Axis_Trigger_Left:
                        return "L2 (Axis)";
                    case InputAction.Axis_Trigger_Right:
                        return "R2 (Axis)";
                    case InputAction.Button_Face_1:
                        return "X";
                    case InputAction.Button_Face_2:
                        return "Circle";
                    case InputAction.Button_Face_3:
                        return "Triangle";
                    case InputAction.Button_Face_4:
                        return "Square";
                    case InputAction.Button_Shoulder_1:
                        return "L1";
                    case InputAction.Button_Shoulder_2:
                        return "R1";
                    case InputAction.Button_Shoulder_3:
                        return "L2";
                    case InputAction.Button_Shoulder_4:
                        return "R2";
                }
                break;
            
            case ControllerType.Xbox:
                switch(action)
                {
                    case InputAction.Axis_Horizontal:
                        return "Axis Horizontal";
                    case InputAction.Axis_Vertical:
                        return "Axis Vertical";
                    case InputAction.Axis_Trigger_Left:
                        return "Left Trigger (Axis)";
                    case InputAction.Axis_Trigger_Right:
                        return "Right Trigger (Axis)";
                    case InputAction.Button_Face_1:
                        return "A";
                    case InputAction.Button_Face_2:
                        return "B";
                    case InputAction.Button_Face_3:
                        return "Y";
                    case InputAction.Button_Face_4:
                        return "X";
                    case InputAction.Button_Shoulder_1:
                        return "Left Bumper";
                    case InputAction.Button_Shoulder_2:
                        return "Right Bumper";
                    case InputAction.Button_Shoulder_3:
                        return "Left Trigger";
                    case InputAction.Button_Shoulder_4:
                        return "Right Trigger";
                }
                break;
        }
        return "";
    }

    

    public static float GetAxis(ControllerType controllerType, InputAction action, int controllerID)
    {
        return Input.GetAxis(GetInputManagerString(controllerType, action, controllerID));
    }

    public static float GetAxisRaw(ControllerType controllerType, InputAction action, int controllerID)
    {
        return Input.GetAxisRaw(GetInputManagerString(controllerType, action, controllerID));
    }

    public static bool GetButton(ControllerType controllerType, InputAction action, int controllerID)
    {
        return Input.GetButton(GetInputManagerString(controllerType, action, controllerID));
    }

    public static bool GetButtonDown(ControllerType controllerType, InputAction action, int controllerID)
    {
        return Input.GetButton(GetInputManagerString(controllerType, action, controllerID));
    }

    public static bool GetButtonUp(ControllerType controllerType, InputAction action, int controllerID)
    {
        return Input.GetButton(GetInputManagerString(controllerType, action, controllerID));
    }
}
