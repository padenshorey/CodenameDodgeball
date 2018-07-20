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
    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } set { _currentRound = value; } }

    private int team1Score;
    private int team2Score;

    private List<PlayerStat> team1;
    private List<PlayerStat> team2;

    private List<DodgeballRound> rounds = new List<DodgeballRound>();

    public DodgeballGame(int rounds, GameType gType, List<PlayerStat> t1, List<PlayerStat> t2)
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
                    for (int j = 0; j < missingPlayers[i]; i++)
                    {
                        //TODO: add player bot to team i
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

        StartNextRound();
    }

    private void StartNextRound()
    {
        GameManager.instance.currentRound = rounds[_currentRound].SetupRound();
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
        StartNextRound();
    }

    private void EndGame(int winningTeam)
    {
        Debug.Log("Team " + (winningTeam+1).ToString() + " Wins!");
        GameManager.instance.EndGame(this);
    }
}
