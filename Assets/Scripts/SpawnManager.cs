using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnManager : MonoBehaviour
{ 
    
    private static SpawnManager _instance;

    public static SpawnManager Instance 
    { 
        get { return _instance; } 
    } 
    public static Transform spawnTarget;
    public static Vector3 spawnPosition;
    private void Awake() 
    { 
        if (_instance != null && _instance != this) 
        { 
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    } 
}