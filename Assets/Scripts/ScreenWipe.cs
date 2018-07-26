using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenWipe : MonoBehaviour
{

    private float _lifespan;

    private float _spawnTime;

    public Text wipeMessage;

    private Action onScreenWipeComplete;

    private bool wipeComplete = false;

    public void Setup(float life, string message, Action onComplete = null)
    {
        if (onComplete != null) onScreenWipeComplete = onComplete;
        _lifespan = life;
        wipeMessage.text = message;
    }

    private void Awake()
    {
        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (wipeComplete) return;

        if (Time.time > (_spawnTime + _lifespan))
        {
            wipeComplete = true;
            GetComponent<Animator>().SetTrigger("Hide");
            onScreenWipeComplete.Invoke();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

}
