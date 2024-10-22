using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{   
    Animator anim;
    internal Rigidbody2D rb;
    internal int damage;
    [SerializeField] private bool affectedByGravity;
    [SerializeField] private float gravityFactor;
    [SerializeField] private float explosionDuration;
    internal Vector2 direction;
    private bool exploding = false;
    private State currentState;
    private enum State{
        Move,
        Explosion
    };

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (affectedByGravity) rb.gravityScale = gravityFactor;
        else rb.gravityScale = 0f; 
    }

    void Update()
    {
        PlayAnimations();
    }

    void PlayAnimations()
    {
        if (exploding)
            ChangeAnimationState(State.Explosion);
        else
            ChangeAnimationState(State.Move);
    }

    void ChangeAnimationState(State newState)
    {
        if (newState == currentState) return;

        if (anim == null) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyStatsScript>().takeDamage(damage);
        }

        if (!other.CompareTag("Player") && !other.CompareTag("Event") && !other.CompareTag("Turret") && !other.CompareTag("Projectile"))
        {
            StartCoroutine(ExplosionCoroutine());
        }
    }

    IEnumerator ExplosionCoroutine()
    {
        exploding = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }
}