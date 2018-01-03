using System.Net.Sockets;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCServer : NetworkUtilities.IMessageReceiver,
    NetworkUtilities.IMessageSender
    {
        // Default variables.
        private const int DefaultPort = 28910;
        private const int DefaultMaxConnections = 4;

        private Socket serverSocket;
        private string[] players; // Connected players.

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
