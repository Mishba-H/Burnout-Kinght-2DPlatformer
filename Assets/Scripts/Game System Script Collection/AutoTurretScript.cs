using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurretScript : MonoBehaviour
{
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private float initialDelay;
    private float initialDelayCounter;
    [SerializeField] private float fireInterval;
    private float fireIntervalCounter;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int damage;
    [SerializeField] private float speed;

    private void Awake() 
    {
        initialDelayCounter = initialDelay;
        fireIntervalCounter = fireInterval;    
    }

    void FixedUpdate()
    {
        ShootProjectile();
    }

    void ShootProjectile()
    {
        if (initialDelayCounter > 0f)
        {
            initialDelayCounter -= Time.fixedDeltaTime;
        }
        else
        {
            if (fireIntervalCounter > 0)
            {
                fireIntervalCounter -= Time.fixedDeltaTime;
            }
            else
            {
                ShootHorizontally();
                fireIntervalCounter = fireInterval;
            }
        }
    }

    void ShootHorizontally()
    {
        var direction = isFacingRight? Vector2.right : Vector2.left;
        var rotation = isFacingRight? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
        projectile.GetComponent<ProjectileScript>().rb.velocity = speed * direction;
        projectile.GetComponent<ProjectileScript>().damage = damage;
    }
}
