using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MyAudio : MonoBehaviour {
    private static MyAudio _current;
    public static MyAudio current
    {
        get
        {
            if(_current != null)
                return _current;
            Debug.LogError("no audio");
            return null;
        }
        set
        {
            _current = value;
        }
    }

    void Awake()
    {
        if(_current == null)
        {
            _current = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum AUDIOCLIPS { explosion, bang, lightsaber, lasergun, metalSlash, thud, revving, hiss, none}
    public AUDIOCLIPS reference;
    public List<AudioClip> explosions;
    public List<AudioClip> bangs;
    public List<AudioClip> lightsabers;
    public List<AudioClip> laserguns;
    public List<AudioClip> metalSlashs;
    public List<AudioClip> thuds;
    public List<AudioClip> revvings;
    public List<AudioClip> hisss;
    private List<List<AudioClip>> allClips = new List<List<AudioClip>>();

    public AudioClip backgroundMusic;
    public AudioSource backgroundSource;
    public bool PlayBackground;

    void Start()
    {
        allClips.Add(explosions);
        allClips.Add(bangs);
        allClips.Add(lightsabers);
        allClips.Add(laserguns);
        allClips.Add(metalSlashs);
        allClips.Add(thuds);
        allClips.Add(revvings);
        allClips.Add(hisss);


        backgroundSource.clip = backgroundMusic;
        if(PlayBackground)
            backgroundSource.Play();
    }

    public void PlayClip(AUDIOCLIPS clip)
    {
        if(clip == AUDIOCLIPS.none)
            return;
        List<AudioClip> group = allClips[(int)clip];
        AudioClip c = group.GetRandom();
        AudioSource.PlayClipAtPoint(c, Vector3.zero);
    }

    public AudioClip GetClip(AUDIOCLIPS clip)
    {
        if(clip == AUDIOCLIPS.none)
            return null;
        List<AudioClip> group = allClips[(int)clip];
        return group.GetRandom();
    }

}
