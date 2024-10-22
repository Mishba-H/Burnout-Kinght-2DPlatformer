using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWormScript : MonoBehaviour
{
    internal Animator anim;
    internal EnemyStatsScript statsScript;
    internal GroundMoveScript groundMoveScript;
    internal DetectPlayerScript detectPlayerScript;
    private State currentState;
    [SerializeField] private GameObject fireProjectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackInterval;
    private float attackIntervalCounter;
    [SerializeField] private float attackDuration = 1.33f;
    [SerializeField] private float getHitDuration;
    [SerializeField] private float deathDuration;
    private int lastHealth;

    private enum State
    {
        Idle,
        Walk, 
        Attack, 
        GetHit,
        Death
    }

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        statsScript = GetComponent<EnemyStatsScript>();
        groundMoveScript = GetComponent<GroundMoveScript>();   
        detectPlayerScript = GetComponent<DetectPlayerScript>(); 
    }

    private void FixedUpdate() 
    {
        if(detectPlayerScript.isAgro) AttackBehaviour();
        Flip();
        PlayStates();
    }

    void ChangeAnimationState(State newState)
    {
        if (newState == currentState) return;

        if (anim == null) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }

    void Flip()
    {
        if (statsScript.isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void PlayStates()
    {
        if (statsScript.health == 0)
        {
            PlayDeathAnimation();
            return;
        }
        if (statsScript.health != lastHealth)
        {
            lastHealth = statsScript.health;
            StartCoroutine(PlayGetHitAnimation());
        }

        if (statsScript.isAttacking) return;

        if (statsScript.gettingHit)
        {
            ChangeAnimationState(State.GetHit);
            return;
        }

        if(groundMoveScript.rb.velocity.x == 0f)
        {
            ChangeAnimationState(State.Idle);
        }
        else if(Mathf.Abs(groundMoveScript.rb.velocity.x) > 0)
        {
            ChangeAnimationState(State.Walk);
        }
    }

    void AttackBehaviour()
    {
        if (attackIntervalCounter > 0f)
        {
            attackIntervalCounter -= Time.fixedDeltaTime;
        }
        else if (!statsScript.isAttacking)
        {
            StartCoroutine(PlayAttackAnimation());
        }
    }

    IEnumerator PlayAttackAnimation()
    {
        statsScript.isAttacking = true;
        ChangeAnimationState(State.Attack);
        yield return new WaitForSeconds(attackDuration);
        statsScript.isAttacking = false;
        attackIntervalCounter = attackInterval;
    }

    // void ShootAtPlayer()
    // {
    //     var playerPosition = GameObject.FindWithTag("Player").transform.position;
    //     var playerDirection = (playerPosition - firePoint.position).normalized;
    //     float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;

    //     GameObject fireProjectile = Instantiate(fireProjectilePrefab, firePoint.position,
    //         Quaternion.AngleAxis(angle, Vector3.forward));
    //     fireProjectile.GetComponent<ProjectileScript>().rb.velocity = fireProjectile.GetComponent<ProjectileScript>().speed * 
    //         playerDirection.normalized;
    // }

    void ShootHorizontally()
    {
        var direction = statsScript.isFacingRight? Vector2.right : Vector2.left;
        var rotation = statsScript.isFacingRight? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        GameObject fireProjectile = Instantiate(fireProjectilePrefab, firePoint.position, rotation);
        fireProjectile.GetComponent<ProjectileScript>().rb.velocity = statsScript.projectileSpeed * direction;
        fireProjectile.GetComponent<ProjectileScript>().damage = statsScript.rangedDamage;
    }
    
    IEnumerator PlayGetHitAnimation()
    {
        statsScript.gettingHit = true;
        yield return new WaitForSeconds(getHitDuration);
        statsScript.gettingHit = false;
    }

    void PlayDeathAnimation()
    {
        ChangeAnimationState(State.Death);
        Destroy(gameObject, deathDuration);
    }
}
