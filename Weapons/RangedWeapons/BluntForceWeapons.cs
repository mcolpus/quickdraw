using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluntForceWeapons : Weapon {
    protected float timeAtPickup;//to make a small delay
    public GameObject colliderObject;
    
    public int damage;
    public float minSpeedForDamage;
    public float stunTime;//0 means no stun

    public float throwerSafetyDistance;//thrower doesn't get hurt unless it travels more than this far away, use playercontroller to keep track of this

    public bool onlyDamageFromFlight;
    protected bool inFlight = false;



    public override void Drop()
    {
        base.Drop();
        colliderObject.SetActive(true);
    }

    public override void Fire()
    {
        if(Time.time - timeAtPickup > 0.2f)
        {
            playerController.ForceDropWeapon();
            Throw(playerController.throwSpeed);
        }

    }
    public override void Throw(float speed)
    {
        base.Throw(speed);
        inFlight = true;
    }

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);
        timeAtPickup = Time.time;
        colliderObject.SetActive(false);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.enabled || !isServer)
            return;

        Damageable dmg = collision.gameObject.GetComponentInParent<Damageable>();
        if(dmg != null && dmg !=(Damageable) playerController && rigid.velocity.magnitude > minSpeedForDamage)
        {
            if(inFlight || !onlyDamageFromFlight)
                dmg.Damage(damage);
        }
        if(inFlight)
        {
            if(dmg==null || (dmg != null && dmg != (Damageable)playerController))
            {
                inFlight = false;
            }
        }   
    }

    void Update()
    {
        if(playerController != null && isServer)
        {
            if(Vector2.Distance(transform.position, playerController.transform.position) > throwerSafetyDistance)
                playerController = null;
        }
    }
}
