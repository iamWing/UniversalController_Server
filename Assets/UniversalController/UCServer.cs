using System;
using System.Net.Sockets;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    /// <summary>
    /// Prefix of the commands used to communicate with 
    /// the clients.
    /// </summary>
    struct Command
    {
        /* For commands received */
        public const string Register = "REGISTER";
        public const string Deregister = "DEREGISTER";

        // Player actions
        public const string KeyDown = "KEY_DOWN";
        public const string Joystick = "JOYSTICK";
        public const string Gyro = "GYRO";

        // Replies to client
        public const string PlayerId = "PLAYER_ID";

        // For test usage
        public const string Test = "TEST_CMD";
    }

    public class UCServer : NetworkUtilities.IMessageReceiver,
    NetworkUtilities.IMessageSender
    {
        // The only instance of UCServer should have.
        private static UCServer instance = null;

        // Default variables.
        private const int DefaultPort = 28910;
        private const int DefaultMaxConnections = 4;

        private Socket serverSocket;
        private string[] players; // Connected players.

        /// <summary>
        /// Returns the existing instance or create a new instance 
        /// of UCServer.
        /// </summary>
        /// <param name="port">Port number for the server socket to 
        /// bind to. Default port is 28910.</param>
        /// <param name="maxConn">Maximum number of connections for 
        /// the server. Default value is 4.</param>
        /// <param name="debug">Writes log msg to console if debug 
        /// == true.</param>
        /// <returns>A newly initialised instance or an existing 
        /// instance of UCServer.</returns>
        public static UCServer Init(int port = DefaultPort,
                                    int maxConn = DefaultMaxConnections,
                                    bool debug = false)
        {
            DebugUtilities.Enable = debug;

            if (instance == null)
            {
                instance = new UCServer(port, maxConn);
                instance.Start(); // Start server.
            }

            return instance;
        }

        /// <summary>
        /// Fire up the server by places the server socket to 
        /// listening state.
        /// </summary>
        public void Start()
        {
            NetworkUtilities.StartListening(serverSocket, this, this);
        }

        /// <summary>
        /// Shutdown the server socket.
        /// </summary>
        public void Shutdown()
        {
            NetworkUtilities.ShutdownSocket(serverSocket);
        }

        /* Private methods */

        /// <summary>
        /// Private constructor that prevents initialisation 
        /// of an instance of UCServer by using the default 
        /// constructor.
        /// </summary>
        private UCServer(int port, int maxConn)
        {
            serverSocket = NetworkUtilities.InitSocketServer(
                            NetworkUtilities.GetIpAddress(),
                            port);

            players = new string[maxConn];
        }

        /* Override methods from NetworkUtilities.IMessageReceiver */

        public void OnReceiveComplete(string msg)
        {
        }

        public void OnReceiveFail(string err)
        {

        }

        /* Override methods from NetworkUtilities.IMessageSender */

        public void OnSendComplete()
        {

        }

        public void OnSendFail(string err)
        {

        }
    }
}
