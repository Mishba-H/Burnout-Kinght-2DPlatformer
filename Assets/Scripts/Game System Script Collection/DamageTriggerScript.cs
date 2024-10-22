using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTriggerScript : MonoBehaviour
{
    PlayerScript playerScript;

    [SerializeField] private int damage = 25;

    void Awake()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerScript.statsScript.takeDamage(damage);
        }
    }
}
