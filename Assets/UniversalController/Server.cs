using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using AlphaOwl.UniversalController.Utilities;

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

        private Socket socketListener;
        private Socket handler;
        private IPEndPoint ipEndPoint;
        private int port;
        private int maxConnections;

        /// <summary>
        /// Create a new instance of Server that will be initialised 
        /// with specified port number & maximum connections.
        /// </summary>
        /// <param name="port">Port number of the server.</param>
        /// <param name="maxConnections">Maximum connections of the 
        /// server.</param>
        /// <param name="debug">Writes log msg to console if debug == 
        /// true.</param>"> 
        public static Server Init(int port = DefaultPort, 
                                  int maxConnections = DefaultMaxConnections, 
                                  bool debug = false)
        {
            Server server = new Server();
            server.SetUp(port, maxConnections, debug);

            return server;
        }

        public bool Start()
        {
            if (IsRunning)
                return false;
            
            PerpareSocket(port);
            StartSocket(maxConnections);
            IsRunning = true;

            DebugUtilities.Log("Server started.");

            return IsRunning;
        }

        /// <summary>
        /// Sets up the customisable fields.
        /// </summary>
        /// <param name="port">Port.</param>
        /// <param name="maxConnections">Max connections.</param>
        /// <param name="debug">If set to <c>true</c> debug.</param>
        private void SetUp(int port, int maxConnections, bool debug)
        {
            this.port = port;
            this.maxConnections = maxConnections;
            DebugUtilities.Enable = debug;
        }

        /// <summary>
        /// Requests the socket permission.
        /// </summary>
        [System.Obsolete("Unity cannot recognise SocketPermission. " +
            "No alternative method is provided in this case.")]
        private void RequestSocketPermission()
        {
//            // Creates SocketPermission object for access restrictions
//            SocketPermission permission = 
//                new SocketPermission(
//                    NetworkAccess.Accept, // Allowes to accept connections
//                    TransportType.Tcp, // Defines transport types
//                    "", // The IP addresses of local host
//                    SocketPermission.AllPorts // Applies to all ports
//                );
//
//            // Ensures the code to have permission to access a socket
//            permission.Demand();
        }

        /// <summary>
        /// Perpares the socket on specified port with SocketType set as 
        /// Stream & ProtocolType set as Tcp.
        /// </summary>
        /// <param name="port">The port that the socket will be bound 
        /// to.</param>
        private void PerpareSocket(int port)
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

        private void StartSocket(int maxConnections)
        {
            try
            {
                // Places the socket in a listening state and specifies the 
                // maximum length of the pending connections queue.
                socketListener.Listen(maxConnections);

                // Begins an asynchronous operation to accept an attempt
                AsyncCallback asyncCallback = new AsyncCallback(AcceptCallback);
                socketListener.BeginAccept(asyncCallback, socketListener);

                DebugUtilities.Log("Socket is now listening on " +
                    ipEndPoint.Address + " port: " + ipEndPoint.Port);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket listener = null;

            // A new socket to handle remote host communication
            Socket handler = null;
            try
            {
                // Receiving byte array
                byte[] buffer = new byte[1024];
                // Get listening Socket object
                listener = (Socket)asyncResult.AsyncState;
                // Create new socket
                handler = listener.EndAccept(asyncResult);

                // Uses Nagle algorithm
                handler.NoDelay = false;

                // Creates one object array for passing data
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data
                handler.BeginReceive(
                    buffer,     // An array of type byte for received data
                    0,          // The zero-based position in the buffer
                    buffer.Length,  // The number of bytes to receive
                    SocketFlags.None, // Specifies send & receive behaviours
                    new AsyncCallback(ReceiveCallback), // An AsyncCallback delegate
                    obj         // Specifies information for receive operation
                );

                // Begins an asynchronous operation to accept an attempt
                AsyncCallback asyncCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(asyncCallback, listener);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Fetch a user-defined object that contains information
                object[] obj = new object[2];
                obj = (object[])asyncResult.AsyncState;

                // Received byte array
                byte[] buffer = (byte[])obj[0];

                // A socket to handle remote host communication
                handler = (Socket)obj[1];

                // Received message
                string content = string.Empty;
               
                // The number of bytes received
                int bytesRead = handler.EndReceive(asyncResult);

                if (bytesRead > 0)
                {
                    content += Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Check for the end-of-connectino tag. If it is not there, 
                    // read more data.
                    if (content.IndexOf("<EOC>") > -1)
                    {
                        // Convert byte array to string
                        string str = content.Substring(0, 
                                         content.LastIndexOf("<EOC>"));

                        DebugUtilities.Log("Data received with EOC tag.");
                        DebugUtilities.Log("Data: " + str);
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] bufferNew = new byte[1024];
                        obj[0] = bufferNew;
                        obj[1] = handler;
                        handler.BeginReceive(
                            bufferNew, 
                            0, 
                            bufferNew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback),
                            obj
                        );
                    }

                    DebugUtilities.Log(content);
                    SendMsg();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                // A socket which has sent the data to remote host
                Socket handler = (Socket)asyncResult.AsyncState;

                // The number of bytes sent to the socket
                int bytesSend = handler.EndSend(asyncResult);
                DebugUtilities.Log("Sent " + bytesSend + " to client.");
            }
            catch (Exception ex)
            {
                
            }
        }

        private void SendMsg()
        {
            try
            {
                // Prepares the reply message
                string replyMsg = "Reply from server";
                byte[] byteData = Encoding.ASCII.GetBytes(replyMsg);

                // Sends data asynchronously to a connected Socket
                handler.BeginSend(byteData, 0, byteData.Length, 0, 
                    new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}