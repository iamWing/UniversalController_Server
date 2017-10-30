using System.IO;
using UnityEngine.Networking;

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

		/// <summary>
		/// Create a new instance of Server that will be initialised 
		/// with specified port number & maximum connections.
		/// </summary>
		/// <param name="port">Port number of the server.</param>
		/// <param name="maxConnections">Maximum connections of the server.</param>
		public static Server Init(int port = DefaultPort, int maxConnections = DefaultMaxConnections)
		{
			Server server = new Server();

			server.CreateHost(port, maxConnections);
		}

		private void CreateHost(int port, int maxConnections)
		{
			Port = port;
			MaxConnections = maxConnections;
		}
	}
}