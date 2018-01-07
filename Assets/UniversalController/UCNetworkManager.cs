using UnityEngine;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCNetworkManager : MonoBehaviour, UCServer.ICommandHandler
    {
        private static UCNetworkManager instance;
        private static UCServer server;

        [SerializeField] private UCPlayer playerPrefab;

        [SerializeField] private int portNumber = 28910;
        [SerializeField] private int maxConnections = 2;
        [SerializeField] private bool debug = false;

        private Dispatcher dispatcher;

        private UCPlayer[] players;

        void Awake()
        {
            InstantiateManager();

            dispatcher = new Dispatcher();
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

        void Update()
        {
            dispatcher.InvokePending();
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
            dispatcher.Invoke(() =>
            {
                players[playerId] = (UCPlayer)Instantiate(playerPrefab);
                players[playerId].OnPlayerRegister(playerId, playerName);
            });
        }

        public void Deregister(int playerId)
        {
            dispatcher.Invoke(() =>
            {
                players[playerId].OnPlayerDeregister();
                players[playerId] = null;
            });
        }

        public void Gyro(int playerId, float x, float y, float z)
        {
            dispatcher.Invoke(() =>
            {
                players[playerId].Gyro(x, y, z);
            });
        }

        public void Joystick(int playerId, float x, float y)
        {
            dispatcher.Invoke(() =>
            {
                players[playerId].Joystick(x, y);
            });
        }

        public void KeyDown(int playerId, string key, string extra = "")
        {
            dispatcher.Invoke(() =>
            {
                players[playerId].KeyDown(key, extra);
            });
        }

    }
}