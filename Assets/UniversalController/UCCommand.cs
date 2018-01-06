namespace AlphaOwl.UniversalController
{
    /// <summary>
    /// Prefix of the commands used to communicate with 
    /// the clients.
    /// </summary>
    public struct UCCommand
    {
        public const char Separator = ':';

        /* For commands received */

        /// <summary>
        /// example - REGISTER:{player_name}
        /// whereas player_name : string
        /// </summary>
        public const string Register = "REGISTER";
        public const int RegisterLength = 2;

        /// <summary>
        /// example - DEREGISTER:{player_id}
        /// whereas player_id : int
        /// </summary>
        public const string Deregister = "DEREGISTER";
        public const int DeregisterLength = 2;

        // Player actions

        /// <summary>
        /// example - {player_id}:KEY_DOWN:{key}
        /// example w/ extra - {player_id}:KEY_DOWN:{key}:{extra}
        /// whereas player_id : int; key : string; extra : string
        /// </summary>
        public const string KeyDown = "KEY_DOWN";
        public const int KeyDownLength = 3;
        public const int KeyDownExtraLength = 4;


        /// <summary>
        /// example - {player_id}:JOYSTICK:{x}:{y}
        /// whereas player_id : int; x : float; y : float;
        /// </summary>
        public const string Joystick = "JOYSTICK";
        public const int JoystickLength = 4;

        /// <summary>
        /// example - {player_id}:GYRO:{x}:{y}:{z}
        /// whereas player_id : int; x : float; y : float; z : float;
        /// </summary>
        public const string Gyro = "GYRO";
        public const int GyroLength = 5;

        // Replies to client
        public const string PlayerId = "PLAYER_ID:";
        public const string PlayerNotFound = "PLAYER_NOT_FOUND";
        public const string ServerShutDown = "SERVER_SHUTDOWN";
        public const string ServerFull = "SERVER_FULL";
        public const string InvalidCmd = "INVALID_COMMAND";

        // For test usage
        public const string Test = "TEST_CMD";
    }
}