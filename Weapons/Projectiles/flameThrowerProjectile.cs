using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flameThrowerProjectile : Projectile {
    public DamageArea damageArea;

    public override void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    public override void Initiate(Vector2 inheritedVelocity, GameObject weapon, PlayerController playerController)
    {
        base.Initiate(inheritedVelocity, weapon, playerController);
        damageArea.ignore = playerController.gameObject;
        

    }

}
