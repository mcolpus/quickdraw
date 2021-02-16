using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkBehaviour : NetworkBehaviour {

	protected bool isClientOnly
    {
        get
        {
            return isClient && !isServer;
        }
    }
}
