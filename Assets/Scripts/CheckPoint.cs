using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform checkTrans;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SpawnManager.spawnPosition = other.transform.position;
            SpawnManager.spawnTarget = checkTrans;
        }
    }
}