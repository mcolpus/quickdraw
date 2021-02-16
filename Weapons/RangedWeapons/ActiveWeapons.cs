using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapons : Weapon {
    private float timeAtPickup;//to make a small delay

    public override void Fire()
    {
        if(Time.time - timeAtPickup > 0.2f)
        {
            playerController.ForceDropWeapon();
            Throw(playerController.throwSpeed);
        }
            
    }

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);
        timeAtPickup = Time.time;
    }
}
