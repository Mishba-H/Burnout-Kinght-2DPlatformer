using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;

    #region DASH VARIABLES
    private Vector2 dashDirection;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldownTime;
    [SerializeField] private float postDashVelocity;
    private float dashCooldownTimer;
    private bool canDash = true;
    internal bool isDashing = false;
    internal bool disableDash = false;
    #endregion

    private void FixedUpdate() 
    {
        EnablePlayerDash();
        
        dashCooldownTimer -= Time.fixedDeltaTime;
        if (disableDash) {canDash = false;  return;}

        if (playerScript.collisionScript.isGrounded && dashCooldownTimer < 0)
        {
            canDash = true;
        }
    }

    internal void Dash()
    {   
        if(canDash)
        {
            dashDirection = playerScript.inputScript.dashDirection;

            PlayDashAnimations();

            isDashing = true;
            dashCooldownTimer = dashCooldownTime;
            playerScript.rb.gravityScale = 0f;
            playerScript.rb.sharedMaterial = playerScript.collisionScript.frictionless;
            playerScript.rb.velocity = new Vector2(dashDirection.x * dashForce, dashDirection.y * dashForce);
            StartCoroutine(EndDash());

            playerScript.horizontalMovementScript.disableMove = true;
            playerScript.jumpScript.DisablePlayerJump();
            playerScript.wallInteractionScript.disableWallGrab = true;
        }
        canDash = false;
    }
    IEnumerator EndDash()
    {
        yield return new WaitForSeconds(dashDuration);
        playerScript.rb.velocity = new Vector2(dashDirection.x * postDashVelocity, dashDirection.y * postDashVelocity);
        isDashing = false;
    }

    private void EnablePlayerDash()
    {
        if (!playerScript.grappleScript.isGrappling && !playerScript.grappleScript.isGrappleDashing && 
            !playerScript.combatScript.isAttacking && !playerScript.combatScript.isShooting)
        {
            disableDash = false;
        }
    }

    void PlayDashAnimations()
    {
        if (playerScript.collisionScript.isGrounded)
        {
            if (dashDirection.y == 0)
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dash);
            }
            else if (dashDirection.y == 1) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashUpwards);
            }
            else if (dashDirection.y > 0) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashDiagonallyUpwards);
            }
        }
        else
        {
            if (dashDirection.y == 0)
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dash);
            }
            else if (dashDirection.y == 1) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashUpwards);
            }
            else if (dashDirection.y == -1) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashDownwards);
            }
            else if (dashDirection.y > 0) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashDiagonallyUpwards);
            }
            else if (dashDirection.y < 0) 
            {
                playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.dashDiagonallyDownwards);
            }
        }
    }
}
