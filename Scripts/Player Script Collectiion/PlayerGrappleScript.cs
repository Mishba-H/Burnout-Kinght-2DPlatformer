using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrappleScript : MonoBehaviour
{
    [SerializeField] GrappleRopeScript grappleRope;
    internal bool isGrappling = false;
    internal bool isGrappleDashing = false;
    internal bool updateGrapplePoint = true;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] internal Transform firePoint;
    [SerializeField] private float maxGrappleDistance;
    internal Vector2 grapplePoint;
    internal Vector2 grappleDirectionVector;
    [SerializeField] internal float grappleGravityFactor;
    [SerializeField] internal Vector2 grappleJumpForce;
    [SerializeField] internal float grappleJumpBuffer;
    internal float grappleJumpBufferTimer;
    internal bool isNearExtrema = false;
    [SerializeField] private float nearExtremaVelocity;
    [SerializeField] internal float extremaGravityFactor;
    [SerializeField] private float del;
    [SerializeField] private Vector2 grappleDashForce;
    Vector2 gunDirection = Vector2.zero;
    internal bool disableGrapple = false;
    internal bool disableGrappleDash = false;

    private void Start()
    {
        playerScript.sj.enabled = false;
    }

    private void FixedUpdate()
    {
        EnableGrapple();
        EnableGrappleDash();

        if (playerScript.collisionScript.isGrounded)
        {
            disableGrapple = true;
            playerScript.sj.enabled = false;
            isGrappling = false;
        }

        if (!isGrappling)
        {
            grappleRope.enabled = false;
            disableGrappleDash = true;
        }

        playerScript.collisionScript.grappleRayLength = maxGrappleDistance;

        if (grappleJumpBufferTimer > 0f && isGrappling)
        {
            grappleJumpBufferTimer = 0f;
            isGrappling = false;
            playerScript.sj.enabled = false;
            GrappleJump();
        }
        else
        {
            grappleJumpBufferTimer -= Time.fixedDeltaTime;
        }

        if (isGrappleDashing)
        {
            playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.grappleDash);

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(grapplePoint.x, grapplePoint.y , 0), del);
            if (Vector2.Distance((Vector2)transform.position, grapplePoint) < 0.3)
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.jump);

                playerScript.rb.velocity = grappleDashForce;
                isGrappleDashing = false;
            }
        }
    }

    private void LateUpdate() 
    {
        // Cancel Extrema Check 
        if (!isGrappling) return;

        // Extrema Check
        if (playerScript.rb.velocity.x < nearExtremaVelocity && playerScript.rb.velocity.x > -nearExtremaVelocity)
        {
            isNearExtrema = true;
        }
        else 
        {
            isNearExtrema = false;
        }
    }

    void SetGrapplePoint()
    {
        if (playerScript.collisionScript.grappleRay.collider != null && updateGrapplePoint)
        {
            grapplePoint = (Vector2)playerScript.collisionScript.grappleRay.transform.localPosition;
            updateGrapplePoint = false;
        }
        else if (playerScript.collisionScript.grappleRay.collider == null && updateGrapplePoint)
        {
            grapplePoint = Vector2.zero;
            updateGrapplePoint = false;
            return;
        }

        if (transform.position.y > grapplePoint.y - 0.3f) return;
        if (Vector2.Distance(grapplePoint, (Vector2)firePoint.position) > maxGrappleDistance) return;

        isGrappling = true;
        playerScript.dashScript.disableDash = true;
        grappleDirectionVector = grapplePoint - (Vector2)firePoint.position;
    }

    internal void Grapple()
    {   
        if (disableGrapple) return;

        SetGrapplePoint();
        if (!isGrappling) return;

        StartCoroutine(GrappleAnimationControl());

        playerScript.sj.autoConfigureDistance = true;
        playerScript.sj.frequency = 0;
        playerScript.sj.connectedAnchor = grapplePoint;
        playerScript.sj.enabled = true;
        grappleRope.enabled = true;
    }

    private void GrappleJump()
    {
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.jump);

        playerScript.rb.velocity = new Vector2(playerScript.horizontalMovementScript.lastDirectionX *
            grappleJumpForce.x,grappleJumpForce.y);
    }

    internal void GrappleDash()
    {
        if (disableGrappleDash) return;

        isGrappling = false;
        isGrappleDashing = true;
        playerScript.sj.enabled = false;
        playerScript.horizontalMovementScript.disableMove = true;
        playerScript.rb.velocity = Vector2.zero;
    }

    private void EnableGrapple()
    {
        if (!playerScript.collisionScript.isGrounded)
        {
            disableGrapple = false;
        }

    }

    private void EnableGrappleDash()
    {
        if (isGrappling)
        {
            disableGrappleDash = false;
        }
    }

    IEnumerator GrappleAnimationControl()
    {
        playerScript.animationScript.playGrappleAnimation = true;
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.grapple);
        yield return new WaitForSeconds(0.6f);
        playerScript.animationScript.playGrappleAnimation = false;
    }
}
