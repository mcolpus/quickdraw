using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firePower : Upgrade
{
    public float duration;
    public float range;
    public Animator flame;
    private bool active;

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);

        flame.enabled = true;
        active = true;
        Destroy(gameObject, duration);
    }
    

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(!isServer)
            return;

        if(active)
        {
            Damageable dmg = other.GetComponentInParent<Damageable>();
            if(dmg != null && Vector2.Distance(transform.position, other.transform.position) < range)
            {
                dmg.Damage(1);
            }
        }
    }

}
