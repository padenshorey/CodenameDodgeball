using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public static Spawner instance = null;
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public PopupCanvas popupCanvas;

    public void SpawnPopupCanvas(Transform parent, Sprite icon, float lifespan)
    {
        PopupCanvas pc = Instantiate(popupCanvas, parent);
        pc.Setup(icon, lifespan);
    }

    public ScreenWipe screenWipe;

    public void SpawnScreenWipe(float lifespan, string message, System.Action onEndAction = null)
    {
        ScreenWipe sw = Instantiate(screenWipe);
        sw.GetComponent<Canvas>().worldCamera = Camera.main;
        if(onEndAction != null)
        {
            sw.Setup(lifespan, message, onEndAction);
        }
        else
        {
            sw.Setup(lifespan, message);
        }
    }

    public CountdownDigit countDigit;

    public void SpawnCountDigit(string textToWrite, float lifespan)
    {
        CountdownDigit cd = Instantiate(countDigit);
        cd.Setup(textToWrite, lifespan);
    }
}
