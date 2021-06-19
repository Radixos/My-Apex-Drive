public struct GameplayInput
    {
        public Player m_Player;

        public string HorizontalInput { get => "Horizontal " + m_Player.ControllerID; }
        public string AccelerateInput { get => "Accelerate " + m_Player.ControllerID; }
        public string BrakeInput { get => "Brake " + m_Player.ControllerID; }
        public string DriftInput { get => "Drift " + m_Player.ControllerID; }
        public string BoostInput { get => "Boost " + m_Player.ControllerID; }
        public string PowerAInput { get => "Power A " + m_Player.ControllerID; }
        public string PowerBInput { get => "Power B " + m_Player.ControllerID; }

        public void SetPlayer(Player player)
        {
            m_Player = player;
        }
    }
