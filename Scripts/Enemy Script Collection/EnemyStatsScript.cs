using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsScript : MonoBehaviour 
{
    [SerializeField] internal bool isFacingRight = true;
    [SerializeField] internal int health;
    [SerializeField] internal int meleeDamage;
    [SerializeField] internal float meleeDistance;
    [SerializeField] internal int rangedDamage;
    [SerializeField] internal float rangedDistance;
    [SerializeField] internal float projectileSpeed = 5f;
    private bool isInvincible = false;
    [SerializeField] private float invincibilityDuration;
    internal bool isAttacking = false;
    internal bool gettingHit = false;

    public void takeDamage(int damage)
    {
        if (isInvincible) return;

        health -= damage;
        health = Mathf.Max(0, health);
        StartCoroutine(MakeInvincible());
    }

    IEnumerator MakeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}