using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnerScript : MonoBehaviour
{
    [SerializeField] PlayerScript playerScript;
    GameManager gameManager;
    internal bool spawnPlayer = true;
    internal Transform currentSpawnPoint;

    private void Start() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (currentSpawnPoint == null)
        {
            currentSpawnPoint = gameManager.startCheckpoint;
        }
        StartCoroutine(SpawnCoroutine());
    }

    internal void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        playerScript.statsScript.currentHealth = 0;
        transform.position = currentSpawnPoint.position + new Vector3(0f, 1.5f, 0f);
        spawnPlayer = true;
        yield return new WaitForSeconds(1.66f);
        playerScript.statsScript.currentHealth = playerScript.statsScript.maxHealth;
        spawnPlayer = false;
    }

    IEnumerator RespawnCoroutine()
    {
        playerScript.animationScript.ChangeAnimationState(playerScript.animationScript.death);
        yield return new WaitForSeconds(3f);
        transform.position = currentSpawnPoint.position + new Vector3(0f, 1.5f, 0f);
        spawnPlayer = true;
        yield return new WaitForSeconds(1.66f);
        playerScript.statsScript.currentHealth = playerScript.statsScript.maxHealth;
        spawnPlayer = false;
    }
}