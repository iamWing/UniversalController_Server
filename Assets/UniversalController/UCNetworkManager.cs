using UnityEngine;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCNetworkManager : MonoBehaviour, UCServer.ICommandHandler
    {
        private static UCNetworkManager instance;
        private static UCServer server;

        [SerializeField] protected GameObject playerPrefab;

        [SerializeField] protected int portNumber;
        [SerializeField] protected int maxConnections;
        [SerializeField] protected bool debug = false;

        void Awake()
        {
            InstantiateManager();
        }

        // Use this for initialization
        void Start()
        {
            if (server == null)
            {
                server = UCServer.Init(this, portNumber,
                maxConnections, debug);
            }
        }

        void OnApplicationQuit()
        {
            ServerShutdown();
        }

        /// <summary>
        /// Shutdown the sever.
        /// </summary>
        public static void ServerShutdown()
        {
            if (server != null)
            {
                server.Shutdown();
                DebugUtilities.Log("Server is now shutting down...");
            }
            else
            {
                DebugUtilities.Log(msg: "Server does not exists.",
                type: Utilities.LogType.Warning);
            }
        }

        /// <summary>
        /// Instantiate an instance of UCNetworkManager and 
        /// set it not to destroy on scene load.
        /// </summary>
        private void InstantiateManager()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else if (this != instance)
            {
                // Prevent Unity duplicates gameobject when 
                // scene reloads.
                Destroy(gameObject);
            }
        }

        /* Override methods from UCServer.ICommandHandler */

        public void Register(int playerId, string playerName)
        {
        }

        public void Deregister(int playerId)
        {
        }

        public void Gyro(int playerId, float x, float y, float z)
        {
        }

        public void Joystick(int playerId, float x, float y)
        {
        }

        public void KeyDown(int playerId, string key, string extra = "")
        {
        }

    }
}