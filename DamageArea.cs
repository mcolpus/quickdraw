using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour {
    public int damage;
    public bool active;
    public GameObject ignore;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if(active && dmg != null && !dmg.Dead && !transform.IsChildOf(other.transform))
        {
            if(ignore != null && other.transform.IsChildOf(ignore.transform))
                return;

            dmg.Damage(damage);
            Collision();
        }
    }

    protected virtual void Collision()
    {
        active = false;
    }

    public void SetActive(bool value)
    {
        active = value;
    }

}
