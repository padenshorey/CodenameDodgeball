using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamZone : MonoBehaviour 
{
    public int team;
    public void OnTriggerEnter2D(Collider2D other)
    {
        FindObjectOfType<GameManager>().AssignTeam(other.GetComponent<PlayerStat>(), team);
        FindObjectOfType<CamShakeSimple>().ShakeCamera(10f);
    }
}
