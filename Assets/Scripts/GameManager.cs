using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GamePreferences gamePreferences;

    public Animator titleAnimator;

    public List<PlayerStat> team1;
    public List<PlayerStat> team2;
    public Text team1Num;
    public Text team2Num;

    public Ball ballPrefab;

    public PlayerController playerController;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnNewPlayer(-1, true);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // TODO: this is not going to work, need a better ID system
            SpawnNewPlayer(team1.Count + team2.Count, false);
        }        
    }

    private PlayerStat SpawnNewPlayer(int controllerId, bool isBot = false)
    {
        if(team1.Count == 0 && team2.Count == 0)
        {
            titleAnimator.SetTrigger("SlideUp");
            AudioManager.instance.PlaySFX(AudioManager.AudioSFX.SwipeOut);

            for(int i = 0; i < 5; i++)
            {
                SpawnBall();
            }
        }

        PlayerController pc = Instantiate(playerController, GetSpawnPosition(), Quaternion.identity);
        Vector2 target = new Vector2(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
        Vector2 v = (target - new Vector2(pc.transform.position.x, pc.transform.position.y)).normalized;
        pc.GetComponent<Rigidbody2D>().velocity = v * Random.Range(150f, 200f);
        AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Slam);

        PlayerStat pStat = pc.GetComponent<PlayerStat>();
        if (pStat == null) Debug.LogError("GameManager SpawnNewPlayer: Player does not have a PlayerStats");

        pc.isRealPlayer = !isBot;

        return pStat;
    }

    private void SpawnBall()
    {
        Ball ball = Instantiate(ballPrefab, GetSpawnPosition(), Quaternion.identity);
        Vector2 target = new Vector2(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
        Vector2 v = (target - new Vector2(ball.transform.position.x, ball.transform.position.y)).normalized;
        ball.GetComponent<Rigidbody2D>().velocity = v * Random.Range(25f, 35f);
    }

    private Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
        float circleRadius = 11f;
        return new Vector2(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);
    }

    public void AssignTeam(PlayerStat pStat, int team)
    {
        RemovePlayerFromAllTeams(pStat);

        if(team == 1)
        {
            team1.Add(pStat);
        }
        else if(team == 2)
        {
            team2.Add(pStat);
        }

        team1Num.text = team1.Count.ToString();
        team2Num.text = team2.Count.ToString();

        pStat.Team = team;
        pStat.SetPlayerColor(team == 1 ? gamePreferences.team1Colors[Random.Range(0, gamePreferences.team1Colors.Count)] : gamePreferences.team2Colors[Random.Range(0, gamePreferences.team2Colors.Count)]);
    }

    private void RemovePlayerFromAllTeams(PlayerStat pStat)
    {
        int indexOfPlayerToRemove = -1;

        int currentIndex = 0;
        foreach(PlayerStat p in team1)
        {
            if (p == pStat) indexOfPlayerToRemove = currentIndex;
            currentIndex++;
        }
        if (indexOfPlayerToRemove > -1)
        {
            team1.Remove(pStat);
            return;
        }

        currentIndex = 0;
        foreach (PlayerStat p in team2)
        {
            if (p == pStat) indexOfPlayerToRemove = currentIndex;
            currentIndex++;
        }
        if (indexOfPlayerToRemove > -1)
        {
            team2.Remove(pStat);
            return;
        }
    }
}
