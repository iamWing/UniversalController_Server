using UnityEngine;
using AlphaOwl.UniversalController;

public class SamplePlayer : UCPlayer
{
    [SerializeField] private Vector3 spawnPoint;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

	// Override callbacks from parent. Call the base methods 
	// for debug usage.

	protected override void OnPlayerRegisterAction()
	{
        base.OnPlayerRegisterAction();

        // Sample action
        transform.position = spawnPoint;
        Debug.Log("Player Spawned", this);
    }

	public override void OnPlayerDeregister()
	{
		// Remove this line if you don't want to destroy 
		// the player gameobject when player deregister.
        base.OnPlayerDeregister(); 
    }

    public override void KeyDown(string key, string extra = "")
    {
        base.KeyDown(key, extra);

        // Switch statement is recommended here to 
        // response on different key inputs.
    }

    public override void Gyro(float x, float y, float z)
	{
		// Values of x, y, z are between 1 and -1.

        base.Gyro(x, y, z);
    }

	public override void Joystick(float x, float y)
	{
		// Values of x, y are between 1 and -1.

        base.Joystick(x, y);
    }
}
