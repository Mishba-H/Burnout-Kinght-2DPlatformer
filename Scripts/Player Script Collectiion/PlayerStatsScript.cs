using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;
    internal bool canTakeDamage = true;
    [SerializeField] internal int maxHealth;
    internal int currentHealth;
    internal bool isInvincible = false;
    [SerializeField] private float invincibilityDuration;
    private float invincibilityCounter;
    internal bool playGetHit = false;
    [SerializeField] private float getHitDuration;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        SetInvincibility();
    }

    public void takeDamage(int damage)
    {
        if (!isInvincible && canTakeDamage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            isInvincible = true;
            invincibilityCounter = invincibilityDuration;
            StartCoroutine(PlayGetHitAnimation());
        }

        if (currentHealth == 0)
        {
            canTakeDamage = false;
            playerScript.spawnerScript.RespawnPlayer();
        }

        Debug.Log(currentHealth);
    }

    IEnumerator PlayGetHitAnimation()
    {
        playGetHit = true;
        yield return new WaitForSeconds(getHitDuration);
        playGetHit = false;
    }

    void SetInvincibility()
    {
        if(invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.fixedDeltaTime;
        }
        else
        {
            isInvincible = false;
        }
        if (currentHealth > 0)
            canTakeDamage = true;
    }
}
