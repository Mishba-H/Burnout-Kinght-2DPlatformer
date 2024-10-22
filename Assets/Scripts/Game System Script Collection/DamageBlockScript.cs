using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlockScript : MonoBehaviour
{

    [SerializeField] private int damage = 25;

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
            if (playerScript.statsScript.canTakeDamage && !playerScript.statsScript.isInvincible)
                playerScript.statsScript.takeDamage(damage);
        }
    }
}