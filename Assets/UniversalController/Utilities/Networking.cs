using System.Net;

namespace AlphaOwl.UniversalController.Utilities
{

    /// <summary>
    /// Class containing network related methods for
    /// socket connection.
    /// </summary>
    public class Networking
    {
        private const string TAG = "Networking";

        /// <summary>
        /// Fetches the local IP address of the machine.
        /// </summary>
        /// <returns>Local IP address.</returns>
        public static string GetIpAddress()
        {
            string hostname = Dns.GetHostName();

            return Dns.GetHostEntry(hostname).AddressList[0].ToString();
        }
    }

}
