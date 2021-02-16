using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluntForce : MonoBehaviour {
    public int damage;
    public float minSpeedForDamage;
    public float stunTime;//0 means no stun

    private Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Damageable dmg = collision.gameObject.GetComponentInParent<Damageable>();
        if(dmg!=null && rigid.velocity.magnitude > minSpeedForDamage)
        {
            dmg.Damage(damage);
        }
    }
	
}
