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

            name = playerName;

            OnPlayerRegisterAction();
        }

		public virtual void OnPlayerDeregister()
		{
            DebugUtilities.Log("Player " + playerName + " deregistered." +
            "\nPlayer ID: " + playerId, this);

            Destroy(gameObject);
        }

        public virtual void KeyDown(string key, string extra = "")
        {
            DebugUtilities.Log(Command.KeyDown + ": " + key, this);
        }

        public virtual void Gyro(float x, float y, float z)
        {
            DebugUtilities.Log(
                Command.Gyro + ": (x: " + x + ", y: " + y +
				 ", z: " + z + ")", this
			);
        }

        public virtual void Joystick(float x, float y)
        {
            DebugUtilities.Log(
				Command.Joystick + ": (x: " + x + ", y: " + y + ")", this);
        }

        protected virtual void OnPlayerRegisterAction()
        {
            DebugUtilities.Log("Player " + playerName + " registered." +
            "\nPlayer ID: " + playerId, this);
        }
    }
}