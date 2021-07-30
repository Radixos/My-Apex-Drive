public struct GameplayInput
    {
        private Player m_Player;

        public string HorizontalInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Axis_Horizontal, m_Player.ControllerID); }
        public string AccelerateInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Axis_Trigger_Right, m_Player.ControllerID); }
        public string BrakeInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Button_Face_2, m_Player.ControllerID); }
        public string DriftInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Button_Shoulder_3, m_Player.ControllerID); }
        public string BoostInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Button_Face_4, m_Player.ControllerID); }
        public string AttackInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Button_Shoulder_1, m_Player.ControllerID); }
        public string ShieldInput { get => InputManager.GetInputManagerString(m_Player.ControllerType, InputAction.Button_Shoulder_2, m_Player.ControllerID); }

        public void SetPlayer(Player player)
        {
            m_Player = player;
        }
    }
