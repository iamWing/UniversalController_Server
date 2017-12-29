using UnityEngine;
using AlphaOwl.UniversalController;

public class TestScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Server server = Server.Init(debug: true);
        server.Start();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }
}
