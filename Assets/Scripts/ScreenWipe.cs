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
        if (Time.time > (_spawnTime + _lifespan))
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
    }

    public void Die()
    {
        onScreenWipeComplete.Invoke();
        Destroy(this.gameObject);
    }

}
