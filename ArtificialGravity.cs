using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravity : MonoBehaviour {
    public float force;
    public bool stun;

    void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D r = other.GetComponent<Rigidbody2D>();
        if(r != null)
        {
            r.AddForce(force * (transform.position - r.transform.position).normalized, ForceMode2D.Force);
            if(stun)
            {
                Stunnable s = r.GetComponent<Stunnable>();
                if(s != null)
                {
                    s.Stun(0.1f);
                }
            }
            
        }
    }
	
}
