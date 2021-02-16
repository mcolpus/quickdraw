using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NerfBullet : Projectile {
    public float stunTime;
    public float stunVelocity;
    private bool firstCollision = true;

    public override void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Stunnable stunnable = collision.collider.GetComponentInParent<Stunnable>();
        
        if(stunnable != null && (firstCollision || rigid.velocity.magnitude > stunVelocity))
        {
            stunnable.Stun(stunTime);
        }
        firstCollision = false;
    }
}
