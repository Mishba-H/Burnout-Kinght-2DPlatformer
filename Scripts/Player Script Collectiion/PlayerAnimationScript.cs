using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;

    #region ANIMATION STATES
    internal string currentState;
    internal string spawn = "Spawn";
    internal string death = "Death";
    internal string idle = "Idle";
    private float idleTime = 0f;
    [SerializeField] internal Animation[] idleStates;
    internal string getHit = "GetHit";
    internal string balanceOnEdge = "Balance On Edge";
    private int idleAnimationIndex = 0;
    internal string turn = "Turn";
    internal string walk = "Walk";
    internal string run = "Run";
    internal string stopRun = "Stop Run";
    internal string crouchIdle = "Crouch Idle";
    internal string crouchMove = "Crouch Move";
    internal string jump = "Jump";
    internal string floatInAir = "Float In Air";
    internal string land = "Land";
    internal string dash = "Dash";
    internal string dashUpwards = "Dash Upwards";
    internal string dashDownwards = "Dash Downwards";
    internal string dashDiagonallyUpwards = "Dash Diagonally Upwards";
    internal string dashDiagonallyDownwards = "Dash Diagonally Downwards";
    internal string wallSlide = "Wall Slide";
    internal string wallGrab = "Wall Grab";
    internal string wallGrabFlip = "Wall Grab Flip";
    internal string wallClimbUp = "Wall Climb Up";
    internal string wallJumpBounce = "Wall Jump Bounce";
    internal string wallJumpLeap = "Wall Jump Leap";
    internal string wallJumpClimb = "Wall Jump Climb";
    internal string grapple = "Grapple";
    internal string hang = "Hang";
    internal string swing = "Swing";
    internal string grappleDash = "Grapple Dash";
    internal string attack = "Attack";
    internal string shoot = "Shoot";
    #endregion

    private bool isIdle = false;
    private bool playingIdleStateAnimation = false;
    internal bool isFacingRight = true;
    internal bool isTurning = false;
    [SerializeField] private float turnDuration;
    internal bool playGrappleAnimation = false;

    internal void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        if (playerScript.anim == null) return;
        
        playerScript.anim.Play(newState);
        currentState = newState;
    }

    private void LateUpdate() 
    {
        if (playerScript == null) return;

        if (playerScript.spawnerScript.spawnPlayer)
        {
            ChangeAnimationState(spawn);
            return;
        }

        if (playerScript.statsScript.currentHealth == 0) 
        {
            ChangeAnimationState(death);
            return;
        }

        if (playerScript.combatScript.isAttacking)
        {
            ChangeAnimationState(attack);
            return;
        }

        if (playerScript.combatScript.isShooting)
        {
            ChangeAnimationState(shoot);
            return;
        }

        if (playerScript.statsScript.playGetHit)
        {
            ChangeAnimationState(getHit);
            return;
        }

        FlipPlayer();
        PlayIdleAnimations();
        PlayHorizontalMovementAnimations();
        PlayJumpAnimations();
        PlayWallInteractionAnimations();
    }

    void FlipPlayer()
    {
        if (!playerScript.dashScript.isDashing && !playerScript.wallInteractionScript.isWallJumping &&
            !playerScript.wallInteractionScript.isWallSliding && !playerScript.grappleScript.isGrappling && 
            !playerScript.collisionScript.slopeIsSteep)
        {
            if (playerScript.inputScript.xInputRaw == 1 && !isFacingRight || playerScript.inputScript.xInputRaw == -1 && isFacingRight)
            Flip();
        }

        if (playerScript.collisionScript.slopeIsSteep &&
            (playerScript.rb.velocity.x > 0 && !isFacingRight || playerScript.rb.velocity.x < 0 && isFacingRight))
            Flip();

        if (playerScript.grappleScript.isGrappling &&
            (playerScript.rb.velocity.x > 0.25 && !isFacingRight || playerScript.rb.velocity.x < -0.25 && isFacingRight))
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1; 
        transform.localScale = Scaler;
        if (playerScript.collisionScript.isGrounded && !playerScript.jumpScript.isJumping && !playerScript.dashScript.isDashing &&
            !playerScript.collisionScript.slopeIsSteep && !playerScript.inputScript.isCrouching)
        {
            isTurning = true;
            StartCoroutine(PlayTurnAnimation());
        }
    }

    IEnumerator PlayTurnAnimation()
    {
        ChangeAnimationState(turn);
        yield return new WaitForSeconds(turnDuration);
        isTurning = false;
    }

    void PlayIdleAnimations()
    {
        if (playerScript.collisionScript.isGrounded && playerScript.horizontalMovementScript.directionX == 0 &&
            !playerScript.jumpScript.isJumping && !playerScript.dashScript.isDashing && playerScript.rb.velocity == Vector2.zero &&
            !playerScript.wallInteractionScript.isWallGrabbing && !playerScript.wallInteractionScript.isWallSliding &&
            !playerScript.inputScript.isCrouching)
        {
            isIdle = true;

            if (playerScript.collisionScript.isOnEdge)
            {
                ChangeAnimationState(balanceOnEdge);
                return;
            }
            ChangeAnimationState(idle);

            // if (idleTime > 3 && !playingIdleStateAnimation)
            // {
            //     playingIdleStateAnimation = true;
            //     ChangeAnimationState(idle);
            //     //StartCoroutine(PlayIdleStateAnimation());
            // }
            // else 
            //     idleTime += Time.deltaTime;

            // if (!playingIdleStateAnimation)
            //     ChangeAnimationState(idle);
        }
        else
        {
            isIdle = false;
            idleTime = 0f;
            playingIdleStateAnimation = false;
        }
    }

    // IEnumerator PlayIdleStateAnimation()
    // {
    //     ChangeAnimationState(idleStates[idleAnimationIndex, 0]);
    //     yield return new WaitForSeconds(float.Parse(idleStates[idleAnimationIndex, 1]));
    //     idleTime = 0f;

    //     playingIdleStateAnimation = false;
    //     idleAnimationIndex = Random.Range(0, idleStates.Length / 2);
    // }

    void PlayHorizontalMovementAnimations()
    {
        if (playerScript.horizontalMovementScript.disableMove) return;

        if (playerScript.collisionScript.isGrounded)
        {
            if (playerScript.inputScript.isCrouching && !isTurning && !playerScript.jumpScript.isJumping && 
                !playerScript.collisionScript.slopeIsSteep && !playerScript.dashScript.isDashing)
            {
                if (playerScript.horizontalMovementScript.directionX == 0 && !playerScript.collisionScript.isOnSlope)
                    ChangeAnimationState(crouchIdle);
                else if (!playerScript.collisionScript.isOnSlope)
                    ChangeAnimationState(crouchMove);
            }
            else if(playerScript.inputScript.moveDirection.magnitude >= 0.9 && playerScript.horizontalMovementScript.directionX != 0 &&
                !isTurning && !playerScript.jumpScript.isJumping && !playerScript.collisionScript.slopeIsSteep)  
                ChangeAnimationState(run);
            else if (playerScript.inputScript.moveDirection.magnitude < 0.9 && playerScript.horizontalMovementScript.directionX != 0 &&
                !isTurning && !playerScript.jumpScript.isJumping && !playerScript.collisionScript.slopeIsSteep) 
                ChangeAnimationState(walk);
            else if (playerScript.horizontalMovementScript.directionX == 0 && Mathf.Abs(playerScript.rb.velocity.x) > 0.001f && !isTurning &&
                !playerScript.jumpScript.isJumping && !isIdle && !playerScript.dashScript.isDashing)
                ChangeAnimationState(stopRun);
        }
        else
        {
            if (playerScript.rb.velocity.y < 0 && !playerScript.wallInteractionScript.isWallSliding &&
                !playerScript.grappleScript.isGrappling)
            {
                ChangeAnimationState(floatInAir);
            }
            
            if(playerScript.horizontalMovementScript.directionX  != 0  && playerScript.horizontalMovementScript.canSwing
                && !playGrappleAnimation && playerScript.grappleScript.isGrappling)
            {
                ChangeAnimationState(swing);
            }
            else if (!playGrappleAnimation && playerScript.grappleScript.isGrappling)
                ChangeAnimationState(hang);
        }
    }

    void PlayJumpAnimations()
    {
        if (playerScript.jumpScript.isJumping)
            ChangeAnimationState(jump);
    }

    void PlayWallInteractionAnimations()
    {
        if (playerScript.wallInteractionScript.isWallSliding)
            ChangeAnimationState(wallSlide);
        
        if (playerScript.wallInteractionScript.isWallGrabbing)
        {
            if (playerScript.wallInteractionScript.yDir > 0)
                ChangeAnimationState(wallClimbUp);
            else if (playerScript.wallInteractionScript.yDir < 0)
                ChangeAnimationState(wallSlide);
            else if (playerScript.collisionScript.isFacingLadder)
                ChangeAnimationState(wallGrab);
            else
                ChangeAnimationState(wallGrabFlip);
        }
    }
}