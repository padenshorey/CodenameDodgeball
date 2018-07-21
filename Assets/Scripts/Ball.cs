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
    public SpriteRenderer ballSprite;
    public Animator animator;
    public GameObject trail;

    public TrailRenderer multiTrail;
    public TrailRenderer multiTrailInstance;

    public List<PlayerController> playersHitThisThrow = new List<PlayerController>();
    public bool firstHit = false;

    public float rightCurve = 0f;
    public float leftCurve = 0f;
    private float throwPowerLevel;

    private const float MAX_BALL_AIRTIME = 1.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
        trail.SetActive(false);
    }

    private void Update()
    {
        if(currentBallState == BallState.Thrown)
        {
            //only allow curving before anything has been hit
            if(!firstHit) ApplyCurve();
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

    private void ApplyCurve()
    {
        rightCurve = Input.GetAxis(lastThrownBy.playerController.xboxController.rt);
        leftCurve = Input.GetAxis(lastThrownBy.playerController.xboxController.lt);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        rightCurve = rightCurve.Remap(-1, 1, 0, 1);
        leftCurve = leftCurve.Remap(-1, 1, 0, 1);
#endif
        float curveValue = (rightCurve - leftCurve) * GameManager.instance.gamePreferences.curveInfluence;
        curveValue *= throwPowerLevel;
        curveValue *= -1;

        Vector2 curveVector = new Vector2(-rigidbody2D.velocity.y, rigidbody2D.velocity.x);
        curveVector.Normalize();

        rigidbody2D.AddForce(curveVector * curveValue);
    }

    private void SetBallColor(float progress)
    {
        Color c = new Color(progress, progress, progress);
        ballSprite.color = c;
    }

    public void SetToIdle()
    {
        currentBallState = BallState.Idle;
        ballSprite.color = Color.white;

        animator.SetTrigger("Dead");
        trail.SetActive(false);

        playersHitThisThrow.Clear();

        lastThrownBy = null;
        firstHit = false;

        if(multiTrailInstance) multiTrailInstance.transform.SetParent(null);
        Destroy(multiTrailInstance, 1f);
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
        throwPowerLevel = power / max;
        throwTimeout = throwPowerLevel * MAX_BALL_AIRTIME;

        animator.SetTrigger("Throw");
        trail.SetActive(true);

        rigidbody2D.isKinematic = false;
        rigidbody2D.simulated = true;
        transform.SetParent(null);
        rigidbody2D.velocity = directon * power;
        currentBallState = Ball.BallState.Thrown;

        multiTrailInstance =  Instantiate(multiTrail, transform);
        multiTrailInstance.startColor = by.playerColor;
        multiTrailInstance.endColor = by.playerColor;
    }

    public void Pickup()
    {
        SetToIdle();
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
                }
                else
                {
                    firstHit = true;
                    if(playerHit.ReceiveHit(this))
                    {
                        playersHitThisThrow.Add(playerHit);
                        if(playersHitThisThrow.Count > 1)
                        {
                            Debug.Log("<color=green>MULTIKILL x" + playersHitThisThrow.Count + "</color>");
                        }
                    }
                }
            }
        }
        else if(collision.gameObject.GetComponent<Ball>())
        {
            // hit another ball
            firstHit = true;
        }
        else
        {
            SetToIdle();
        }
    }
}
