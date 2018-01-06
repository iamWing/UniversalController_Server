using UnityEngine;
using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public abstract class UCPlayer : MonoBehaviour
    {
        protected int playerId;
        protected string playerName;

        public void OnPlayerRegister(int playerId, string playerName)
        {
            this.playerId = playerId;
            this.playerName = playerName;
        }

        public virtual void KeyDown(string key, string extra = "")
        {
            DebugUtilities.Log("KEYDOWN: " + key, this);
        }

        public virtual void Gyro(float x, float y, float z)
        {
            DebugUtilities.Log(
                "GYRO: (x: " + x + ", y: " + y + ", z: " + z + ")", this);
        }

        public virtual void Joystick(float x, float y)
        {
            DebugUtilities.Log(
				"JOYSTICK: (x: " + x + ", y: " + y + ")", this);
        }

        protected virtual void OnPlayerRegisterAction()
        {
            DebugUtilities.Log("Player " + playerName + " registered." +
            "\nPlayer ID: " + playerId, this);
        }
    }
}