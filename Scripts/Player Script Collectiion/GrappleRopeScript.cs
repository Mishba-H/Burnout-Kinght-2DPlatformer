using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRopeScript : MonoBehaviour
{
    [Header("General References:")]
    [SerializeField] PlayerGrappleScript grappleScript;
    [SerializeField] private LineRenderer lr;
    [Header("General Settings:")]
    [SerializeField] private int ropePoints = 40;
    [SerializeField] [Range(0, 20)] private float straightenLineSpeed = 5f;
    [Header("Rope Animation Settings:")]
    [SerializeField] private AnimationCurve ropeAnimationCurve;
    [SerializeField] [Range(0.01f, 4)] private float startWaveSize = 2;
    private float waveSize = 0;

    [Header("Rope Progression:")]
    [SerializeField] private AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;
    private float moveTime = 0f;
    private bool straightenLine = false;

    private void OnEnable()
    {
        moveTime = 0f;
        lr.positionCount = ropePoints;
        waveSize = startWaveSize;
        straightenLine = false;

        LinePointsToFirePoints();
        lr.enabled = true;
    }

    private void OnDisable()
    {
        lr.enabled = false;
    }

    private void LinePointsToFirePoints()
    {
        for (int i = 0; i < ropePoints; i++)
        {
            lr.SetPosition(i, grappleScript.firePoint.position);
        }
    }

    private void FixedUpdate() 
    {
        moveTime += Time.fixedDeltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (!straightenLine)
        {
            if (Vector2.Distance((Vector2)lr.GetPosition(ropePoints - 1),  grappleScript.grapplePoint) < 0.3f)
            {
                straightenLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (waveSize > 0)
            {
                waveSize -= Time.fixedDeltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;
                if (lr.positionCount != 2) {
                    lr.positionCount = 2;
                }
                DrawRopeNoWaves();
            }
        }
    }
    void DrawRopeWaves()
    {
        for (int i = 0; i < ropePoints; i++)
        {
            float delta = (float)i/ ((float)ropePoints - 1.0f);
            Vector2 offset = Vector2.Perpendicular(grappleScript.grappleDirectionVector).normalized * 
                ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp((Vector2)grappleScript.firePoint.position,
                grappleScript.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp((Vector2)grappleScript.firePoint.position, targetPosition, 
                ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            lr.SetPosition(i, currentPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        lr.SetPosition(0, grappleScript.firePoint.position);
        lr.SetPosition(1, grappleScript.grapplePoint);
    }
}