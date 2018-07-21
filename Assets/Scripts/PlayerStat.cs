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

    public SpriteRenderer rightHand;
    public SpriteRenderer leftHand;
    public SpriteRenderer playerBodyTop;
    public SpriteRenderer playerBodyBottom;

    public AudioSource audioSource;
    public AudioClip boing;
    public AudioClip swish;

    public PlayerController playerController;

    public List<Sprite> emojis = new List<Sprite>();

    public float timeOfLastEmoji;

    public void Setup()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPlayerColor(Color color)
    {
        playerColor = color;
        characterBorder.color = color;
        //rightHand.color = color;
        //leftHand.color = color;
        //playerBodyBottom.color = color;
        playerBodyTop.color = color;
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

    public void DoEmoji(int emojiId)
    {
        if (emojiId > emojis.Count || Time.time < (timeOfLastEmoji + GameManager.instance.gamePreferences.emojiCooldown)) return;
        Spawner.instance.SpawnPopupCanvas(transform, emojis[emojiId], 1f);
        timeOfLastEmoji = Time.time;
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
