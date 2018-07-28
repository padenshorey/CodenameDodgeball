using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreferences :MonoBehaviour
{
    public List<Color> team1Colors = new List<Color>();
    public List<Color> team2Colors = new List<Color>();

    public float lobbyRespawnTime;
    public float gameRespawnTime = 1f;

    public float respawnInvulnerabilityPeriod = 1f;

    public float timeForQuickThrowRecharge = 10f;

    public float throwSpeed = 40f;
    public float quickThrowSpeed = 0.8f;
    public float FULL_POWER_TIME = 2f;
    public float minThrowPower;

    public float curveInfluence = 1000f;
    public float emojiCooldown = 1f;

    public int minBallCount = 3;

    public float countdownDuration = 5f;

    private void Awake()
    {
        minThrowPower = throwSpeed / 5f;
    }
}
