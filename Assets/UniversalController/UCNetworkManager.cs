using UnityEngine;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCNetworkManager : MonoBehaviour, UCServer.ICommandHandler
    {
        private static UCNetworkManager instance;
        private static UCServer server;

        [SerializeField] protected UCPlayer playerPrefab;

        [SerializeField] protected int portNumber;
        [SerializeField] protected int maxConnections;
        [SerializeField] protected bool debug = false;

        private UCPlayer[] players;

        void Awake()
        {
            InstantiateManager();
        }

        // Use this for initialization
        void Start()
        {
            if (server == null)
            {
                players = new UCPlayer[maxConnections];

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

        /* Implement methods from UCServer.ICommandHandler */

        public void Register(int playerId, string playerName)
        {
            players[playerId] = (UCPlayer)Instantiate(playerPrefab);
            players[playerId].OnPlayerRegister(playerId, playerName);
        }

        public void Deregister(int playerId)
        {
            players[playerId].OnPlayerDeregister();
            players[playerId] = null;
        }

        public void Gyro(int playerId, float x, float y, float z)
        {
            players[playerId].Gyro(x, y, z);
        }

        public void Joystick(int playerId, float x, float y)
        {
            players[playerId].Joystick(x, y);
        }

        public void KeyDown(int playerId, string key, string extra = "")
        {
        }

    }
}