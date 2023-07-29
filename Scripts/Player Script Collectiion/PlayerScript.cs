using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private static GameObject instance;
    #region SCRIPTS
    [SerializeField] internal PlayerInputScript inputScript;
    [SerializeField] internal PlayerAnimationScript animationScript;
    [SerializeField] internal PlayerStatsScript statsScript;
    [SerializeField] internal PlayerSpawnerScript spawnerScript;
    [SerializeField] internal PlayerCollisionScript collisionScript;
    [SerializeField] internal PlayerHorizontalMovementScript horizontalMovementScript;
    [SerializeField] internal PlayerJumpScript jumpScript;
    [SerializeField] internal PlayerDashScript dashScript;
    [SerializeField] internal PlayerWallInteractionScript wallInteractionScript;
    [SerializeField] internal PlayerGrappleScript grappleScript;
    [SerializeField] internal PlayerCombatScript combatScript;
    
    #endregion

    #region COMPONENTS
    internal Rigidbody2D rb;
    internal Animator anim;
    internal SpringJoint2D sj;
    internal Renderer rend;
    #endregion

    void Awake() 
    {
        if (instance == null)
        {
            instance = GameObject.FindWithTag("Player");;
            DontDestroyOnLoad(instance);
        }
        else 
        {
            Destroy(instance);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sj =  GetComponent<SpringJoint2D>();
        rend = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        ControlGravityScale();
    }

    void ControlGravityScale()
    {
        if (wallInteractionScript.isWallGrabbing || wallInteractionScript.isWallSliding)
        {
            rb.gravityScale = 0f;
            return;
        }
        if (dashScript.isDashing)
        {
            rb.gravityScale = 0;
            return;
        }
        if (grappleScript.isGrappling && grappleScript.isNearExtrema)
        {
            rb.gravityScale = grappleScript.extremaGravityFactor;
            return;
        }
        else if (grappleScript.isGrappling && !grappleScript.isNearExtrema)
        {
            rb.gravityScale = grappleScript.grappleGravityFactor;
            return;
        }
        if (grappleScript.isGrappleDashing)
        {
            rb.gravityScale = 0f;
            return;
        }
        if (jumpScript.isNearApex)
        {
            rb.gravityScale = jumpScript.apexGravityFactor;
            return;
        }
        else if (rb.velocity.y < 0 || jumpScript.endedJump)
        {
            rb.gravityScale = jumpScript.descentGravityFactor;
            return;
        }
        else if (rb.velocity.y > 0)
        {
            rb.gravityScale = jumpScript.ascentGravityFactor;
            return;
        }
        rb.gravityScale = 1;
    }
}
