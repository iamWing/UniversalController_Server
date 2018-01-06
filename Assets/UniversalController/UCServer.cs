using System;
using System.Net.Sockets;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    /// <summary>
    /// Prefix of the commands used to communicate with 
    /// the clients.
    /// </summary>
    public struct Command
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

    public class UCServer : NetworkUtilities.IMessageReceiver,
    NetworkUtilities.IMessageSender
    {
        // The only instance of UCServer should have.
        private static UCServer instance = null;

        // Default variables.
        private const int DefaultPort = 28910;
        private const int DefaultMaxConnections = 4;

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
                    SendMsg(i, Command.ServerShutDown);
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
                            NetworkUtilities.GetIpAddress(),
                            port);

            clients = new Socket[maxConn];
        }

        private void HandleInputCommands(Socket socket, int playerId,
                string[] cmd, string fullCmd)
        {
            switch (cmd[0])
            {
                case Command.Gyro:
                    // PLayer ID has been taken out
                    if (cmd.Length == Command.GyroLength - 1)
                        GyroCommand(
                            socket, playerId,
                            GeneralUtilities.ArrayCopy<string>(
                                cmd, 1, cmd.Length
                            ),
                            fullCmd
                        );
                    else
                        // Command length not match.
                        InvalidCommand(socket, fullCmd);

                    break;
                case Command.Joystick:
                case Command.KeyDown:
                default:
                    break;
            }
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
                    SendMsg(i, Command.PlayerId + i);

                    DebugUtilities.Log(playerName +
                    "registered with player ID " + i);

                    return;
                }
            }

            DebugUtilities.Log("Unable to register player " + playerName +
            ". Maximum player numbers reached.");
            NetworkUtilities.Send(socket, Command.ServerFull);
        }

        private void DeregisterClient(Socket socket, int playerId)
        {
            if (clients[playerId] != null)
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
            float[] pos = new float[Command.GyroLength - 2];

            // Index starts from 1 to skip the player ID
            for (int i = 1; i < cmd.Length; i++)
            {
                float result;

                if (float.TryParse(cmd[i], out result))
                    pos[i - 1] = result;
                else
                {
                    // Cannot parse to float
                    InvalidCommand(socket, fullCmd);
                    return;
                }
            }

            cmdHandler.Gyro(playerId, x: pos[0], y: pos[1], z: pos[2]);
        }

        /* Error handling */

        private void PlayerNotFound(Socket socket, int playerId)
        {
            DebugUtilities.Log(
                                msg: Command.PlayerNotFound + " {" + Command.PlayerId +
                                playerId + "}",
                                type: LogType.Error
                            );

            NetworkUtilities.Send(socket, Command.PlayerNotFound);

        }

        private void InvalidCommand(Socket socket, string cmd)
        {
            DebugUtilities.Log(
                msg: Command.InvalidCmd + " {" + cmd + "}",
                type: LogType.Error
            );

            NetworkUtilities.Send(socket, Command.InvalidCmd);
        }

        /* Implement methods from NetworkUtilities.IMessageReceiver */

        public void OnReceiveComplete(Socket handler, string msg)
        {
            string[] cmd = msg.Split(Command.Separator);

            switch (cmd[0])
            {
                case Command.Register:
                    if (cmd.Length == Command.RegisterLength)
                    {
                        RegisterClient(handler, msg);
                    }
                    else
                    {
                        InvalidCommand(handler, msg);
                    }
                    break;
                case Command.Deregister:
                    {
                        if (cmd.Length == Command.DeregisterLength)
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
                            if (clients[playerId] != null)
                            {
                                HandleInputCommands(
                                    handler, playerId,
                                    // Create a new command array
                                    GeneralUtilities.ArrayCopy<string>(
                                        cmd, 1, cmd.Length
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
