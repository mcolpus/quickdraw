using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HairDryer : Weapon
{
    public GameObject pushArea;
    

    public MyAudio.AUDIOCLIPS audioClip;
    private AudioClip clip;
    private AudioSource audioSource;

    private bool on;
    private float turnOnTime;

    public override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }
    

    void Update()
    {
        if(equiped)
        {
            pushArea.SetActive(on);
            if(Time.time > turnOnTime + 0.1f)
                on = false;
        }
     }
    


    public override void Fire()
    {
        on = true;
        turnOnTime = Time.time;
        if(!audioSource.isPlaying)
        {
            clip = MyAudio.current.GetClip(audioClip);
            audioSource.clip = clip;
            audioSource.Play();
        }
        if(isServer)
            RpcTurnOn();
    }
    [ClientRpc]
    void RpcTurnOn()
    {
        on = true;
        turnOnTime = Time.time;
        if(!audioSource.isPlaying)
        {
            clip = MyAudio.current.GetClip(audioClip);
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public override void Drop()
    {
        base.Drop();

        pushArea.SetActive(false);
    }

}
