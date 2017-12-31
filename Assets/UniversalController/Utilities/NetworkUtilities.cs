using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        /* Socket connections related */

        private static IMessageReceiver messageReceiver;

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

        // Callbacks for socket connection

        /// <summary>
        /// Callback of the asynchronous socket while 
        /// message received from client.
        /// </summary>
        /// <param name="ar">Result of the async task.</param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            string content = string.Empty;

            // Retrieve the state object and the handler socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data
                // received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead
                ));

                // Check for end-of-content tag. If it is not there, 
                // read more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOC>") > -1)
                {
                    // All the data has been read from the client. 
                    // Pass the content to the listener.
                    messageReceiver.OnReceiveComplete(content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(
                        state.buffer, 0, state.BufferSize, 0, 
                        new AsyncCallback(ReceiveCallback), state
                    );
                }
            }
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

        // Inner classes

        /// <summary>
        /// State object for reading data from socket clients 
        /// asynchronously.
        /// </summary>
        private class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public readonly int BufferSize;
            // Receive buffer.
            public byte[] buffer;
            // Received data string.
            public StringBuilder sb = new StringBuilder();

            /// <summary>
            /// Constructor of the state object.
            /// </summary>
            /// <param name="bufferSize">Size of receive buffer.</param>
            public StateObject(int bufferSize)
            {
                BufferSize = bufferSize;
                buffer = new byte[BufferSize];
            }
        }
    }

}
