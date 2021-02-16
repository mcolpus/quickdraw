using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : MyNetworkBehaviour, Holdable {
    protected Rigidbody2D rigid;
    protected NetworkTransform networkTransform;
    protected NetworkIdentity networkIdentity;

    protected PlayerController playerController;
    protected bool equiped;
    public bool IsEquiped { get { return equiped; } }

    public float ThrowSpeedMultiplier;
    public float despawnTime;//after being dropped. zero means never despawn
    protected Coroutine despawnIEnumerator;
    

    public virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        networkTransform = GetComponent<NetworkTransform>();
        networkIdentity = GetComponent<NetworkIdentity>();
    }


    public virtual void Drop()
    {
        transform.SetParent(null);
        equiped = false;
        rigid.isKinematic = false;
        networkTransform.enabled = true;

        if(isServer)
        {
            if(despawnTime != 0)
            {
                despawnIEnumerator = StartCoroutine(Despawn());
            }
            RpcCallDrop();
        }
    }
    [ClientRpc]
    void RpcCallDrop()
    {
        if(isClientOnly)
        {
            Drop();
        }
    }

    public virtual bool CanFire()
    {
        return true;
    }
    public virtual void Fire()
    {
        
    }
    protected virtual void DoFire()
    {
        //this is what happens if firing can happen
    }

    //The pickup Function will first be called on Server and then done on clients
    public virtual void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        networkTransform.enabled = false;
        //networkIdentity.localPlayerAuthority = true;
        //networkIdentity.AssignClientAuthority(_playerController.GetComponent<NetworkIdentity>().connectionToServer);

        playerController = ObjectPlayerController.GetComponent<PlayerController>();
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = 0;
        transform.SetParent(playerController.HoldTransform);
        transform.localRotation = Quaternion.identity;
        
        transform.localPosition = localPosition;
        equiped = true;

        if(isServer)
        {
            if(despawnIEnumerator != null)
                StopCoroutine(despawnIEnumerator);

            RpcCallPickup(ObjectPlayerController, localPosition);
        }
    }
    [ClientRpc]
    void RpcCallPickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        if(isClientOnly)
        {
            Pickup(ObjectPlayerController, localPosition);
        }
    }
    

    public virtual void Throw(float speed)
    {
        Drop();
        rigid.velocity = transform.right * speed * ThrowSpeedMultiplier;
        if(isServer)
            RpcCallThrow(speed);
    }
    [ClientRpc]
    void RpcCallThrow(float speed)
    {
        if(isClientOnly)
        {
            Throw(speed);
        }
    }

    protected virtual IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
