using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCanvas : MonoBehaviour {

    public Image emoji;
    private Sprite _icon;
    private float _lifespan;

    private float _spawnTime;

    public void Setup(Sprite icon, float life)
    {
        _icon = icon;
        _lifespan = life;

        if(_icon != null)
            emoji.sprite = _icon;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Random.Range(-10, 10));
        float randomValue = Random.Range(0.95f, 1.05f);
        transform.localScale = new Vector3(transform.localScale.x * randomValue, transform.localScale.y * randomValue, transform.localScale.z);
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
        Destroy(this.gameObject);
    }
}
