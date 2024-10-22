using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHorizontalMovementScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;

    #region HORIZONTAL MOVEMENT VARIABLES
    internal float directionX;
    internal float lastDirectionX;
    internal float currentVelocityX;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float floatSpeed;
    [SerializeField] private float crouchMoveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float airMoveForce;
    [SerializeField] private float airDeceleration;
    [SerializeField] private float apexBonus;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float swingForce;
    [SerializeField] private float swingDeceleration;
    internal bool canSwing = true;
    [SerializeField] private float swingDuration;
    internal bool disableMove = false;
    #endregion

    void FixedUpdate()
    {
        EnablePlayerMove();
        lastDirectionX = directionX != 0 ? directionX : lastDirectionX;
    }

    internal void Walk()
    {
        if (disableMove) return;

        if(directionX != 0 && !playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity += (directionX * new Vector2(acceleration * Time.fixedDeltaTime, 0f).magnitude) *
                playerScript.collisionScript.slopeParallel.normalized;
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -walkSpeed, walkSpeed), playerScript.rb.velocity.y);
        }
    }

    internal void Run()
    {
        if (disableMove) return;

        if(directionX != 0 && !playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity += (directionX * new Vector2(acceleration * Time.fixedDeltaTime, 0f).magnitude) *
                playerScript.collisionScript.slopeParallel.normalized;
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -runSpeed, runSpeed), playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x > 0f)
        {
            playerScript.rb.velocity -= new Vector2(deceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, 0f, runSpeed), playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x < 0f)
        {
            playerScript.rb.velocity += new Vector2(deceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -runSpeed, 0f), playerScript.rb.velocity.y);
        }
    }

    internal void SlopeWalk()
    {
        if (disableMove) return;

        if (!playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity = directionX * walkSpeed * playerScript.collisionScript.slopeParallel.normalized;
        }
    }

    internal void SlopeRun()
    {
        if (disableMove) return;

        if (!playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity = directionX * runSpeed * playerScript.collisionScript.slopeParallel.normalized;
        }
    }

    internal void CrouchMove()
    {
        if (disableMove) return;

        if(directionX != 0 && !playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity += directionX * new Vector2(acceleration * Time.fixedDeltaTime, 0f).magnitude *
                playerScript.collisionScript.slopeParallel.normalized;
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -crouchMoveSpeed, crouchMoveSpeed),
                playerScript.rb.velocity.y);
        }
        else if (directionX == 0 && !playerScript.collisionScript.slopeIsSteep)
        {
            playerScript.rb.velocity = Vector2.zero;
        }
    }

    internal void Float()
    {
        if (disableMove) return;

        if (playerScript.jumpScript.isNearApex && directionX != 0)
        {
            playerScript.rb.AddForce(new Vector2(directionX * airMoveForce * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -(floatSpeed + apexBonus), (floatSpeed + apexBonus)), 
                playerScript.rb.velocity.y);
        }
        else if (directionX != 0)
        {
            playerScript.rb.AddForce(new Vector2(directionX * airMoveForce * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -floatSpeed, floatSpeed),
                playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x > 0f)
        {
            playerScript.rb.velocity -= new Vector2(airDeceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, 0f, floatSpeed), playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x < 0f)
        {
            playerScript.rb.velocity += new Vector2(airDeceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -floatSpeed, 0f), playerScript.rb.velocity.y);
        }
    }

    internal void Swing()
    {
        if (disableMove) return;

        if(directionX != 0  && canSwing)
        {
            StartCoroutine(SwingControl());
            playerScript.rb.velocity += new Vector2(directionX * swingForce * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -swingSpeed, swingSpeed), playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x > 0f &&
            transform.position.x > playerScript.grappleScript.grapplePoint.x)
        {
            playerScript.rb.velocity -= new Vector2(swingDeceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, 0f, swingSpeed), playerScript.rb.velocity.y);
        }
        else if (playerScript.rb.velocity.x < 0f &&
            transform.position.x < playerScript.grappleScript.grapplePoint.x)
        {
            playerScript.rb.velocity += new Vector2(swingDeceleration * Time.fixedDeltaTime, 0f);
            playerScript.rb.velocity = new Vector2(Mathf.Clamp(playerScript.rb.velocity.x, -swingSpeed, 0f), playerScript.rb.velocity.y);
        }
    }
    IEnumerator SwingControl()
    {
        yield return new WaitForSeconds(swingDuration);
        canSwing = false;
        yield return new WaitForSeconds(swingDuration);
        canSwing = true;
    }

    private void EnablePlayerMove()
    {
        if (!playerScript.dashScript.isDashing && !playerScript.wallInteractionScript.isWallGrabbing &&
            !playerScript.grappleScript.isGrappleDashing && !playerScript.combatScript.isAttacking &&
            !playerScript.combatScript.isShooting)
        {
            disableMove = false;
        }
    }
}
