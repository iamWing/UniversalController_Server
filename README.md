# Universal Controller Server

Universal Controller is a SDK that allows developers to use their customised 
controller on mobile devices or tailor made hardware controllers with Unity. 
This package doesn't limited the mobile side application to be an Unity app. 
It's aimed to support native Android, iOS apps, and single board computers like 
Arduino boards & Raspberry Pi for the controller side.

This repo is the server side Unity package which hosts a socket server within 
the Unity app by using native .NET socket APIs.

For the client side library, please refer to 
[here](https://github.com/iamWing/uc-client-java) for Java based client and 
[here](https://github.com/iamWing/uc-client-android) for Android client library.

---
## Getting start

To set up the server, follow the following instructions:

1. Create an empty GameObject in the scene you want to start up the server 
and attach component _UCNetworkManager_ to it.
2. Set the port number and maximum connections in _UCNetworkManager_.
3. Create a new C# script as a child class of _UCPlayer_, override methods from 
_UCPlayer_ (see _SamplePlayer_.cs for reference) and implement your own logics for 
each command received.
4. Attach the script you just created to your player object, then make that as 
a prefab.
5. Set the player prefab in _UCNetworkManager_.
6. You are now good to go.

There is a sample scene included in the _Sample_ folder with everything set up 
properly that you can use it as an example.

The server will shutdown by itself on application quit. Alternatively you can 
call UCNetworkManager.ServerShutDown() to shutdown the server anytime you 
want, the server will then shutdown after all data is sent and received.

To test the server, you can use 
[this](https://gist.github.com/iamWing/45ce5432acacf75fbb3b98c0b4c8e0fe) Java 
cmd tool. It is a fairly simple cmd line program to test out your server. Just 
type in your server IP and type in the commands. One thing to mention is this 
tool will throw the IOException and terminate itself if there is any error on 
the connection between the tool and the socket server.

---
## Commands

Below are the formats of each command that will be handled by the server once 
received from client:

__Register player__

`REGISTER:{player_id}`

__Deregister player__

`DEREGISTER:{player_id}`<br>
_* The client who registered that player will be disconnected._

__Key pressed__

`{player_id}:KEY_DOWN:{custom_key}`

_or_

`{player_id}:KEY_DOWN:{custom_key}:{extra_content}`

__Joystick data__

`{player_id}:JOYSTICK:{x}:{y}`

__Gyro data__

`{player_id}:GYRO:{x}:{y}:{z}`

___Type of parameters___

- _player\_id : int_
- _custom\_key : string_
- _extra\_content : string_
- _x : float_
- _y : float_
- _z : float_
- ___value of float x, y, z must be between -1.0f & 1.0f___

---
## Version history

__v2.0.1__

- Closes the socket when disconnected.

__v2.0.0__

- Reimplemented socket server with better structure and resources management.
- Added function GetIPv4Address() in NetworkUtilities for fetching the local 
IPv4 address.
- Implemented callbacks for data received from the clients.
- Introduces UC-Commands (Register, Deregister, KeyDown, Joystick, Gyro).
- Added component UCNetworkManager & UCPlayer.
- Includes sample scene.

__v1.0.0-alpha *pre-release*__

 - Set up socket server in Unity.
 - Includes test script for starting the server.

## License & copyright

Copyright (c) 2018 Wing Chau & AlphaOwl.co.uk
<br />
All rights reserved.

This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
