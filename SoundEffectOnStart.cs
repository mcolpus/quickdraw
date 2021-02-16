using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectOnStart : MonoBehaviour {
    public MyAudio.AUDIOCLIPS audioType;
    public float playTime;

    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(playTime);
        MyAudio.current.PlayClip(audioType);
    }
	
}
