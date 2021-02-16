using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighJump : Upgrade
{
    public int jumpIncrease;

    

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        ObjectPlayerController.GetComponent<PlayerController>().jumpSpeed += jumpIncrease;
        Destroy(gameObject);
        if(isServer)
            RpcCallPickup(ObjectPlayerController, localPosition);
    }
}
