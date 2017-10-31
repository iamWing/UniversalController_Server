using System.Net;
using System.Net.Sockets;

namespace AlphaOwl.UniversalController
{
    /// <summary>
    /// Main class of the library & initialises a signle 
    /// socket server as the host within the game.
    /// </summary>
    public class Server
    {
        private const int DefaultPort = 28910;
        private const int DefaultMaxConnections = 4;

        private static bool IsRunning = false;

        private static int Port;
        private static int MaxConnections;

        private Socket socketListener;
        private Socket handler;
        private IPEndPoint ipEndPoint;

        /// <summary>
        /// Create a new instance of Server that will be initialised 
        /// with specified port number & maximum connections.
        /// </summary>
        /// <param name="port">Port number of the server.</param>
        /// <param name="maxConnections">Maximum connections of the 
        /// server.</param>
        public static Server Init(int port = DefaultPort, 
                                  int maxConnections = DefaultMaxConnections)
        {
            Server server = new Server();

        }

        /// <summary>
        /// Requests the socket permission.
        /// </summary>
        private void RequestSocketPermission()
        {
            // Creates SocketPermission object for access restrictions
            SocketPermission permission = 
                new SocketPermission(
                    NetworkAccess.Accept, // Allowes to accept connections
                    TransportType.Tcp, // Defines transport types
                    "", // The IP addresses of local host
                    SocketPermission.AllPorts // Applies to all ports
                );

            // Ensures the code to have permission to access a socket
            permission.Demand();
        }
    }
}