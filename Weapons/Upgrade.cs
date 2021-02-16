using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Upgrade : MyNetworkBehaviour, PickUpAble
{
    protected Rigidbody2D rigid;
    protected PlayerController playerController;
    protected NetworkTransform networkTransform;

    public virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        networkTransform = GetComponent<NetworkTransform>();
    }

    public virtual void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        networkTransform.enabled = false;

        playerController = ObjectPlayerController.GetComponent<PlayerController>();
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        transform.SetParent(playerController.ArmorPointTransform);
        transform.localRotation = Quaternion.identity;

        transform.localPosition = localPosition;
        if(isServer)
            RpcCallPickup(ObjectPlayerController, localPosition);
    }
    [ClientRpc]
    protected virtual void RpcCallPickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        if(isClientOnly)
        {
            Pickup(ObjectPlayerController, localPosition);
        }
    }
}
