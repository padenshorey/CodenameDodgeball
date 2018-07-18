using UnityEngine;

public class PlayerCatchZone : MonoBehaviour {

    public PlayerController pControl;

    private void Start()
    {
        pControl = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Ball ball = col.GetComponent<Ball>();
        if (ball != null)
        {
            if (ball.currentBallState != Ball.BallState.Carry)
            {
                // ball in zone
                pControl.GetComponent<Animator>().SetTrigger("InRange");
                pControl.ballInZone.Insert(0, col.GetComponent<Ball>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Ball>() != null)
        {
            // ball in zone
            pControl.ballInZone.Remove(col.GetComponent<Ball>());
            if(pControl.ballInZone.Count < 1)
            {
                pControl.GetComponent<Animator>().SetTrigger("OutOfRange");
            }
        }
    }
}
