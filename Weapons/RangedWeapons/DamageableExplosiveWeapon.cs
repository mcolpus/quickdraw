using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableExplosiveWeapon : ExplosiveForceWeapons, Damageable
{
    public int health;

    public bool Dead { get { return false; } }
    public int Health { get { return health; } }

    public event standardAnnouncement OnDeath;
    public event intAnnouncement OnHit;

    public void Damage(int amount)
    {
        if(health <= 0)
            return;

        health -= amount;
        if(OnHit != null)
            OnHit(health);

        if(health <= 0)
            Kill();
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void Kill()
    {
        health = 0;
        if(OnDeath != null)
            OnDeath();

        Explode();
    }

    public void Resurrect()
    {
        throw new NotImplementedException();
    }
}
