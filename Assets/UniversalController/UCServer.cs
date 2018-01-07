using System.Net.Sockets;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCServer : NetworkUtilities.IMessageReceiver,
    NetworkUtilities.IMessageSender
    {
        // The only instance of UCServer should have.
        private static UCServer instance = null;

        // Default variables.
        private const int DefaultPort = 28910;
        private const int DefaultMaxConnections = 4;

        private const float MinF = -1f;
        private const float MaxF = 1f;

        private Socket serverSocket;
        private Socket[] clients; // Registered clients.

        private ICommandHandler cmdHandler;

        /// <summary>
        /// Returns the existing instance or create a new instance 
        /// of UCServer.
        /// </summary>
        /// <param name="handler">An instance of ICommandHandler that 
        /// handles the commands received.</param>
        /// <param name="port">Port number for the server socket to 
        /// bind to. Default port is 28910.</param>
        /// <param name="maxConn">Maximum number of connections for 
        /// the server. Default value is 4.</param>
        /// <param name="debug">Writes log msg to console if debug 
        /// == true.</param>
        /// <returns>A newly initialised instance or an existing 
        /// instance of UCServer.</returns>
        public static UCServer Init(ICommandHandler handler,
                                    int port = DefaultPort,
                                    int maxConn = DefaultMaxConnections,
                                    bool debug = false)
        {
            DebugUtilities.Enable = debug;

            if (instance == null)
            {
                instance = new UCServer(port, maxConn);
                instance.Start(); // Start server.
            }

            instance.cmdHandler = handler;

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
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] != null)
                    SendMsg(i, UCCommand.ServerShutDown);
            }

            DebugUtilities.Log("Notified all clients.");

            NetworkUtilities.ShutdownSocket(serverSocket);
        }

        /// <summary>
        /// Send message to a connected player.
        /// </summary>
        /// <param name="targetPlayer">ID of the selected player 
        /// which uses to get the related socket.</param>
        /// <param name="msg">String that needs to be sent to 
        /// the player.</param>
        public void SendMsg(int targetPlayer, string msg)
        {
            NetworkUtilities.Send(clients[targetPlayer], msg);
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
                            NetworkUtilities.GetIPv4Address(),
                            port);

            clients = new Socket[maxConn];
        }

        private void HandleInputCommands(Socket socket, int playerId,
                string[] cmd, string fullCmd)
        {
            // PLayer ID has been taken out from the string array.
            switch (cmd[0])
            {
                case UCCommand.Gyro:
                    if (cmd.Length == UCCommand.GyroLength - 1)
                    {
                        GyroCommand(
                            socket, playerId,
                            GeneralUtilities.ArrayCopy<string>(
                                cmd, 1, cmd.Length - 1
                            ), fullCmd
                        );
                        break;
                    }
                    else goto default;
                case UCCommand.Joystick:
                    if (cmd.Length == UCCommand.JoystickLength - 1)
                    {
                        JoystickCommand(
                            socket, playerId,
                            GeneralUtilities.ArrayCopy<string>(
                                cmd, 1, cmd.Length - 1
                            ), fullCmd
                        );
                        break;
                    }
                    else goto default;
                case UCCommand.KeyDown:
                    if (cmd.Length == UCCommand.KeyDownLength - 1
                        || cmd.Length == UCCommand.KeyDownExtraLength - 1)
                    {
                        KeyDownCommand(
                            playerId,
                            GeneralUtilities.ArrayCopy<string>(
                                cmd, 1, cmd.Length - 1
                            )
                        );
                        break;
                    }
                    else goto default;
                default:
                    InvalidCommand(socket, fullCmd);
                    break;
            }
        }

        private bool isValidFloat(float val, float min, float max)
        {
            return val <= max && val >= min;
        }

        /* Command handling */

        private void RegisterClient(Socket socket, string playerName)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null)
                {
                    clients[i] = socket;
                    cmdHandler.Register(i, playerName);

                    // Replies player ID
                    // example -  PLAYER_ID:1
                    SendMsg(i, UCCommand.PlayerId + i);

                    DebugUtilities.Log(playerName +
                    "registered with player ID " + i);

                    return;
                }
            }

            DebugUtilities.Log("Unable to register player " + playerName +
            ". Maximum player numbers reached.");
            NetworkUtilities.Send(socket, UCCommand.ServerFull);
        }

        private void DeregisterClient(Socket socket, int playerId)
        {
            if (playerId < clients.Length && clients[playerId] != null)
            {
                cmdHandler.Deregister(playerId);

                // Todo: Toggle this line for testing later.
                NetworkUtilities.ShutdownSocket(clients[playerId]);
                clients[playerId] = null; // release space for new client
            }
            else
            {
                PlayerNotFound(socket, playerId);
            }
        }

        private void GyroCommand(Socket socket, int playerId, string[] cmd,
        string fullCmd)
        {
            // Player ID has been taken out & Command.Gyro prefix needs
            // to be taken away as well.
            float[] pos = new float[cmd.Length];

            for (int i = 0; i < cmd.Length; i++)
            {
                float result;

                if (float.TryParse(cmd[i], out result))
                {
                    if (isValidFloat(result, MinF, MaxF))
                        pos[i] = result;
                    else
                    {
                        // Invalid float value
                        InvalidCommand(socket, fullCmd);
                        return;
                    }
                }
                else
                {
                    // Cannot parse to float
                    InvalidCommand(socket, fullCmd);
                    return;
                }
            }

            cmdHandler.Gyro(playerId, x: pos[0], y: pos[1], z: pos[2]);
        }

        private void JoystickCommand(Socket socket, int playerId,
        string[] cmd, string fullCmd)
        {
            // Player ID has been taken out & Command.Joystick prefix needs
            // to be taken away as well.
            float[] pos = new float[cmd.Length];

            for (int i = 0; i < cmd.Length; i++)
            {
                float result;

                if (float.TryParse(cmd[i], out result))
                {
                    if (isValidFloat(result, MinF, MaxF))
                        pos[i] = result;
                    else
                    {
                        // Invalid float value
                        InvalidCommand(socket, fullCmd);
                        return;
                    }
                }
                else
                {
                    // Cannot parse to float
                    InvalidCommand(socket, fullCmd);
                    return;
                }
            }

            cmdHandler.Joystick(playerId, x: pos[0], y: pos[1]);
        }

        private void KeyDownCommand(int playerId, string[] cmd)
        {
            bool hasExtra = (cmd.Length == UCCommand.KeyDownExtraLength - 2);

            if (hasExtra)
                cmdHandler.KeyDown(playerId, cmd[0], cmd[1]);
            else
                cmdHandler.KeyDown(playerId, cmd[0]);
        }

        /* Error handling */

        private void PlayerNotFound(Socket socket, int playerId)
        {
            DebugUtilities.Log(
                                msg: UCCommand.PlayerNotFound + " {" +
                                UCCommand.PlayerId + playerId + "}",
                                type: LogType.Error
                            );

            NetworkUtilities.Send(socket, UCCommand.PlayerNotFound);

        }

        private void InvalidCommand(Socket socket, string cmd)
        {
            DebugUtilities.Log(
                msg: UCCommand.InvalidCmd + " {" + cmd + "}",
                type: LogType.Error
            );

            NetworkUtilities.Send(socket, UCCommand.InvalidCmd);
        }

        /* Implement methods from NetworkUtilities.IMessageReceiver */

        public void OnReceiveComplete(Socket handler, string msg)
        {
            string[] cmd = msg.Split(UCCommand.Separator);

            switch (cmd[0])
            {
                case UCCommand.Register:
                    if (cmd.Length == UCCommand.RegisterLength)
                    {
                        RegisterClient(handler, cmd[1]);
                    }
                    else
                    {
                        InvalidCommand(handler, msg);
                    }
                    break;
                case UCCommand.Deregister:
                    {
                        if (cmd.Length == UCCommand.DeregisterLength)
                        {
                            int playerId;
                            if (int.TryParse(cmd[1], out playerId))
                                DeregisterClient(handler, playerId);
                            else
                            {
                                // Cannot parse to int
                                InvalidCommand(handler, msg);
                            }
                        }
                        else
                        {
                            // Command length not match
                            InvalidCommand(handler, msg);
                        }
                        break;
                    }
                default:
                    {
                        int playerId;
                        if (int.TryParse(cmd[0], out playerId))
                        {
                            // If the command is from a registered 
                            // player.
                            if (playerId < clients.Length
                                && clients[playerId] != null)
                            {
                                HandleInputCommands(
                                    handler, playerId,
                                    // Create a new command array
                                    GeneralUtilities.ArrayCopy<string>(
                                        cmd, 1, cmd.Length - 1
                                    ),
                                    msg
                                );
                            }
                            else
                                PlayerNotFound(handler, playerId);
                        }
                        else
                            // Cannot parse to int
                            InvalidCommand(handler, msg);

                        break;
                    }
            }
        }

        public void OnReceiveFail(string err)
        {

        }

        /* Implement methods from NetworkUtilities.IMessageSender */

        public void OnSendComplete()
        {

        }

        public void OnSendFail(string err)
        {

        }

        /* Interfaces / Listeners */

        public interface ICommandHandler
        {
            void Register(int playerId, string playerName);
            void Deregister(int playerId);
            void KeyDown(int playerId, string key, string extra = "");
            void Gyro(int playerId, float x, float y, float z);
            void Joystick(int playerId, float x, float y);
        }
    }
}
