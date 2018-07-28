using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballRound {

    private DodgeballGame _dbGame;
    private int _roundnumber;
    private int _winningTeam;
    private List<PlayerStat> team1;
    private List<PlayerStat> team2;

    public DodgeballRound(DodgeballGame dg, int rNum, List<PlayerStat> t1, List<PlayerStat> t2)
    {
        _dbGame = dg;
        _roundnumber = rNum;
        team1 = t1;
        team2 = t2;
    }

    private void SpawnBalls()
    {
        int ballsToSpawn = Mathf.Max((int)_dbGame.CurrentGameType - 1, GameManager.instance.gamePreferences.minBallCount);
        _dbGame.RemoveAllBalls();
        _dbGame.SpawnBalls(ballsToSpawn);
    }

    public DodgeballRound SetupRound()
    {
        // move players to spawn points and freeze
        _dbGame.MovePlayersToSpawn();

        // move/spawn balls to spawn points
        SpawnBalls();

        // start countdown
        return this;
    }

    public void StartRound()
    {
        // un freeze players
        _dbGame.UnlockPlayers();
    }

    public void EndRound(int winningTeam)
    {
        
    }
}
