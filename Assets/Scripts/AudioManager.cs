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
        Error
    }

    public AudioSource audioSource;

    public AudioClip boing;
    public AudioClip slam;
    public AudioClip swipeIn;
    public AudioClip swipeOut;
    public AudioClip pop;
    public AudioClip error;

    void Start () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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
        }

        audioSource.PlayOneShot(clipToPlay);
    }

}
