using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour
{
    internal Animator anim;
    internal EnemyStatsScript statsScript;
    internal GroundMoveScript groundMoveScript;
    internal DetectPlayerScript detectPlayerScript;
    private State currentState;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float attackInterval;
    private float attackIntervalCounter;
    [SerializeField] private float attack1Duration = 1.33f;
    [SerializeField] private float attack2Duration = 1.33f;
    [SerializeField] private float attack3Duration = 1.33f;
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private float getHitDuration;
    [SerializeField] private float deathDuration;
    private int lastHealth;

    private enum State{
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        GetHit,
        Death
    };

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        statsScript = GetComponent<EnemyStatsScript>();
        groundMoveScript = GetComponent<GroundMoveScript>();   
        detectPlayerScript = GetComponent<DetectPlayerScript>();
        lastHealth = statsScript.health;
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
            ChangeAnimationState(State.Run);
        }
    }

    void AttackBehaviour()
    {
        var playerPosition = GameObject.FindWithTag("Player").transform.position;
        float distanceFromPlayer = (playerPosition - transform.position).magnitude;
        if (attackIntervalCounter > 0f)
        {
            attackIntervalCounter -= Time.fixedDeltaTime;
        }
        else if (!statsScript.isAttacking)
        {
            if (distanceFromPlayer >= statsScript.rangedDistance)
            {
                StartCoroutine(PlayAttack3Animation());
            }
            else if (distanceFromPlayer < statsScript.meleeDistance)
            {
                var attack = Random.Range(0, 2);
                if (attack == 0)
                {
                    StartCoroutine(PlayAttack1Animation());
                }
                else
                {
                    StartCoroutine(PlayAttack2Animation());
                }
            }
        }
    }

    IEnumerator PlayAttack1Animation()
    {
        statsScript.isAttacking = true;
        ChangeAnimationState(State.Attack1);
        yield return new WaitForSeconds(attack1Duration);
        statsScript.isAttacking = false;
        attackIntervalCounter = attackInterval;
    }
    IEnumerator PlayAttack2Animation()
    {
        statsScript.isAttacking = true;
        ChangeAnimationState(State.Attack2);
        yield return new WaitForSeconds(attack2Duration);
        statsScript.isAttacking = false;
        attackIntervalCounter = attackInterval;
    }
    IEnumerator PlayAttack3Animation()
    {
        statsScript.isAttacking = true;
        ChangeAnimationState(State.Attack3);
        yield return new WaitForSeconds(attack3Duration);
        statsScript.isAttacking = false;
        attackIntervalCounter = attackInterval;
    }

    void MeleeAttack()
    {
        RaycastHit2D hit = Physics2D.CircleCast(hitPoint.position, 0.5f, Vector2.right, defaultLayer);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerScript playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
                if (playerScript.statsScript.canTakeDamage && !playerScript.statsScript.isInvincible)
                    playerScript.statsScript.takeDamage(statsScript.meleeDamage);
            }
        }
    }

    void RangedAttack()
    {
        Vector2 direction;
        if (statsScript.isFacingRight)
            direction = new Vector2(1, 1).normalized;
        else 
            direction = new Vector2(-1, 1).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().rb.velocity = statsScript.projectileSpeed * direction;
        projectile.GetComponent<ProjectileScript>().damage = statsScript.rangedDamage;
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
