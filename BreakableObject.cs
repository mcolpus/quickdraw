using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, Damageable {
    public GameObject breakableObject;
    public int startHealth;
    public int health;
    public bool broken;
    public bool Dead { get { return broken; } }
    public int Health { get { return health; } }

    public GameObject Cracks;
    public int healthToShowCracks;

    public bool breakOnStanding;//break when the player jumps on it
    public float timeToBreakFromStanding;

    public GameObject explosion;

    public bool respawn;
    public float respawnTime;

    public GameObject activateOnBreak;

    public event standardAnnouncement OnDeath;
    public event intAnnouncement OnHit;

    
    

    public void Damage(int amount)
    {
        health -= amount;
        if(health <= healthToShowCracks && Cracks!=null)
            Cracks.SetActive(true);

        if(OnHit != null)
            OnHit(health);
        if(health <= 0)
            Kill();
    }

    public void Heal(int amount)
    {
        if(!broken)
            health += amount;
    }

    public void Kill()
    {
        if(broken)
            return;

        health = 0;
        broken = true;
        if(explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        if(activateOnBreak != null)
            activateOnBreak.SetActive(true);

        if(OnDeath != null)
            OnDeath();

        breakableObject.SetActive(false);
        StopAllCoroutines();

        breaking = false;
        if(respawn)
            StartCoroutine(RespawnInTime());
    }

    public void Resurrect()
    {
        health = startHealth;
        broken = false;
        breakableObject.SetActive(true);
        if(Cracks!=null)
            Cracks.SetActive(false);
        if(activateOnBreak != null)
            activateOnBreak.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == Tags.player && breakOnStanding && !broken)
        {
            if(!breaking)
                StartCoroutine(BreakInTime());
        }
    }

    bool breaking = false;
    IEnumerator BreakInTime()
    {
        print(1);
        breaking = true;
        yield return new WaitForSeconds(timeToBreakFromStanding);
        Damage(1);
        breaking = false;
    }

    IEnumerator RespawnInTime()
    {
        yield return new WaitForSeconds(respawnTime);
        Resurrect();
    }
}
