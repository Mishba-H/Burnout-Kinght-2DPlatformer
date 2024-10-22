using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField] internal Transform startCheckpoint;
    private GameObject player;

    void Awake()
    {   
        player = GameObject.FindWithTag("Player");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else 
        {
            Destroy(instance);
        }
    }
}
