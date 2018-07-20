using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballRound : MonoBehaviour {

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

    public DodgeballRound SetupRound()
    {
        // move players to spawn points and freeze

        // move/spawn balls to spawn points

        // start countdown
        return this;
    }

    public void StartGame()
    {
        // un freeze players
    }

    public void EndRound(int winningTeam)
    {
        _dbGame.EndRound(winningTeam);
    }
}
