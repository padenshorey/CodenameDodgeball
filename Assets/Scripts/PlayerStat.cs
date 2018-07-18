using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public int controllerId;

    public float Speed;
    public float Agility;

    public int Team;

    public Color playerColor;

    public SpriteRenderer characterBorder;

    public AudioSource audioSource;
    public AudioClip boing;
    public AudioClip swish;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPlayerColor(Color color)
    {
        characterBorder.color = color;
        GetComponent<Animator>().SetTrigger("Throb");
        audioSource.PlayOneShot(boing);
    }

}
