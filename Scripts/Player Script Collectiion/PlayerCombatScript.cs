using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;
    [SerializeField] private LayerMask enemyLayer;
    internal bool isAttacking = false;
    [SerializeField] internal GameObject sword;
    [SerializeField] internal Transform hitPoint;
    [SerializeField] internal float attackRadius;
    [SerializeField] internal int meleeDamage = 15;
    [SerializeField] private float attackInterval;
    internal float attackIntervalCounter;
    [SerializeField] private float attackDuration;
    internal bool isShooting = false;
    [SerializeField] internal GameObject gun;
    [SerializeField] internal Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] internal int rangedDamage = 20;
    [SerializeField] internal float bulletSpeed = 8f;
    [SerializeField] private float shootInterval;
    internal float shootIntervalCounter;
    [SerializeField] private float shootDuration;

    void Awake()
    {
        sword.SetActive(false);
        gun.SetActive(false);
    }

    void FixedUpdate() 
    {
        if(attackIntervalCounter > 0f)
        {
            attackIntervalCounter -= Time.fixedDeltaTime;
        }
        else if (isAttacking)
        {
            playerScript.horizontalMovementScript.disableMove = true;
            playerScript.jumpScript.disableJump = true;
            playerScript.dashScript.disableDash = true;
            StartCoroutine(PlayAttackAnimation());
            attackIntervalCounter = attackInterval;
        }

        if(shootIntervalCounter > 0f)
        {
            shootIntervalCounter -= Time.fixedDeltaTime;
        }
        else if (isShooting)
        {
            playerScript.horizontalMovementScript.disableMove = true;
            playerScript.jumpScript.disableJump = true;
            playerScript.dashScript.disableDash = true;
            StartCoroutine(PlayShootAnimation());
            shootIntervalCounter = shootInterval;
        }
    }

    IEnumerator PlayAttackAnimation()
    {
        playerScript.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
        attackIntervalCounter = attackInterval;
    }

    void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyStatsScript>().takeDamage(meleeDamage);
        }
    }

    IEnumerator PlayShootAnimation()
    {
        playerScript.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(shootDuration);
        isShooting = false;
        shootIntervalCounter = shootInterval;
    }

    void RangedAttack()
    {
        var direction = playerScript.animationScript.isFacingRight? Vector2.right : Vector2.left;
        var rotation = playerScript.animationScript.isFacingRight? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        bullet.GetComponent<BulletScript>().rb.velocity = bulletSpeed * direction;
        bullet.GetComponent<BulletScript>().damage = rangedDamage;
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.DrawWireSphere(hitPoint.position, attackRadius);
    }
}
