using System.Collections;
using UnityEngine;

public class DetectPlayerScript : MonoBehaviour 
{
    EnemyStatsScript statsScript;
    internal Vector2 playerDirection;
    internal bool isAgro = false;
    [SerializeField] internal float detectRadius;
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] internal float followDistance = 10f;
    internal bool isDetecting = true;

    private void Awake() {
        statsScript = GetComponent<EnemyStatsScript>();
    }

    void Update() 
    {
        DetectPlayer();
        FollowPlayer();
    }

    void DetectPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectRadius, defaultLayer);
        if (collider == null) 
        {
            isDetecting = true;
            return;
        }
        
        if (collider.CompareTag("Player") && isDetecting)
        {
            isDetecting = false;
            isAgro = true;
        }
    }

    void FollowPlayer()
    {
        if (isAgro)
        {
            var playerPosition = GameObject.FindWithTag("Player").transform.position;
            var distanceFromPlayer = Vector3.Magnitude(playerPosition - transform.position);
            if (distanceFromPlayer > followDistance) isAgro = false;

            statsScript.isFacingRight = (playerPosition.x - transform.position.x) > 0? true : false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}