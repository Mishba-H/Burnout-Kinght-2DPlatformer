using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;

    #region GROUND CHECK VARIABLES
    internal bool isGrounded = false;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckRadius;
    private RaycastHit2D groundCheckerRay;
    [SerializeField] private float groundCheckerLength;
    [SerializeField] private Transform groundCheckerPoint;
    [SerializeField] private LayerMask groundLayer;
    internal bool isNearGround = false;
    [SerializeField] private float groundRayLength;
    internal bool isOnEdge;
    private RaycastHit2D groundRay;
    private RaycastHit2D edgeHit;
    [SerializeField] private LayerMask edgeLayer;
    internal bool isOnSlope;
    internal Vector2 slopeParallel;
    internal float slopeAngleVertical;
    internal float slopeAngleHorizontal;
    [SerializeField] private float maxSlopeAngle;
    internal bool slopeIsSteep = false;
    [SerializeField] internal PhysicsMaterial2D frictionless;
    [SerializeField] internal PhysicsMaterial2D fullFriction;
    #endregion

    #region WALL CHECK VARIABLES
    internal bool isWalled = false;
    internal bool isOnLadder = false;
    internal bool isFacingWall;
    internal bool isFacingLadder;
    internal bool isNearWall = false;
    [SerializeField] private Transform wallCheckerA;
    [SerializeField] private Transform wallCheckerB;
    [SerializeField] private float wallCheckRadius;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask ladderLayer;
    private RaycastHit2D wallRay;
    [SerializeField] private Transform wallRaycastPoint;
    private Vector2 wallRayDirection;
    [SerializeField] private float wallRayLength;
    internal bool isNearTop = false;
    #endregion

    #region AIM CHECK VARIABLES
    internal RaycastHit2D grappleRay;
    private Vector2 grappleDirection;
    [SerializeField] private LayerMask grappleLayer;
    internal float grappleRayLength;
    #endregion

    void FixedUpdate()
    {
        CheckGroundCollision();
        CheckEdge();
        CheckSlope();
        CheckWallCollision();
        CheckLadderCollision();
        CheckGrapplePoint();
    }

    void CheckGroundCollision()
    {
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);

        groundCheckerRay = Physics2D.Raycast(groundCheckerPoint.position, playerScript.animationScript.isFacingRight? 
            Vector2.right : Vector2.left, groundCheckerLength, groundLayer);
        
        if (isOnSlope || isOnEdge)
        {
            if (groundCheckerRay.collider != null)
                isGrounded = true;
            else 
                isGrounded = false; 
        }
        if (isGrounded) playerScript.wallInteractionScript.disableWallGrab = true;

        groundRay = Physics2D.Raycast(groundChecker.position, Vector2.down, groundRayLength, groundLayer);
        if (groundRay.collider != null)
            isNearGround = true;
        else 
            isNearGround = false;
    }

    void CheckEdge()
    {
        edgeHit = Physics2D.Raycast(groundChecker.position, Vector2.down, groundRayLength, edgeLayer);
        if (edgeHit.collider != null) 
        {    
            isOnEdge = true;
            isGrounded = true;
        }
        else
        {    
            isOnEdge = false;
        }
    }

    void CheckSlope()
    {
        GetGroundSlopeVertical();

        GetGroundSlopeHorizontal();

        if (slopeAngleVertical == 0 && slopeAngleHorizontal == 0)
            isOnSlope = false;
        else
            isOnSlope = true;

        if (slopeAngleVertical > maxSlopeAngle || slopeAngleHorizontal > maxSlopeAngle)
        {
            playerScript.rb.sharedMaterial = frictionless;
            slopeIsSteep = true;
        }
        else
        {
            slopeIsSteep = false;
        }

        if (isGrounded && playerScript.dashScript.isDashing)
            playerScript.rb.sharedMaterial = frictionless;
        else if (isGrounded && !slopeIsSteep && playerScript.horizontalMovementScript.directionX == 0)
            playerScript.rb.sharedMaterial = fullFriction;
            
        else 
            playerScript.rb.sharedMaterial = frictionless;
    }

    void GetGroundSlopeVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundChecker.position, Vector2.down, groundCheckerLength, groundLayer);
        if (hit)
        {
            slopeParallel = -Vector2.Perpendicular(hit.normal);
            slopeAngleVertical = Vector2.Angle(hit.normal, Vector2.up);

            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawRay(hit.point, slopeParallel, Color.red);
        }
    }

    void GetGroundSlopeHorizontal()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(groundChecker.position, Vector2.right, groundCheckerLength, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(groundChecker.position, Vector2.left, groundCheckerLength, groundLayer);

        if (hitRight)
        {
            Debug.DrawRay(hitRight.point, hitRight.normal, Color.blue);
            slopeAngleHorizontal = Vector2.Angle(hitRight.normal, Vector2.up) == 90 ? 0 : Vector2.Angle(hitRight.normal, Vector2.up);
        }
        if (hitLeft)
        {
            Debug.DrawRay(hitLeft.point, hitLeft.normal, Color.blue);
            slopeAngleHorizontal = Vector2.Angle(hitLeft.normal, Vector2.up) == 90 ? 0 : Vector2.Angle(hitLeft.normal, Vector2.up);
        }
    }

    void CheckWallCollision()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckerA.position, wallCheckRadius, wallLayer) || 
            Physics2D.OverlapCircle(wallCheckerB.position, wallCheckRadius, wallLayer);
        
        if (playerScript.animationScript.isFacingRight)
        {
            wallRay = Physics2D.Raycast(wallRaycastPoint.position, Vector2.right, wallRayLength, wallLayer);
        }
        else
        {
            wallRay = Physics2D.Raycast(wallRaycastPoint.position, Vector2.left, wallRayLength, wallLayer);
        }

        if (Physics2D.OverlapCircle(wallCheckerA.position, wallCheckRadius, wallLayer))
        {
            isFacingWall = true;
        }
        else if(Physics2D.OverlapCircle(wallCheckerB.position, wallCheckRadius, wallLayer))
        {
            isFacingWall = false;
        }
        
        if (wallRay.collider != null)
        {
            isNearWall = true;
        }
        else
        {
            isNearWall = false;
        }
    }

    void CheckLadderCollision()
    {
        isOnLadder = Physics2D.OverlapCircle(wallCheckerA.position, wallCheckRadius, ladderLayer) || 
            Physics2D.OverlapCircle(wallCheckerB.position, wallCheckRadius, ladderLayer);
        
        if (Physics2D.OverlapCircle(wallCheckerA.position, wallCheckRadius, ladderLayer))
        {
            isFacingLadder = true;
        }
        else if(Physics2D.OverlapCircle(wallCheckerB.position, wallCheckRadius, ladderLayer))
        {
            isFacingLadder = false;
        }

        if (playerScript.animationScript.isFacingRight)
        {
            wallRay = Physics2D.Raycast(wallRaycastPoint.position, Vector2.right, wallRayLength, ladderLayer);
        }
        else
        {
            wallRay = Physics2D.Raycast(wallRaycastPoint.position, Vector2.left, wallRayLength, ladderLayer);
        }

        if (isOnLadder && wallRay.collider == null)
        {
            isNearTop = true;
        }
        else
        {
            isNearTop = false;
        }
    }

    void CheckGrapplePoint()
    {
        grappleDirection = playerScript.inputScript.aimDirection.normalized == Vector2.zero?
            grappleDirection : playerScript.inputScript.aimDirection.normalized;
        grappleRay = Physics2D.Raycast(playerScript.grappleScript.firePoint.position, grappleDirection, grappleRayLength, grappleLayer);
    }

    private void OnDrawGizmos() {
        if (groundChecker == null) return;
        if (wallRaycastPoint == null) return;

        Gizmos.DrawWireSphere(groundChecker.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallCheckerA.position, wallCheckRadius);
        Gizmos.DrawWireSphere(wallCheckerB.position, wallCheckRadius);

        Vector2 fromG = groundChecker.position;
        Vector2 toG = new Vector2(groundChecker.position.x, groundChecker.position.y - groundRayLength);

        Gizmos.DrawLine(fromG, toG);

        Vector2 fromW = wallRaycastPoint.position;
        float add = playerScript.animationScript.isFacingRight? wallRayLength : -wallRayLength;
        Vector2 toW = new Vector2(wallRaycastPoint.position.x + add, wallRaycastPoint.position.y);

        Gizmos.DrawLine(fromW, toW);

        Vector2 A3 = playerScript.grappleScript.firePoint.position;
        Vector2 B3 = playerScript.grappleScript.firePoint.position + new Vector3(grappleDirection.x * grappleRayLength,
            grappleDirection.y * grappleRayLength, 0f);

        Gizmos.DrawLine(A3, B3);

        Gizmos.DrawLine((Vector2)groundCheckerPoint.position, new Vector2(groundCheckerPoint.position.x +  
            (playerScript.animationScript.isFacingRight? groundCheckerLength : - groundCheckerLength),
            groundCheckerPoint.position.y));
    }
}
