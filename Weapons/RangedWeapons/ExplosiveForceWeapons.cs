using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosiveForceWeapons : BluntForceWeapons
{
    public int explosionDamage;
    public float explosionChance;
    public float range;
    public float pushForce;
    public GameObject explosion;
    public LayerMask damageLayerMask;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.enabled || !isServer)
            return;

        Damageable dmg = collision.gameObject.GetComponentInParent<Damageable>();
        bool isPlayer = dmg != null && dmg == (Damageable)playerController;

        if(inFlight || !onlyDamageFromFlight)
        {
            if(!isPlayer && rigid.velocity.magnitude > minSpeedForDamage)
            {
                
                if(dmg != null)
                {
                    dmg.Damage(damage);
                }
                Explode();
            }
        }
        if(inFlight)
        {
            if(dmg == null || (dmg != null && dmg != (Damageable)playerController))
            {
                inFlight = false;
            }
        }


    }

    
    protected void Explode()
    {
        if(Random.Range(0,1f) < explosionChance)
        {
            RpcExplode();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, damageLayerMask);
            Instantiate(explosion, transform.position, Quaternion.identity);
            foreach(Collider2D c in colliders)
            {
                Damageable damagable = c.GetComponent<Damageable>();
                if(damagable != null)
                {
                    damagable.Damage(explosionDamage);
                }
                Rigidbody2D r = c.GetComponent<Rigidbody2D>();
                if(r != null)
                {
                    float dis = Vector2.Distance(r.transform.position, transform.position);
                    if(dis == 0) continue;

                    r.AddForce(((r.transform.position - transform.position) / dis * dis) * pushForce, ForceMode2D.Impulse);
                }
            }
            Destroy(gameObject);
        }
    }
    [ClientRpc]
    protected void RpcExplode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, damageLayerMask);
        Instantiate(explosion, transform.position, Quaternion.identity);
        foreach(Collider2D c in colliders)
        {
            
            Rigidbody2D r = c.GetComponent<Rigidbody2D>();
            if(r != null)
            {
                float dis = Vector2.Distance(r.transform.position, transform.position);
                if(dis == 0) continue;

                r.AddForce(((r.transform.position - transform.position) / dis * dis) * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
