using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour {

    public enum BallState
    {
        Idle,
        Carry,
        Thrown
    }

    public BallState currentBallState = BallState.Idle;

    private AudioSource audioSource;
    public AudioClip bounce;

    private float timeThrown;
    // TODO: Figure out a better way to determine this time
    private float throwTimeout = 0.5f;

    private Rigidbody2D rigidbody2D;
    private Vector3 initialScale;

    public PlayerStat lastThrownBy;

    private const float MAX_BALL_AIRTIME = 1.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if(currentBallState == BallState.Thrown)
        {
            if(Time.time > (timeThrown + throwTimeout))
            {
                SetToIdle();
            }
            else
            {
                SetBallColor((Time.time - timeThrown) / throwTimeout);
            }
        }

        if(transform.localScale != initialScale && currentBallState != BallState.Carry) transform.localScale = initialScale;
    }

    private void SetBallColor(float progress)
    {
        Color c = new Color(progress, progress, progress);
        GetComponent<SpriteRenderer>().color = c;
    }

    public void SetToIdle()
    {
        currentBallState = BallState.Idle;
        GetComponent<SpriteRenderer>().color = Color.white;
        lastThrownBy = null;
    }

    public void Drop()
    {
        rigidbody2D.isKinematic = false;
        rigidbody2D.simulated = true;
        transform.SetParent(null);
        rigidbody2D.velocity = Vector2.zero;
        currentBallState = Ball.BallState.Idle;
    }

    public void Throw(float power, float max, Vector2 directon, PlayerStat by)
    {
        lastThrownBy = by;

        timeThrown = Time.time;
        throwTimeout = (power / max) * MAX_BALL_AIRTIME;

        rigidbody2D.isKinematic = false;
        rigidbody2D.simulated = true;
        transform.SetParent(null);
        rigidbody2D.velocity = directon * power;
        currentBallState = Ball.BallState.Thrown;
    }

    public void Pickup()
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.isKinematic = true;
        rigidbody2D.simulated = false;
        currentBallState = Ball.BallState.Carry;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.PlayOneShot(bounce);
        PlayerController playerHit = collision.gameObject.GetComponent<PlayerController>();

        if (playerHit != null)
        {
            if (currentBallState == BallState.Thrown)
            {
                // playerHit is hit by the thrown ball (they are out if on other team)
                if (lastThrownBy == playerHit.PLStat)
                {
                    //Debug.Log("Player hit with <color=purple>OWN</color> ball.");
                }
                else
                {
                    Debug.Log("PLAYER HIT WITH THROWN BALL");
                }
            }
        }
        else if(collision.gameObject.GetComponent<Ball>())
        {
            // hit another ball
        }
        else
        {
            SetToIdle();
        }
    }
}
