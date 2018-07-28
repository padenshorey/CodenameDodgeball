using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    public TextMesh team1_1;
    public TextMesh team1_2;

    public TextMesh team2_1;
    public TextMesh team2_2;

    public int team1Score = 0;
    public int team2Score = 0;

	// Use this for initialization
	void Start () {
        //UpdateScore(0, 0);
	}
	
	public void UpdateScore(int team1, int team2)
    {
        AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Tick);

        team1Score = team1;
        team2Score = team2;

        team1_2.text = ((int)(team1Score % 10)).ToString();
        team1_1.text = Mathf.Floor(team1 / 10).ToString();

        team2_2.text = ((int)(team2Score % 10)).ToString();
        team2_1.text = Mathf.Floor(team2 / 10).ToString();
    }
}
