using NUnit.Framework;
using AlphaOwl.UniversalController.Utilities;

/// <summary>
/// Test units for class NetworkUtilities.
/// </summary>
public class NetworkingTest {

    private const string TAG = "NetworkingTest: ";

    [Test]
	/// <summary>
	/// Confirm the IP address retrieved isn't the 
	/// loopback address.
	/// </summary>
	public void GetIpAddressTest() {
        const string localhost = "127.0.0.1";
        string ip;

        Assert.AreNotEqual(localhost, ip = NetworkUtilities.GetIPv4Address());
        DebugUtilities.Log(TAG + ip);
    }

}
