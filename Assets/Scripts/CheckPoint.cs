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
           
            if (other.GetComponent<PlayerController>().grounded)
            {
            Vector3 checkPosition = new Vector3(transform.position.x,other.transform.position.y, other.transform.position.z);
            SpawnManager.spawnPosition = checkPosition;
            SpawnManager.spawnTarget = checkTrans;
            }
        }
    }
}