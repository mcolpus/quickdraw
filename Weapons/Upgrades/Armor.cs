using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Upgrade {
    public int healthBoost;

   

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);
        playerController.Heal(healthBoost);
        playerController.OnHit += Hit;
        
    }

    public void Hit(int health)
    {
        playerController.OnHit -= Hit;
        Destroy(gameObject);
    }
}
