using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJump : Upgrade
{
    public int jumpIncrease;
    
    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        PlayerController player = ObjectPlayerController.GetComponent<PlayerController>();
        player.maxJumps += jumpIncrease;
        player.jumpsLeft = player.maxJumps;
        Destroy(gameObject);
        if(isServer)
            RpcCallPickup(ObjectPlayerController, localPosition);
    }
}
