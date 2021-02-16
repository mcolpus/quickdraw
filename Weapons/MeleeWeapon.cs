using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : Weapon
{

    public Transform holdTransform;
    public DamageArea damageArea;
    public float coolDown;
    public float swingTime;
    public float swingSpeed;//how quickly to lerp towards attack rot
    public float swingReturnSpeed;//how quickly to lerp towards attack rot
    public Vector3 defaultRotation;
    public Vector3 swingRotation;

    public bool damageOnThrow;
    public Vector3 throwRotation;
    protected bool thrown;

    public MyAudio.AUDIOCLIPS audioClipOnSwing;
    

    protected float LastSwingFired;//keeps track of the time at which they most recently attacked
    
    

    public override void Fire()
    {
        if(Time.time - LastSwingFired > coolDown)
        {
            DoFire();
        }
    }
    protected override void DoFire()
    {
        base.DoFire();
        StartCoroutine(Swing());
        if(isServer)
            RpcRunSwing();
    }
    
    [ClientRpc]
    protected void RpcRunSwing()
    {
        StartCoroutine(Swing());
    }

    protected virtual IEnumerator Swing()
    {
        MyAudio.current.PlayClip(audioClipOnSwing);

        LastSwingFired = Time.time + 100f;
        
        damageArea.SetActive(true);
        float startTime = Time.time;

        Quaternion targetRot = Quaternion.Euler(swingRotation);
        while(Time.time - startTime < swingTime / 2f)
        {
            holdTransform.localRotation = Quaternion.Lerp(holdTransform.localRotation, targetRot, swingSpeed * Time.deltaTime);
            yield return null;
        }
        startTime = Time.time;

        targetRot = Quaternion.Euler(defaultRotation);
        while(Time.time - startTime < swingTime / 2f)
        {
            holdTransform.localRotation = Quaternion.Lerp(holdTransform.localRotation, targetRot, swingReturnSpeed * Time.deltaTime);
            yield return null;
        }
        damageArea.SetActive(false);
        holdTransform.localRotation = Quaternion.Euler(defaultRotation);

        LastSwingFired = Time.time;
    }

    public override void Pickup(GameObject ObjectPlayerController, Vector3 localPosition)
    {
        base.Pickup(ObjectPlayerController, localPosition);

        holdTransform.localRotation = Quaternion.identity;
    }

    

    public override void Throw(float speed)
    {
        holdTransform.localRotation = Quaternion.Euler(throwRotation);

        base.Throw(speed);
        
        if(damageOnThrow)
        {
            damageArea.SetActive(true);
            thrown = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(thrown)
        {
            damageArea.SetActive(false);
        }
    }
}
