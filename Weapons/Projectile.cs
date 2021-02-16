using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : MyNetworkBehaviour {
    protected Rigidbody2D rigid;
    protected GameObject sourceWeapon;
    protected PlayerController sourcePlayer;

    public float speed;
    public int damage = 1;

    public float verticalSpeed;
    public float inheritVelocityMultiplier;
    

    public virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        
        rigid.velocity +=  (Vector2)transform.right * speed + (Vector2)Vector3.up*verticalSpeed;
    }

    public virtual void Initiate(Vector2 inheritedVelocity, GameObject weapon, PlayerController playerController)
    {
        rigid.velocity += inheritedVelocity * inheritVelocityMultiplier;
        sourceWeapon = weapon;
        sourcePlayer = playerController;
    }


    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(!isServer)
            return;

        Damageable dmg = other.GetComponentInParent<Damageable>();
        if(dmg != null && !dmg.Dead)
        {
            dmg.Damage(damage);
            Collision();
        }
        else if(!other.isTrigger)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void Collision()
    {
        Destroy(this.gameObject);
    }
}
