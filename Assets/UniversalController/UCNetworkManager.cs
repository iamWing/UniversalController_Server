using UnityEngine;

namespace AlphaOwl.UniversalController
{
    public class UCNetworkManager : MonoBehaviour, UCServer.ICommandHandler
    {
        [SerializeField] protected GameObject playerPrefab;

        [SerializeField] protected int portNumber;
        [SerializeField] protected int maxConnections;
        [SerializeField] protected bool debug = false;

        private UCServer server;

        // Use this for initialization
        void Start()
        {
            server = UCServer.Init(this, portNumber, maxConnections, debug);
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