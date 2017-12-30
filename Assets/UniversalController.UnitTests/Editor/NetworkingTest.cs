using UnityEngine;
using NUnit.Framework;
using AlphaOwl.UniversalController.Utilities;

/// <summary>
/// Test units for class Networking.
/// </summary>
public class NetworkingTest {

	[Test]
	/// <summary>
	/// Confirm the IP address retrieved isn't the 
	/// loopback address.
	/// </summary>
	public void GetIpAddressTest() {
        const string localhost = "127.0.0.1";

        Assert.AreNotEqual(localhost, Networking.GetIpAddress());
        Debug.Log(Networking.GetIpAddress());
    }

}
