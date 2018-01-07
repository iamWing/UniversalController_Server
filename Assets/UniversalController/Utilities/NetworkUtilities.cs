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
        private const string TAG = "NetworkUtilities: ";

        // The end tag of the messages.
        private const string EndTag = "<EOC>";

        // The null value for int field.
        private const int OptionalInt = -1;

        /// <summary>
        /// Fetches the local IP address of the machine.
        /// </summary>
        /// <returns>Local IP address.</returns>
        public static string GetIPv4Address()
        {
            string hostname = Dns.GetHostName();

            int addressLength = Dns.GetHostEntry(hostname).AddressList.Length;

            string ipv4Addr = "127.0.0.1";

            for (int i = 0; i < addressLength; i++)
            {
                string ip =
                    Dns.GetHostEntry(hostname).AddressList[i].ToString();

                if (ip.Split('.').Length == 4)
                    ipv4Addr = ip;
            }

            return ipv4Addr;
        }

        /* Socket connections related */

        private static IMessageReceiver messageReceiver;
        private static IMessageSender messageSender;

        /// <summary>
        /// Initialises the server socket and bind it on specified 
        /// IP & port.
        /// </summary>
        /// <param name="ip">IP address to bind to the socket.</param>
        /// <param name="port">Port to bind to the socket.</param>
        /// <param name="bufferSize">Optional size of receive 
        /// buffer. Default buffer size is 1024.</param>
        /// <returns>A Socket object that binds to specified IP & 
        /// port.</returns>
        public static Socket InitSocketServer(string ip, int port,
        int bufferSize = OptionalInt)
        {
            // Parses IP string to IPAddress object.
            IPAddress ipAddr = IPAddress.Parse(ip);
            // Creates a network endpoint.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Set buffer size
            if (bufferSize != OptionalInt)
                StateObject.bufferSize = bufferSize;

            // Creates a Socket object to listen the
            // incoming connection.
            Socket listener = new Socket(
                ipAddr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            // Associates a socket with a local endpoint.
            listener.Bind(localEndPoint);

            DebugUtilities.Log(TAG + "Server started");

            return listener;
        }

        /// <summary>
        /// Places the socket in a listening state and begins 
        /// to accept connections.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="receiver">An instance of 
        /// NetworkUtilities.IMessageReceiver that handles the 
        /// callbacks for receiving data from remote socket client.</param>
        /// <param name="sender">And instance of 
        /// NetworkUtilities.IMessageSender that handles the 
        /// callbacks for sending data to remote socket client.</param>
        /// <param name="backlog">The maximum length of the 
        /// pending connections queue. Default value is 10.</param>
        public static void StartListening(Socket socket,
        IMessageReceiver receiver, IMessageSender sender,
        int backlog = 10)
        {
            messageReceiver = receiver;
            messageSender = sender;

            // Listen for incoming connections
            socket.Listen(backlog);
            // Start an asynchronous socket to listen for 
            // connections.
            socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

            // For logging purpose.
            string localAddr = socket.LocalEndPoint.ToString();
            DebugUtilities.Log(
                TAG + "Server is now listening on " + localAddr
            );
        }

        /// <summary>
        /// Send data to remote socket client.
        /// </summary>
        /// <param name="handler">The socket that handles the 
        /// message delivery.</param>
        /// <param name="data">The data that needs to be sent.</param>
        public static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using 
            // ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data + EndTag);

            // Begin sending the data to the remote socket 
            // client.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        /// <summary>
        /// Shutdown the specificed socket.
        /// </summary>
        /// <param name="socket">The socket that needs to be 
        /// shutdown.</param>
        public static void ShutdownSocket(Socket socket)
        {
            if (socket.Connected)
            {
                DebugUtilities.Log("Closing socket on port " +
                socket.LocalEndPoint);

                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(false);
            }
        }

        // Callbacks for socket connection

        /// <summary>
        /// Callback of the asynchronous socket while the 
        /// client connection request has been accepted.
        /// </summary>
        /// <param name="ar">Result of the async</param>
        private static void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(
                state.buffer, 0, StateObject.bufferSize, 0,
                new AsyncCallback(ReceiveCallback), state
            );

            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }

        /// <summary>
        /// Callback of the asynchronous socket while 
        /// message received from client.
        /// </summary>
        /// <param name="ar">Result of the async task.</param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            string content = string.Empty;

            try
            {
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
                    if (content.IndexOf(EndTag) > -1)
                    {
                        // All the data has been read from the client. 
                        string trimmedContent =
                            content.Substring(0, content.LastIndexOf(EndTag));

                        // Pass the content to the listener.
                        messageReceiver.OnReceiveComplete(
                            handler, trimmedContent);

                        // Clean state data string
                        state.sb = new StringBuilder();
                    }
                    //  Continue to receive data
                    handler.BeginReceive(
                        state.buffer, 0, StateObject.bufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state
                    );
                }
            }
            catch (Exception ex)
            {
                messageReceiver.OnReceiveFail(ex.Message);
            }
        }

        /// <summary>
        /// Callback of the asynchronous socket while 
        /// message is sent to the client.
        /// </summary>
        /// <param name="ar">Result of the async task.</param>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                DebugUtilities.Log(
                    TAG + "Send " + bytesSent + " bytes to client."
                );

                messageSender.OnSendComplete();
            }
            catch (Exception ex)
            {
                messageSender.OnSendFail(ex.Message);
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
            /// <param name="handler">Socket that connects 
            /// with the client.</param>
            /// <param name="msg">Received message from 
            /// remote socket client.</param>
            void OnReceiveComplete(Socket handler, string msg);

            /// <summary>
            /// Implement this method to receive callback 
            /// when the message receive fails.
            /// </summary>
            /// <param name="err">Error message. Usually 
            /// the exception message.</param>
            void OnReceiveFail(string err);
        }

        /// <summary>
        /// Interface to receive callback after a message 
        /// has been sent to the remote socket client.
        /// </summary>
        public interface IMessageSender
        {
            /// <summary>
            /// Implement this method to receive callback 
            /// when message has been sent to the remote 
            /// socket client.
            /// </summary>
            void OnSendComplete();

            /// <summary>
            /// Implement this method to receive callback
            /// when the message fails to be sent to the 
            /// remote socket client.
            /// </summary>
            /// <param name="err">Error message. Usually 
            /// the exception message.</param>
            void OnSendFail(string err);
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
            // Size of receive buffer. Default = 1024.
            public static int bufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[bufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }
    }

}
