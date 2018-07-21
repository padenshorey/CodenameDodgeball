using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public PlayerController playerController;

    public void Setup()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPlayerColor(Color color)
    {
        characterBorder.color = color;
        GetComponent<Animator>().SetTrigger("Throb");
        audioSource.PlayOneShot(boing);
        playerController = GetComponent<PlayerController>();
        playerController.throwMeterBG.GetComponent<Outline>().effectColor = ChangeColorBrightness(color, 0.8f);
        playerController.throwMeterFill.color = color;
        foreach(QuickThrowIcon q in playerController.quickThrowIcons)
        {
            q.fill.color = color;
        }
    }

    public static Color ChangeColorBrightness(Color color, float correctionFactor)
    {
        float red = (float)color.r;
        float green = (float)color.g;
        float blue = (float)color.b;

        red *= correctionFactor;
        green *= correctionFactor;
        blue *= correctionFactor;


        return new Color(red, green, blue);
    }


}
