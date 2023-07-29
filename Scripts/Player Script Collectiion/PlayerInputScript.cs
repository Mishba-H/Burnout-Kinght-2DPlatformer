using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;
    [SerializeField] PlayerInput playerInput;
    internal PlayerInputActions playerInputActions;

    #region VARIABLES
    internal Vector2 moveDirection;
    internal Vector2 dashDirection;
    internal Vector2 aimDirection;
    internal float xInputRaw;
    internal float yInputRaw;
    internal bool isCrouching = false;
    #endregion

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Jump.performed += JumpPerformed;
        playerInputActions.Player.Jump.performed += AirJump;
        playerInputActions.Player.Jump.canceled += JumpCanceled;
        playerInputActions.Player.Dash.performed += Dash;
        playerInputActions.Player.Grapple.performed += GrapplePerformed;
        playerInputActions.Player.Grapple.canceled += GrappleCanceled;
        playerInputActions.Player.GrappleDash.performed += GrappleDash;
        playerInputActions.Player.Attack.performed += Attack;
        playerInputActions.Player.Shoot.performed += Shoot;
    }

    private void OnEnable() 
    {
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    void FixedUpdate()
    {
        PlayerInputActionMapControl();

        GetMoveInput();

        GetCrouchInput();

        GetAimDirection();

        GetDashDirection();

        GetWallGrabInput();
    }

    void PlayerInputActionMapControl()
    {
        if (playerScript.statsScript.currentHealth == 0)
            playerInputActions.Player.Disable();
        else 
            playerInputActions.Player.Enable();
    }

    void GetMoveInput()
    {
        moveDirection = playerInputActions.Player.Move.ReadValue<Vector2>();

        if (moveDirection.x > 0.2)
            playerScript.horizontalMovementScript.directionX = 1;
        else if (moveDirection.x < -0.2)
            playerScript.horizontalMovementScript.directionX = -1;
        else
            playerScript.horizontalMovementScript.directionX = 0;

        if (playerScript.collisionScript.isGrounded && !playerScript.collisionScript.isOnSlope)
        {
            if (isCrouching)
            {
                playerScript.horizontalMovementScript.CrouchMove();
            }
            else if (moveDirection.magnitude >= 0.9)
            {
                playerScript.horizontalMovementScript.Run();
            }
            else if (moveDirection.magnitude == 0)
            {
                playerScript.horizontalMovementScript.Run();
            }
            else
            {
                playerScript.horizontalMovementScript.Walk();
            }
        }
        else if (playerScript.collisionScript.isGrounded && playerScript.collisionScript.isOnSlope)
        {
            if (moveDirection.magnitude >= 0.9)
            {
                playerScript.horizontalMovementScript.SlopeRun();
            }
            else if (moveDirection.magnitude == 0)
            {
                playerScript.horizontalMovementScript.Run();
            }
            else
            {
                playerScript.horizontalMovementScript.SlopeWalk();
            }
        }
        else if (playerScript.grappleScript.isGrappling) 
        {
            playerScript.horizontalMovementScript.Swing();
        }
        else
        {
            playerScript.horizontalMovementScript.Float();
        }
    }

    void GetCrouchInput()
    {
        if (moveDirection.y < -0.2f && playerScript.collisionScript.isGrounded)
        {
            isCrouching = true;
        }
        else 
        {
            isCrouching = false;
        }
    }

    void GetAimDirection()
    {
        if (playerInput.currentControlScheme.Equals("Gamepad"))
        {
            aimDirection = playerInputActions.Player.GamepadAim.ReadValue<Vector2>() != Vector2.zero?
                playerInputActions.Player.GamepadAim.ReadValue<Vector2>() : aimDirection;
        }
        else if (playerInput.currentControlScheme.Equals("Keyboard & Mouse"))
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(playerInputActions.Player.MouseAim.ReadValue<Vector2>()).x,
                Camera.main.ScreenToWorldPoint(playerInputActions.Player.MouseAim.ReadValue<Vector2>()).y);
            aimDirection = (mousePosition - (Vector2)playerScript.grappleScript.firePoint.position).normalized;
        }
    }

    void GetDashDirection()
    {
        if (playerInputActions.Player.Move.ReadValue<Vector2>().x > 0.2)
        {
            xInputRaw = 1;
        }
        else if (playerInputActions.Player.Move.ReadValue<Vector2>().x < -0.2)
        {
            xInputRaw = -1;
        }
        else
        {
            xInputRaw = Mathf.Abs(playerInputActions.Player.Move.ReadValue<Vector2>().y) < 0.2 ?
                playerScript.horizontalMovementScript.lastDirectionX : 0;
        }
        if (playerInputActions.Player.Move.ReadValue<Vector2>().y > 0.2)
        {
            yInputRaw = 1;
        }
        else if (playerInputActions.Player.Move.ReadValue<Vector2>().y < -0.2)
        {
            yInputRaw = -1;
        }
        else
        {
            yInputRaw = 0;
        }
        dashDirection = new Vector2(xInputRaw, yInputRaw).normalized;
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        playerScript.wallInteractionScript.wallJumpBufferTimer = playerScript.wallInteractionScript.wallJumpBuffer;

        if (playerScript.grappleScript.isGrappling)
            playerScript.grappleScript.grappleJumpBufferTimer = playerScript.grappleScript.grappleJumpBuffer;

        playerScript.jumpScript.jumpBufferTimer = playerScript.jumpScript.jumpBufferTime;
        playerScript.jumpScript.endedJump = false;
        playerScript.jumpScript.Jump();
    }
    private void JumpCanceled(InputAction.CallbackContext context)
    {
        playerScript.jumpScript.jumpTimer = 0f;
        playerScript.jumpScript.endedJump = true;
    }

    private void AirJump(InputAction.CallbackContext context)
    {
        playerScript.jumpScript.AirJump();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        playerScript.dashScript.Dash();
    }

    void GetWallGrabInput()
    {
        if (playerInputActions.Player.WallGrab.ReadValue<float>() > 0.3f)
        {
            playerScript.wallInteractionScript.WallGrab();
        }
        else
        {
            playerScript.wallInteractionScript.isWallGrabbing = false;
        }
    }

    private void GrapplePerformed(InputAction.CallbackContext context)
    {
        playerScript.grappleScript.updateGrapplePoint = true;
        playerScript.grappleScript.Grapple();
    }

    private void GrappleCanceled(InputAction.CallbackContext context)
    {
        playerScript.grappleScript.isGrappling = false;
        playerScript.sj.enabled = false;
    }

    private void GrappleDash(InputAction.CallbackContext context)
    {
        playerScript.grappleScript.GrappleDash();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (playerScript.collisionScript.isGrounded && !playerScript.dashScript.isDashing && 
            playerScript.combatScript.attackIntervalCounter <= 0)
        playerScript.combatScript.isAttacking = true;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (playerScript.collisionScript.isGrounded && !playerScript.dashScript.isDashing &&
            playerScript.combatScript.shootIntervalCounter <= 0)
        playerScript.combatScript.isShooting = true;
    }
}