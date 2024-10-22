using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveScript : MonoBehaviour
{
    internal Rigidbody2D rb;
    EnemyStatsScript statsScript;
    DetectPlayerScript detectPlayerScript;
    [SerializeField] private float stoppingDistance = 4f;
    [SerializeField] private float minPauseDuration = 2f;
    [SerializeField] private float maxPauseDuration = 4f;
    [SerializeField] private float minPauseInterval = 3f;
    [SerializeField] private float maxPauseInterval = 5f;
    private float pauseIntervalCounter;
    internal float pauseCounter;
    [SerializeField] internal float moveSpeed = 3f;
    [SerializeField] private bool boundsEnabled = false;
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    [SerializeField] private Transform detector; 
    [SerializeField] private float detectRadius = 2f;
    internal bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    internal RaycastHit2D wallRay;
    [SerializeField] private LayerMask wallLayer;

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        detectPlayerScript = GetComponent<DetectPlayerScript>();
        statsScript = GetComponent<EnemyStatsScript>();
        if (leftBound == null || rightBound == null) boundsEnabled = false;
        pauseIntervalCounter = Random.Range(minPauseInterval, maxPauseInterval + 1);
    }

    void FixedUpdate() 
    {
        if (statsScript.health == 0) return;

        Move();
        PauseMovement();

        if (pauseCounter > 0f) return;

        GroundCheck();
        WallCheck();
        if (boundsEnabled) BoundsCheck();
    }

    void Move()
    {
        var distanceFromPlayer = Vector3.Magnitude(GameObject.FindWithTag("Player").transform.position - 
            detector.position);
        if (pauseCounter > 0f && !detectPlayerScript.isAgro)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (detectPlayerScript.isAgro && distanceFromPlayer < stoppingDistance)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (statsScript.health == 0 || statsScript.isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!detectPlayerScript.isAgro)
        {
            rb.velocity = new Vector2(statsScript.isFacingRight? moveSpeed : -moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(statsScript.isFacingRight? moveSpeed * 1.5f : -moveSpeed * 1.5f, rb.velocity.y);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(detector.position, detectRadius, groundLayer);
        if (!isGrounded) 
        {
            statsScript.isFacingRight = !statsScript.isFacingRight;
            detectPlayerScript.isAgro = false;
        }
    }

    void WallCheck()
    {
        wallRay = Physics2D.Raycast(detector.position, statsScript.isFacingRight ? Vector2.right : Vector2.left, 0.5f, wallLayer);
        if (wallRay.collider != null)
        {
            statsScript.isFacingRight = !statsScript.isFacingRight;
            detectPlayerScript.isAgro = false;
        }
    }

    void BoundsCheck()
    {
        if (!(detector.position.x > leftBound.position.x && detector.position.x < rightBound.position.x))
        {
            statsScript.isFacingRight = !statsScript.isFacingRight;
        }
    }

    void PauseMovement()
    {
        if (pauseIntervalCounter > 0f)
        {
            pauseIntervalCounter -= Time.fixedDeltaTime;
            if (pauseIntervalCounter <= 0f)
            {
                pauseCounter = Random.Range(minPauseDuration, maxPauseDuration + 1);
            }
        }
        if (pauseCounter > 0f)
        {
            pauseCounter -= Time.fixedDeltaTime;
            if (pauseCounter <= 0f) 
            {
                pauseIntervalCounter = Random.Range(minPauseInterval, maxPauseInterval + 1);
            }
        }
    }
}
