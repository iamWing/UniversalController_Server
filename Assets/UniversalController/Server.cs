using System.Net;
using System.Net.Sockets;
using UnityEngine;

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

        /// <summary>
        /// Opens the socket on specified port with SocketType set as 
        /// Stream & ProtocolType set as Tcp.
        /// </summary>
        /// <param name="port">The port that the socket will be bound 
        /// to.</param>
        private void OpenSocket(int port)
        {
            try
            {
                // Reset Socket object
                socketListener = null;

                // Resolves a host name to an IPHostEntry instance
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets 1st IP address associated with a localhost
                IPAddress ipAddress = ipHost.AddressList[0];

                // Creates a network endpoint
                ipEndPoint = new IPEndPoint(ipAddress, port);

                // Creates a Socket object to listen the incoming connection
                socketListener = 
                new Socket(
                    ipAddress.AddressFamily, 
                    SocketType.Stream, 
                    ProtocolType.Tcp
                );

                // Associates a socket with a local endpoint
                socketListener.Bind(ipEndPoint);
            }
            catch (SocketException ex)
            {
                throw;
            }
        }
    }
}