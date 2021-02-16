using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour {
    public float lifeTime;


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void DestroyNow()
    {
        Destroy(gameObject);
    }
	
}
