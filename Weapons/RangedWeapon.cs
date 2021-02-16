using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangedWeapon : Weapon {
    protected Transform firePos;


    public bool dropOnEmpty = true;
    public GameObject projectile;
    public int ammo;
    public float coolDown;
    public bool inheritVelocity;
    public bool destroyOnUse = false;

    public MyAudio.AUDIOCLIPS audioClipOnFire;

    protected float LastShotFired;//keeps track of the time at which they most recently fired

    public override void Awake()
    {
        base.Awake();
        firePos = transform.Find(Tags.firePos);
    }

    public override bool CanFire()
    {
        return Time.time - LastShotFired > coolDown && ammo > 0;
    }
    public override void Fire()
    {
        if(Time.time - LastShotFired > coolDown && ammo>0)
        {
            DoFire();
        }
    }
    protected override void DoFire()
    {
        base.DoFire();
        //this is what happens if firing can happen
        LastShotFired = Time.time;
        ammo--;
        MyAudio.current.PlayClip(audioClipOnFire);
        GameObject obj = Instantiate(projectile, firePos.position, firePos.rotation);
        obj.GetComponent<Projectile>().Initiate(inheritVelocity ? transform.parent.GetComponentInParent<Rigidbody2D>().velocity : Vector2.zero, gameObject, playerController);

        NetworkServer.Spawn(obj);

        if(destroyOnUse)
        {
            playerController.ForceDropWeapon();
            Destroy(gameObject);
        }

        if(dropOnEmpty && ammo <= 0)
        {
            playerController.ForceDropWeapon();
        }
    }

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);

        LastShotFired = Time.time + 0.1f;//so you dont fire straight away
    }
}
