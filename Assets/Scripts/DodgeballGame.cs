using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballGame : MonoBehaviour
{
    public enum GameType
    {
        Pratice = 0,
        OneOnOne = 1,
        TwoOnTwo = 2,
        ThreeOnThree = 3,
        FourOnFour = 4
    }

    private int _roundTotal;
    private int _roundsToWin;
    private GameType _gameType;
    public GameType CurrentGameType { get { return _gameType; }}
    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } set { _currentRound = value; } }

    private int team1Score;
    private int team2Score;

    private List<PlayerStat> team1;
    private List<PlayerStat> team2;

    public Ball ballPrefab;
    private List<Ball> balls = new List<Ball>();

    private List<DodgeballRound> rounds = new List<DodgeballRound>();

    private int secondCounter = 1;

    public void Setup(int rounds, GameType gType, List<PlayerStat> t1, List<PlayerStat> t2)
    {
        _roundTotal = rounds;
        _roundsToWin = (int)(Mathf.Floor((float)rounds / 2f) + 1f);
        _gameType = gType;

        team1 = t1;
        team2 = t2;

        if (_gameType != GameType.Pratice)
        {
            int[] missingPlayers = MissingPlayers(_gameType);

            if (missingPlayers[0] > 0 || missingPlayers[1] > 0)
            {
                for(int i = 0; i < missingPlayers.Length; i++)
                {
                    for (int j = 0; j < missingPlayers[i]; j++)
                    {
                        //TODO: add player bot to team i
                        Debug.Log("<color=purple>Adding Bot to Team</color>");
                    }
                }
            }
            else if (missingPlayers[0] < 0 || missingPlayers[1] < 0)
            {
                Debug.LogError("TOO MANY PLAYERS FOR GAMETYPE: " + _gameType.ToString());
            }
        }

        _currentRound = 0;
        team1Score = team2Score = 0;

        StartGame();
    }

    private int[] MissingPlayers(GameType gType)
    {
        int[] missingPlayers = new int[2];
        missingPlayers[0] = (int)gType - team1.Count;
        missingPlayers[1] = (int)gType - team2.Count;
        return missingPlayers;
    }

    public void StartGame()
    {
        if (_roundTotal < 1) Debug.LogError("DodgeballGame StartGame: Trying to start a game with 0 rounds.");

        for(int i = 0; i < _roundTotal; i++)
        {
            rounds.Add(new DodgeballRound(this, i, team1, team2));
        }

        SetupNextRound();
    }

    private void SetupNextRound()
    {
        GameManager.instance.currentRound = rounds[_currentRound].SetupRound();
        StartTimer();
    }

    private void StartRound()
    {
        secondCounter = 1;
        countingDown = false;
        rounds[_currentRound].StartRound();
        AudioManager.instance.PlaySFX(AudioManager.AudioSFX.GongBig);

    }

    public float roundStartTime;
    private bool countingDown = false;


    private void StartTimer()
    {
        roundStartTime = Time.time;
        countingDown = true;
    }

    private void Update()
    {
        if(countingDown)
        {
            //Debug.Log("<color=green>Starting Game in: " + ((roundStartTime + GameManager.instance.gamePreferences.countdownDuration) - Time.time).ToString() + "</color>");
            if (Time.time > (roundStartTime + secondCounter))
            {
                int secondsLeft = (int)(GameManager.instance.gamePreferences.countdownDuration - secondCounter);

                secondCounter++;
                AudioManager.instance.PlaySFX(AudioManager.AudioSFX.GongSmall);
                Spawner.instance.SpawnCountDigit(secondsLeft == 0 ? "GO!" : secondsLeft.ToString(), 1.5f);
            }

            if (Time.time > (roundStartTime + GameManager.instance.gamePreferences.countdownDuration))
            {
                StartRound();
            }
        }
    }

    public void EndRound(int winningTeam)
    {
        if(winningTeam == 0)
        {
            team1Score++;
        }
        else if(winningTeam == 1)
        {
            team2Score++;
        }

        if(team1Score == _roundsToWin || team2Score == _roundsToWin)
        {
            if(team1Score == _roundsToWin)
            {
                // TEAM 1 WINS
                EndGame(0);
            }
            else
            {
                // TEAM 2 WINS
                EndGame(1);
            }
        }

        _currentRound++;
        SetupNextRound();
    }

    public void MovePlayersToSpawn()
    {
        int counter = 0;
        foreach(PlayerStat player in team1)
        {
            Vector2 playerSpawn = GetPlayerSpawn(GameManager.instance.team1SpawnStart, GameManager.instance.team1SpawnEnd, team1.Count, counter);
            Vector3 playerSpawnV3 = new Vector3(playerSpawn.x, playerSpawn.y, player.transform.position.z);
            player.playerController.SetPlayerPosition(playerSpawnV3);
            player.playerController.SetPlayerState(PlayerController.PlayerState.SettingUp);
            counter++;
        }
        counter = 0;
        foreach (PlayerStat player in team2)
        {
            Vector2 playerSpawn = GetPlayerSpawn(GameManager.instance.team2SpawnStart, GameManager.instance.team2SpawnEnd, team2.Count, counter);
            Vector3 playerSpawnV3 = new Vector3(playerSpawn.x, playerSpawn.y, player.transform.position.z);
            player.playerController.SetPlayerPosition(playerSpawnV3);
            player.playerController.SetPlayerState(PlayerController.PlayerState.SettingUp);
            counter++;
        }
    }

    public void UnlockPlayers()
    {
        foreach (PlayerStat player in team1)
        {
            player.playerController.SetPlayerState(PlayerController.PlayerState.Idle);
        }
        foreach (PlayerStat player in team2)
        {
            player.playerController.SetPlayerState(PlayerController.PlayerState.Idle);
        }

    }

    public void SpawnBalls(int ballsToSpawn)
    {   for (int i = 0; i < ballsToSpawn; i++)
        {
            Vector2 ballSpawn = GetBallSpawn(GameManager.instance.ballSpawnStart, GameManager.instance.ballSpawnEnd, ballsToSpawn, i);
            Vector3 ballSpawnV3 = new Vector3(ballSpawn.x, ballSpawn.y, GameManager.instance.ballPrefab.transform.position.z);
            Ball ball = Instantiate(GameManager.instance.ballPrefab, ballSpawnV3, Quaternion.identity);
            balls.Add(ball);
        }
    }

    private Vector2 GetBallSpawn(Transform ballStart, Transform ballEnd, int ballCount, int currentBall)
    {
        float yPosition = (ballStart.position.y + ballEnd.position.y) / 2f;
        float spawnRangeMagnitude = ballStart.position.y - ballEnd.position.y;

        float numberINeed = spawnRangeMagnitude / (ballCount + 1);

        float foof = numberINeed * (currentBall + 1);
        foof += ballEnd.position.y;

        return new Vector2(ballStart.position.x, foof);
    }

    private Vector2 GetPlayerSpawn(Transform teamStart, Transform teamEnd, int teamCount, int currentPlayer)
    {
        //float yPosition = (teamStart.position.y + teamEnd.position.y) / 2f;
        float spawnRangeMagnitude = Vector2.Distance(teamStart.position, teamEnd.position);

        //Debug.Log("Distance between start and end: " + spawnRangeMagnitude.ToString());

        float numberINeed = (spawnRangeMagnitude / (teamCount + 1));
        //Debug.Log("Padding: " + numberINeed.ToString());
        numberINeed *= (currentPlayer + 1);
        //Debug.Log("Current Player (" + currentPlayer + ")'s Position Y: " + numberINeed.ToString());
        numberINeed += teamEnd.position.y;
        //Debug.Log("Adjusted Player (" + currentPlayer + ")'s Position Y: " + numberINeed.ToString());

        return new Vector2(teamStart.position.x, numberINeed);
    }

    public void RemoveAllBalls()
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();

        foreach (Ball b in allBalls)
        {
            Destroy(b.gameObject);
        }

        balls.Clear();
    }

    private void EndGame(int winningTeam)
    {
        Debug.Log("Team " + (winningTeam+1).ToString() + " Wins!");
        GameManager.instance.EndGame(this);
    }
}
