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
        Throwing,
        PickingUp,
        Dead,
        Respawning,
        SettingUp
    }

    public XboxController xboxController;

    // human control
    public bool isRealPlayer = true;

    // Normal Movements Variables
    private float walkSpeed;
    private float curSpeed;
    private const float maxSpeed = 50;
    private const float lerpSpeed = 0.2f;
    private float sprintSpeed;

    private bool sprinting = false;

    private PlayerStat plStat;
    public PlayerStat PLStat { get { return plStat; } }
    private Rigidbody2D rigidbody2D;

    private Vector2 normalizedDirection;

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
    public List<Ball> ballInZone = new List<Ball>();

    // hands
    public Transform leftHand;
    public Transform rightHand;

    // throw UI
    public Image throwMeterBG;
    public Image throwMeterFill;
    private float throwPower;
    public float throwSpeedModifier = 1f;
    private float throwStartTime;
    public List<QuickThrowIcon> quickThrowIcons = new List<QuickThrowIcon>();

    // controls
    private KeyCode quickThrow = KeyCode.P;
    private KeyCode startThrow = KeyCode.Space;
    private KeyCode dropBall = KeyCode.J;
    private KeyCode pickupBall = KeyCode.Space;
    private KeyCode dash = KeyCode.O;

    // death
    private float timeOfDeath;
    private float timeOfRespawn;
    public GameObject explosion;

    void Start()
    {
        plStat = GetComponent<PlayerStat>();
        plStat.Setup();

        rigidbody2D = GetComponent<Rigidbody2D>();

        SetupControls();

        walkSpeed = (float)(plStat.Speed + (plStat.Agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

        SetPlayerState(PlayerState.Idle);

        throwMeterBG.enabled = false;
        throwMeterFill.enabled = false;
    }

    private void SetupControls()
    {
        xboxController = new XboxController(plStat.controllerId);
    }

    void FixedUpdate()
    {
        if (currentPlayerState == PlayerState.Dead || currentPlayerState == PlayerState.SettingUp)
        {
            return;
        }

        if (isRealPlayer && xboxController != null)
        {
            curSpeed = sprinting ? sprintSpeed : walkSpeed;

            if (curSpeed > maxSpeed) curSpeed = maxSpeed;

            Vector2 analogAxis;
            if (GameManager.instance.KeyboardEnabled)
            {
                float axisX = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
                float axisY = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
                analogAxis = new Vector2(axisX, axisY);
            }
            else
            {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                analogAxis = new Vector2(Input.GetAxis(xboxController.joyLeftHori), -Input.GetAxis(xboxController.joyLeftVert));
#else
                analogAxis = new Vector2(Input.GetAxis(xboxController.joyLeftHori), Input.GetAxis(xboxController.joyLeftVert));
#endif
            }

            //this makes sure that the character is always facing a direction
            normalizedDirection = analogAxis.normalized == Vector2.zero ? normalizedDirection : analogAxis.normalized;

            if (analogAxis.magnitude > 1f)
            {
                analogAxis = analogAxis.normalized;
            }

            Vector2 movementLerp = Vector2.Lerp(new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y), analogAxis * curSpeed * throwSpeedModifier, lerpSpeed);

            rigidbody2D.velocity = movementLerp;

            Vector2 staticVector = new Vector2(0f, 1f);

            float rotationAngleRelative = Mathf.Atan2(normalizedDirection.y, -normalizedDirection.x) - Mathf.Atan2(staticVector.y, staticVector.x);
            rotationAngleRelative *= Mathf.Rad2Deg;
            rotationAngleRelative *= -1f;

            characterPlayer.eulerAngles = new Vector3(0, 0, rotationAngleRelative);
        }
    }

    private void Update()
    {
        if(currentPlayerState == PlayerState.SettingUp)
        {
            return;
        }

        if (currentPlayerState == PlayerState.Dead)
        {
            if (Time.time > (timeOfDeath + GameManager.instance.gamePreferences.lobbyRespawnTime))
            {
                RespawnPlayer();
            }
            return;
        }

        if (currentPlayerState == PlayerState.Respawning)
        {
            if (Time.time > (timeOfRespawn + GameManager.instance.gamePreferences.respawnInvulnerabilityPeriod))
            {
                SetPlayerState(PlayerState.Idle);
            }
        }

        if (isRealPlayer && xboxController != null)
        {
            if (GameManager.instance.KeyboardEnabled)
            {
                CheckKeyboardInput();
            }
            else
            {
                CheckXboxInput();
            }
        }
    }

    public void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(dash))
        {
            Dash();
        }

        if (Input.GetKeyDown(dropBall))
        {
            DropBall();
        }

        if (Input.GetKeyDown(pickupBall))
        {
            PickupClosestBall();
        }

        if (currentPlayerState != PlayerState.PickingUp)
        {
            // build up throw
            if (Input.GetKeyDown(startThrow))
            {
                StartThrow();
            }
            else if (Input.GetKeyUp(startThrow))
            {
                ThrowBall(throwPower * GameManager.instance.gamePreferences.throwSpeed + GameManager.instance.gamePreferences.minThrowPower);
            }
        }
        else
        {
            if (Input.GetKeyUp(pickupBall))
            {
                currentPlayerState = PlayerState.Carrying;
            }
        }

        // quick throw
        if (Input.GetKeyDown(quickThrow))
        {
            QuickThrow();
        }

        if (currentPlayerState == PlayerState.Throwing)
        {
            PowerUpThrow();
        }

        ChargeQuickThrow();
    }

    public void CheckXboxInput()
    {
        if (Input.GetButtonDown(xboxController.lb))
        {
            Dash();
        }

        if (Input.GetButtonDown(xboxController.b))
        {
            DropBall();
        }

        if (Input.GetButtonDown(xboxController.a))
        {
            PickupClosestBall();
        }

        if (currentPlayerState != PlayerState.PickingUp)
        {
            // build up throw
            if (Input.GetButtonDown(xboxController.a))
            {
                StartThrow();
            }
            else if (Input.GetButtonUp(xboxController.a))
            {
                ThrowBall(throwPower * GameManager.instance.gamePreferences.throwSpeed + GameManager.instance.gamePreferences.minThrowPower);
            }
        }
        else
        {
            if (Input.GetButtonUp(xboxController.a))
            {
                currentPlayerState = PlayerState.Carrying;
            }
        }
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (Input.GetButtonDown(xboxController.dpadUp))
        {
            plStat.DoEmoji(0);
        }
        else if (Input.GetButtonDown(xboxController.dpadDown))
        {
            plStat.DoEmoji(1);
        }
        else if (Input.GetButtonDown(xboxController.dpadLeft))
        {
            plStat.DoEmoji(2);
        }
        else if (Input.GetButtonDown(xboxController.dpadRight))
        {
            plStat.DoEmoji(3);
        }
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (Input.GetAxis(xboxController.dpadVert) > 0.98f)
        {
            plStat.DoEmoji(0);
        }
        else if (Input.GetAxis(xboxController.dpadVert) < -0.98f)
        {
            plStat.DoEmoji(1);
        }
        else if (Input.GetAxis(xboxController.dpadHori) > 0.98f)
        {
            plStat.DoEmoji(2);
        }
        else if (Input.GetAxis(xboxController.dpadHori) < -0.98f)
        {
            plStat.DoEmoji(3);
        }
#endif

        // quick throw
        if (Input.GetButtonDown(xboxController.x))
        {
            QuickThrow();
        }

        if (currentPlayerState == PlayerState.Throwing)
        {
            PowerUpThrow();
        }

        ChargeQuickThrow();
    }

    public void PickupClosestBall()
    {
        if(ballInZone.Count > 0)
        {
            currentPlayerState = PlayerState.PickingUp;
            PickupBall(ballInZone[0]);
        }
    }

    public void DropBall()
    {
        if (ballBeingHeld != null)
        {
            ballBeingHeld.Drop();
            throwSpeedModifier = 1f;
            ballBeingHeld = null;
            throwPower = 0f;
            throwMeterBG.enabled = false;
            throwMeterFill.enabled = false;
            SetPlayerState(PlayerState.Idle);
        }
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
                if (!ballThrown && ThrowBall(GameManager.instance.gamePreferences.quickThrowSpeed * GameManager.instance.gamePreferences.throwSpeed))
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

        if(!ballThrown && ballBeingHeld) AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Error);
    }

    public bool ReceiveHit(Ball ball)
    {
        if (ball && ball.lastThrownBy.Team == plStat.Team)
        {
            // hitting teammate
            return false;
        }

        if(currentPlayerState == PlayerState.Respawning)
        {
            return false;
        }

        DropBall();
        GameObject exp = Instantiate(explosion, transform);
        Destroy(exp, 1f);
        SetPlayerState(PlayerState.Dead);
        rigidbody2D.simulated = false;
        timeOfDeath = Time.time;
        GetComponent<Animator>().SetTrigger("Die");
        AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Explosion);
        return true;
    }

    public void BallCaught(Ball ball)
    {
        ReceiveHit(null);
    }

    public void SetPlayerPosition(Vector3 playerPosition)
    {
        transform.position = playerPosition;
        rigidbody2D.velocity = Vector2.zero;
    }

    public void RespawnPlayer()
    {
        SetPlayerState(PlayerState.Respawning);
        GetComponent<Animator>().SetTrigger("Respawn");
        rigidbody2D.simulated = true;
        timeOfRespawn = Time.time;
    }

    public void SetPlayerState(PlayerState ps)
    {
        previousPlayerState = currentPlayerState;
        currentPlayerState = ps;
    }

    private void Dash()
    {
        if (currentPlayerState != PlayerState.Throwing)
        {
            rigidbody2D.AddForce(normalizedDirection * dashMagnitude);
            plStat.audioSource.PlayOneShot(plStat.swish);
        }
        else
        {
            AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Error);
        }
    }

    public void PickupBall(Ball ball)
    {
        if (ballBeingHeld == null)
        {
            if (ball.currentBallState != Ball.BallState.Carry)
            {
                if (ball.currentBallState == Ball.BallState.Thrown)
                {
                    if (plStat.Team != ball.lastThrownBy.Team)
                    {
                        //caught ball thrown by other team
                        ball.lastThrownBy.playerController.BallCaught(ball);
                    }
                }

                AudioManager.instance.PlaySFX(AudioManager.AudioSFX.SwipeIn);

                ball.Pickup();
                ballBeingHeld = ball;
                ball.transform.SetParent(characterPlayer);
                ball.transform.position = Random.Range(0f, 1f) > 0.5f ? rightHand.position : leftHand.position;
            }
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
        throwPower = Mathf.Min(((Time.time - throwStartTime) / GameManager.instance.gamePreferences.FULL_POWER_TIME), 1f);
        throwMeterFill.fillAmount = throwPower / 2f;
    }

    public bool ThrowBall(float power)
    {
        if (ballBeingHeld != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.AudioSFX.SwipeOut);
            ballBeingHeld.Throw(power, GameManager.instance.gamePreferences.throwSpeed, normalizedDirection, plStat);
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
}
