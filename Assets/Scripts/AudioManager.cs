using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    public enum AudioSFX
    {
        Slam,
        Boing,
        SwipeIn,
        SwipeOut,
        Pop,
        Error,
        Explosion,
        Gunshot,
        GongSmall,
        GongBig,
        Tick
    }

    public AudioSource audioSource;

    public AudioClip boing;
    public AudioClip slam;
    public AudioClip swipeIn;
    public AudioClip swipeOut;
    public AudioClip pop;
    public AudioClip error;
    public AudioClip explosion;
    public AudioClip gunshot;
    public AudioClip tick;

    public AudioClip[] shoeSqueaks;

    public AudioClip gongSmall;
    public AudioClip gongBig;

    void Start () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
	
    public AudioClip GetSqueak()
    {
        return shoeSqueaks[(int)Mathf.Floor(Random.Range(0, shoeSqueaks.Length))];
    }

    public void PlaySFX(AudioSFX sfx)
    {
        AudioClip clipToPlay = null;
        switch(sfx)
        {
            case AudioSFX.Boing:
                clipToPlay = boing;
                break;
            case AudioSFX.Slam:
                clipToPlay = slam;
                break;
            case AudioSFX.SwipeIn:
                clipToPlay = swipeIn;
                break;
            case AudioSFX.SwipeOut:
                clipToPlay = swipeOut;
                break;
            case AudioSFX.Pop:
                clipToPlay = pop;
                break;
            case AudioSFX.Error:
                clipToPlay = error;
                break;
            case AudioSFX.Explosion:
                clipToPlay = explosion;
                break;
            case AudioSFX.Gunshot:
                clipToPlay = gunshot;
                break;
            case AudioSFX.GongSmall:
                clipToPlay = gongSmall;
                break;
            case AudioSFX.GongBig:
                clipToPlay = gongBig;
                break;
            case AudioSFX.Tick:
                clipToPlay = tick;
                break;
        }

        audioSource.PlayOneShot(clipToPlay);
    }

}
