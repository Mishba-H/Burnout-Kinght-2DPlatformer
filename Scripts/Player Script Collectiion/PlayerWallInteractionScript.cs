using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallInteractionScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;

    #region WALL MECHANICS VARIABLES
    [SerializeField] private float maxWallStamina;
    private float currentWallStamina;
    internal float yDir;
    [SerializeField] private float wallClimbUpVelocity;
    [SerializeField] private float wallClimbDownVelocity;
    [SerializeField] private float wallSlideVelocity;
    [SerializeField] private float wallSlideAcceleration;
    internal bool isWallGrabbing = false;
    internal bool isWallSliding = false;
    internal bool isWallJumping = false;
    private float wallDirection;
    private float ladderDirection;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] internal float wallJumpBuffer;
    internal float wallJumpBufferTimer;
    [SerializeField] private Vector2 wallClimbForce = Vector2.zero;
    [SerializeField] private Vector2 wallBounceForce= Vector2.zero;
    [SerializeField] private Vector2 wallLeapForce= Vector2.zero;
    internal bool disableWallGrab = false;
    internal bool disableWallSlide = false;
    #endregion

    private void FixedUpdate() 
    {
        if (playerScript.collisionScript.isGrounded)
        {
            currentWallStamina = maxWallStamina;
        }
        if (isWallGrabbing && yDir == 0)
            currentWallStamina -= Time.fixedDeltaTime;
        else if (isWallGrabbing && yDir == 1)
            currentWallStamina -= Time.fixedDeltaTime * 1.33f;

        wallDirection = playerScript.collisionScript.isFacingWall ?
            playerScript.inputScript.xInputRaw : -playerScript.inputScript.xInputRaw;

        ladderDirection = playerScript.collisionScript.isFacingLadder ?
            playerScript.inputScript.xInputRaw : -playerScript.inputScript.xInputRaw;

        if (wallJumpBufferTimer > 0f)
        {
            if (playerScript.wallInteractionScript.isWallGrabbing && 
                playerScript.collisionScript.isFacingLadder)
            {
                playerScript.wallInteractionScript.WallJumpClimb();
                wallJumpBufferTimer = 0f;
            }
            else if (playerScript.wallInteractionScript.isWallGrabbing && !playerScript.collisionScript.isFacingLadder)
            {
                playerScript.wallInteractionScript.WallJumpLeap();
                wallJumpBufferTimer = 0f;
            }
            else if (playerScript.wallInteractionScript.isWallSliding)
            {
                playerScript.wallInteractionScript.WallJumpBounce();
                wallJumpBufferTimer = 0f;
            }
        }
        else
        {
            wallJumpBufferTimer -= Time.fixedDeltaTime;
        }

        if (isWallJumping)
        {
            playerScript.horizontalMovementScript.disableMove = true;
            isWallGrabbing = false;
            isWallSliding = false;
            disableWallGrab = true;
            disableWallSlide = true;
            return;
        }
        else if (!playerScript.dashScript.isDashing)
        {
            disableWallGrab = false;
            disableWallSlide = false;
        }

        WallSlide();

        if (!playerScript.collisionScript.isWalled && playerScript.collisionScript.isGrounded)
        {
            isWallGrabbing = false;
            isWallSliding = false;
        }
        if (isWallGrabbing)
        {
            playerScript.jumpScript.disableAirJump = true;
            playerScript.horizontalMovementScript.disableMove = true;
            return;
        }
        if (isWallSliding)
        {
            playerScript.jumpScript.disableAirJump = true;
            return;
        }
    }

    private void WallSlide()
    {
        if (isWallGrabbing || disableWallSlide || playerScript.collisionScript.isGrounded)
        {
            isWallSliding = false;
            return;
        }

        if (playerScript.collisionScript.isWalled && 
            Mathf.Abs(playerScript.inputScript.moveDirection.x) > 0.2 && 
            !playerScript.collisionScript.isGrounded &&
            playerScript.rb.velocity.y <= 0f)
        {
            isWallSliding = true;
            playerScript.jumpScript.disableAirJump = true;
            playerScript.rb.velocity += new Vector2(0f, -wallSlideAcceleration * Time.fixedDeltaTime);
            playerScript.rb.velocity = new Vector2(playerScript.rb.velocity.x,
                Mathf.Clamp(playerScript.rb.velocity.y, -wallSlideVelocity, 0f));
        }
        else
        {
            isWallSliding = false;
        }
    }

    internal void WallGrab()
    {
        if (disableWallGrab || currentWallStamina <= 0f || isWallJumping ||
            playerScript.collisionScript.isGrounded || !playerScript.collisionScript.isOnLadder) 
        {
            isWallGrabbing = false;
            return;
        }

        isWallGrabbing = true;
        isWallSliding = false;

        // Get wall climb direction
        if (playerScript.collisionScript.isNearTop)
        {
            yDir = playerScript.inputScript.yInputRaw == 1 ? 0 : playerScript.inputScript.yInputRaw;
        }
        else if (!playerScript.collisionScript.isFacingLadder)
        {
            yDir = 0f;
        }
        else
        {
            yDir = playerScript.inputScript.yInputRaw;
        }

        // Wall Climb
        if (yDir >= 0)
        {
            playerScript.rb.velocity = new Vector2(0f, yDir * wallClimbUpVelocity);
        }
        else
        {
            playerScript.rb.velocity = new Vector2(0f, yDir * wallClimbDownVelocity);
        }
    }

    internal void WallJumpClimb()
    {
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.wallJumpClimb);

        currentWallStamina -= 1.75f;

        isWallJumping = true;
        playerScript.rb.velocity = new Vector2(ladderDirection * wallClimbForce.x, wallClimbForce.y);
        StartCoroutine(EndWallJump());
    }

    internal void WallJumpBounce()
    {
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.wallJumpBounce);

        currentWallStamina -= 1.75f;

        isWallJumping = true;
        playerScript.rb.velocity = new Vector2(-wallDirection * wallBounceForce.x, wallBounceForce.y);
        StartCoroutine(EndWallJump());
    }

    internal void WallJumpLeap()
    {
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.wallJumpLeap);

        isWallJumping = true;
        playerScript.rb.velocity = new Vector2(-ladderDirection * wallLeapForce.x, wallLeapForce.y);
        StartCoroutine(EndWallJump());
    }

    IEnumerator EndWallJump()
    {
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    private void EnablePlayerWallGrab()
    {
        if (!playerScript.dashScript.isDashing && !playerScript.collisionScript.isGrounded)
        {
            disableWallGrab = false;
        }
    }

    private void EnablePlayerWallSlide()
    {
        if (!playerScript.dashScript.isDashing)
        {
            disableWallSlide = false;
        }
    }
}
