using System;
using System.Collections;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    GameManager gameManager;
    Animator anim;
    internal State currentState;
    internal bool isActive = false;


    internal enum State{
        Inactive,
        Activate,
        Active
    };

    private void Awake() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        ChangeAnimationState(State.Inactive);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
            playerScript.spawnerScript.currentSpawnPoint = transform;
        }
    }

    internal void ChangeAnimationState(State newState)
    {
        if (newState == currentState) return;

        if (anim == null) return;

        anim.Play(newState.ToString());
        currentState = newState;
    }
}