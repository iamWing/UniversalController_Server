using System.Net;
using System.Net.Sockets;

namespace AlphaOwl.UniversalController.Utilities
{

    /// <summary>
    /// Class containing network related methods for
    /// socket connection.
    /// </summary>
    public class NetworkUtilities
    {
        private const string TAG = "Network Utilities";

        /// <summary>
        /// Fetches the local IP address of the machine.
        /// </summary>
        /// <returns>Local IP address.</returns>
        public static string GetIpAddress()
        {
            string hostname = Dns.GetHostName();

            return Dns.GetHostEntry(hostname).AddressList[0].ToString();
        }

        /// <summary>
        /// Initialises the server socket and bind it on specified 
        /// IP & port.
        /// </summary>
        /// <param name="ip">IP address to bind to the socket.</param>
        /// <param name="port">Port to bind to the socket.</param>
        /// <returns>A Socket object that binds to specified IP & 
        /// port.</returns>
        public static Socket InitSocketServer(string ip, int port)
        {
            // Parses IP string to IPAddress object.
            IPAddress ipAddr = IPAddress.Parse(ip);
            // Creates a network endpoint.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creates a Socket object to listen the
            // incoming connection.
            Socket workSocket = new Socket(
                ipAddr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            // Associates a socket with a local endpoint.
            workSocket.Bind(localEndPoint);

            DebugUtilities.Log(TAG + ": Server started");

            return workSocket;
        }

        // Interfaces / Listeners

        /// <summary>
        /// Interface to receive callback upon received the 
        /// whole message from remote socket client.
        /// </summary>
        public interface IMessageReceiver
        {
            /// <summary>
            /// Implement this method to receive callback 
            /// after the whole message is received from 
            /// remote socket client.
            /// </summary>
            /// <param name="msg">Received message from 
            /// remote socket client.</param>
            void OnReceiveComplete(string msg);
        }
    }

}
