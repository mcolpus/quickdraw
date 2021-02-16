using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushArea : MonoBehaviour {
    public float pushForce;
    public float range;//for linear fall off
    public bool stun;

    public void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D r = other.GetComponentInParent<Rigidbody2D>();
        if(r!=null && !r.isKinematic)
        {
            float dis = Vector2.Distance(transform.position, r.transform.position);
            if(dis > range)
                return;

            r.AddForce(transform.right * pushForce * (1f - (dis/range)), ForceMode2D.Force);

            if(dis < range / 2f)
            {
                Stunnable stunnable = r.GetComponent<Stunnable>();
                if(stunnable != null)
                {
                    stunnable.Stun(0.3f);
                }
            }
            
        }
    }
	
}
