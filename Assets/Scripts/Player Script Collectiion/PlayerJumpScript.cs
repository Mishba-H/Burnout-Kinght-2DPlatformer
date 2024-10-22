using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpScript : MonoBehaviour
{
    #region JUMP VARIABLES
    [SerializeField] PlayerScript playerScript;
    [SerializeField] internal float ascentGravityFactor;
    [SerializeField] internal float apexGravityFactor;
    [SerializeField] internal float descentGravityFactor;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float nearApexVelocity;
    internal bool isJumping = false;
    internal bool isNearApex = false;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDuration;
    internal float jumpTimer;
    [SerializeField] private float jumpCoyoteTime;
    private float jumpCoyoteTimer;
    [SerializeField] internal float jumpBufferTime;
    internal float jumpBufferTimer;
    internal bool endedJump = false;
    internal bool disableJump = false;
    #endregion

    #region AIR JUMP VARIABLES
    [SerializeField] private float airJumpCount;
    [SerializeField] private float airJumpForce;
    private float airJumpCounter;
    internal bool disableAirJump;
    #endregion

    private void Update() 
    {
        EnablePlayerJump();
        EnablePlayerAirJump();

        if (!playerScript.collisionScript.isGrounded)
        {
            jumpCoyoteTimer -= Time.deltaTime;
        }
        else
        {
            if (jumpTimer <= 0f)
            {
                jumpCoyoteTimer = jumpCoyoteTime;
            }
            airJumpCounter = airJumpCount;
        }
        jumpBufferTimer -= Time.deltaTime;
    }

    private void FixedUpdate() 
    {
        Jump();   
        if (jumpTimer <= 0f)
        {
            isJumping = false;
        } 
    }

    private void LateUpdate() 
    {
        // Cancel Apex Check 
        if (playerScript.wallInteractionScript.isWallGrabbing || playerScript.wallInteractionScript.isWallSliding ||
            playerScript.grappleScript.isGrappling) return;

        // Apex Check
        if (!playerScript.collisionScript.isGrounded && 
            playerScript.rb.velocity.y < nearApexVelocity && playerScript.rb.velocity.y > -nearApexVelocity)
        {
            isNearApex = true;
        }
        else 
        {
            isNearApex = false;
        }

        playerScript.rb.velocity = new Vector2(playerScript.rb.velocity.x, 
            Mathf.Clamp(playerScript.rb.velocity.y, -maxFallSpeed, Mathf.Infinity));
    }

    internal void Jump()
    {
        if (disableJump) return;

        if (jumpBufferTimer > 0f && (playerScript.collisionScript.isGrounded || jumpCoyoteTimer > 0f) &&
            !playerScript.collisionScript.slopeIsSteep)
        {
            isJumping = true;
            jumpTimer = jumpDuration;
            jumpBufferTimer = 0f;
            jumpCoyoteTimer = 0f;
            playerScript.rb.velocity = new Vector2(playerScript.rb.velocity.x, jumpForce);
            playerScript.wallInteractionScript.wallJumpBufferTimer = 0f;
            disableAirJump = true;
            
            playerScript.animationScript.isTurning = false;
            playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.jump);
        }
        else
        {
            jumpTimer -= Time.fixedDeltaTime;
        }

        if (jumpTimer > 0f)
        {
            playerScript.rb.velocity = new Vector2(playerScript.rb.velocity.x, jumpForce);
        }
    }

    internal void AirJump()
    {
        if (disableAirJump) return;

        if (airJumpCounter > 0 && !playerScript.collisionScript.isNearGround && 
            (!playerScript.collisionScript.isNearWall || playerScript.rb.velocity.y > 0))
        {
            playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.jump);

            airJumpCounter -= 1;
            playerScript.rb.velocity = new Vector2(playerScript.rb.velocity.x, airJumpForce);
            playerScript.wallInteractionScript.wallJumpBufferTimer = 0f;
        }
    }

    internal void DisablePlayerJump()
    {
        jumpTimer = 0f;
        disableJump = true;
        isJumping = false;
    }

    private void EnablePlayerJump()
    {
        if (!playerScript.dashScript.isDashing && !playerScript.wallInteractionScript.isWallGrabbing && 
            !playerScript.combatScript.isAttacking && !playerScript.combatScript.isShooting)
        {
            disableJump = false;
        }
    }

    private void EnablePlayerAirJump()
    {
        if (!isJumping && !playerScript.dashScript.isDashing && !playerScript.wallInteractionScript.isWallSliding && 
            !playerScript.wallInteractionScript.isWallGrabbing)
        {
            disableAirJump = false;
        }
    }
}
