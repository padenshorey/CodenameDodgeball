using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    void Start()
    {
        plStat = GetComponent<PlayerStat>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        walkSpeed = (float)(plStat.Speed + (plStat.Agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);
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

        Vector2 movementLerp = Vector2.Lerp(new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y), analogAxis * curSpeed, lerpSpeed);

        rigidbody2D.velocity = movementLerp;

        Vector2 staticVector = new Vector2(0f, 1f);

        float rotationAngleRelative = Mathf.Atan2(analogAxis.y, -analogAxis.x) -  Mathf.Atan2(staticVector.y, staticVector.x);
        rotationAngleRelative *= Mathf.Rad2Deg;
        rotationAngleRelative *= -1f;

        characterPlayer.eulerAngles = new Vector3(0, 0, rotationAngleRelative);
    }


}
