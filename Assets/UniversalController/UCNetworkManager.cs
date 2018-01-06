using UnityEngine;

namespace AlphaOwl.UniversalController
{
    public class UCNetworkManager : MonoBehaviour, UCServer.ICommandHandler
    {
        private static UCNetworkManager instance;

        [SerializeField] protected GameObject playerPrefab;

        [SerializeField] protected int portNumber;
        [SerializeField] protected int maxConnections;
        [SerializeField] protected bool debug = false;

        private UCServer server;

        void Awake()
        {
            InstantiateManager();
        }

        // Use this for initialization
        void Start()
        {
            server = UCServer.Init(this, portNumber, maxConnections, debug);
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