using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontZone : MonoBehaviour {

    public int team;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(collision.GetComponent<PlayerStat>().Team != team)
            {
                //die
                Debug.Log("STAY ON YOUR SIDE BITCH!");
            }
        }
    }
}
