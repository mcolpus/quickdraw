using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombProjectile : Projectile {
    public float range;
    public float pushForce;
    public float timeTillExplosion;
    public LayerMask damageLayerMask;
    public GameObject Explosion;
    public bool activateOnCollide;
    //public LayerMask activateOnImpactLayerMask;

    public override void Start()
    {
        base.Start();
        StartCoroutine(Run());
    }
    

    public override void OnTriggerEnter2D(Collider2D other)
    {
       
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isServer)
            return;

        if(activateOnCollide && collision.enabled)
        {
            Collision();
        }
    }

    public override void Collision()
    {
        RpcExplode();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, damageLayerMask);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        foreach(Collider2D c in colliders)
        {
            Damageable damagable = c.GetComponent<Damageable>();
            if(damagable != null)
            {
                damagable.Damage(damage);
            }
            Rigidbody2D r = c.GetComponent<Rigidbody2D>();
            if(r != null)
            {
                float dis = Vector2.Distance(r.transform.position, transform.position);
                if(dis == 0) continue;

                r.AddForce( ((r.transform.position - transform.position) / dis*dis ) * pushForce,ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);
    }
    [ClientRpc]
    protected void RpcExplode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, damageLayerMask);
        Instantiate(Explosion, transform.position, Quaternion.identity);
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

    IEnumerator Run()
    {
        yield return new WaitForSeconds(timeTillExplosion);
        Collision();
    }
}
