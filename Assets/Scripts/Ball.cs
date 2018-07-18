using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private const float throwTimeout = 0.5f;

    private Rigidbody2D rigidbody2D;
    private Vector3 initialScale;

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
                currentBallState = BallState.Idle;
            }
        }

        if(transform.localScale != initialScale && currentBallState != BallState.Carry) transform.localScale = initialScale;
    }

    public void Throw(float power)
    {
        timeThrown = Time.time;

        rigidbody2D.isKinematic = false;
        rigidbody2D.simulated = true;
        transform.SetParent(null);
        rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * power;
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
            if(currentBallState == BallState.Idle)
            {
                playerHit.PickupBall(this);
            }
            else if(currentBallState == BallState.Thrown)
            {

            }
        }
    }
}
