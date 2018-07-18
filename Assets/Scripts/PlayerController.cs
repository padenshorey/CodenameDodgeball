using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Carrying,
        Dashing,
        Throwing
    }

    // Normal Movements Variables
    private float walkSpeed;
    private float curSpeed;
    private const float maxSpeed = 50;
    private const float lerpSpeed = 0.2f;
    private float sprintSpeed;

    private bool sprinting = false;

    private PlayerStat plStat;
    private Rigidbody2D rigidbody2D;

    public Transform characterPlayer;

    private float lerpedX = 0;
    private float lerpedY = 0;

    // states
    private PlayerState currentPlayerState;
    private PlayerState previousPlayerState = PlayerState.Idle;

    // dash
    private bool isDashing;
    private float dashCooldown;
    private float dashMagnitude = 1000f;

    // ball
    private Ball ballBeingHeld;

    // hands
    public Transform leftHand;
    public Transform rightHand;

    // throw UI
    public Image throwMeterBG;
    public Image throwMeterFill;
    private float throwPower;
    private float throwSpeed = 50f;
    private float throwSpeedModifier = 1f;
    private const float FULL_POWER_TIME = 1.5f;
    private float throwStartTime;

    public List<QuickThrowIcon> quickThrowIcons = new List<QuickThrowIcon>();

    void Start()
    {
        plStat = GetComponent<PlayerStat>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        walkSpeed = (float)(plStat.Speed + (plStat.Agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

        SetPlayerState(PlayerState.Idle);

        throwMeterBG.enabled = false;
        throwMeterFill.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }

        // build up throw
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartThrow();
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            ThrowBall(throwPower * throwSpeed);
        }

        // quick throw
        if(Input.GetKeyDown(KeyCode.K))
        {
            QuickThrow();
        }

        if(currentPlayerState == PlayerState.Throwing)
        {
            PowerUpThrow();
        }

        ChargeQuickThrow();
    }

    public void ChargeQuickThrow()
    {
        foreach(QuickThrowIcon q in quickThrowIcons)
        {
            if (q.state == QuickThrowIcon.QuickThrowIconState.Charging)
            {
                return;
            }
            else if (q.state == QuickThrowIcon.QuickThrowIconState.Empty)
            {
                q.StartCharge();
                return;
            }
        }
    }

    public void QuickThrow()
    {
        bool ballThrown = false;

        for(int i = 0; i < quickThrowIcons.Count; i++)
        {
            if (quickThrowIcons[i].state == QuickThrowIcon.QuickThrowIconState.Ready)
            {
                if (!ballThrown && ThrowBall(1f * throwSpeed))
                {
                    AudioManager.instance.PlaySFX(AudioManager.AudioSFX.SwipeIn);
                    ballThrown = true;
                    quickThrowIcons[i].Reset();
                }                
            }

            if (ballThrown)
            {
                if (i < (quickThrowIcons.Count - 1))
                    quickThrowIcons[i].SetIconState(quickThrowIcons[i + 1]);
                else
                    quickThrowIcons[i].Reset();
            }
        }
    }

    public void SetPlayerState(PlayerState ps)
    {
        previousPlayerState = currentPlayerState;
        currentPlayerState = ps;
    }

    private void Dash()
    {
        rigidbody2D.AddForce(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * dashMagnitude);
        plStat.audioSource.PlayOneShot(plStat.swish);
    }

    public void PickupBall(Ball ball)
    {
        if (ballBeingHeld == null && ball.currentBallState == Ball.BallState.Idle)
        {
            ball.Pickup();
            ballBeingHeld = ball;
            ball.transform.SetParent(characterPlayer);
            ball.transform.position = Random.Range(0f, 1f) > 0.5f ? rightHand.position : leftHand.position;
        }
        else
        {
            //cant pick up ball, already have one
        }
    }

    public void StartThrow()
    {
        if (ballBeingHeld == null) return;

        throwSpeedModifier = 0.5f;
        throwMeterBG.enabled = true;
        throwMeterFill.enabled = true;
        throwStartTime = Time.time;
        SetPlayerState(PlayerState.Throwing);
    }

    public void PowerUpThrow()
    {
        throwPower = Mathf.Min(((Time.time - throwStartTime) / FULL_POWER_TIME), 1f);
        throwMeterFill.fillAmount = throwPower / 2f;
    }

    public bool ThrowBall(float power)
    {
        if (ballBeingHeld != null)
        {
            ballBeingHeld.Throw(power);
            throwSpeedModifier = 1f;
            ballBeingHeld = null;
            throwPower = 0f;
            throwMeterBG.enabled = false;
            throwMeterFill.enabled = false;
            SetPlayerState(PlayerState.Idle);
            return true;
        }
        else
        {
            return false;
        }
    }

    void FixedUpdate()
    {
        curSpeed = sprinting ? sprintSpeed : walkSpeed;

        if (curSpeed > maxSpeed) curSpeed = maxSpeed;

        Vector2 analogAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                       
        if(analogAxis.magnitude > 1f)
        {
            analogAxis = analogAxis.normalized;
        }

        Vector2 movementLerp = Vector2.Lerp(new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y), analogAxis * curSpeed * throwSpeedModifier, lerpSpeed);

        rigidbody2D.velocity = movementLerp;

        Vector2 staticVector = new Vector2(0f, 1f);

        float rotationAngleRelative = Mathf.Atan2(analogAxis.y, -analogAxis.x) -  Mathf.Atan2(staticVector.y, staticVector.x);
        rotationAngleRelative *= Mathf.Rad2Deg;
        rotationAngleRelative *= -1f;

        characterPlayer.eulerAngles = new Vector3(0, 0, rotationAngleRelative);
    }


}
