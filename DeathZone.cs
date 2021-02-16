using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if(dmg != null)
        {
            dmg.Kill();
        }
    }
}
