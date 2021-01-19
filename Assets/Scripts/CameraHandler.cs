using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public bool shifting = false;
    public Transform dampTarget;
    public Transform initialTarget;
    public float dampTime = 2.0f;
    private Vector3 velocity = Vector3.zero;
    public GameObject playActor;
    private PlayerController actorCheck;
    private InputSystem controls;
    public bool right = false;
    public bool vert = false;
    public bool gotten = false;

    void Start()
    {
        dampTarget = dampTarget.GetComponent<Transform>();
        SpawnManager.spawnTarget = dampTarget;
        //initialTarget = initialTarget.GetComponent<Transform>();
        if (playActor==null)
        {
            playActor = GameObject.FindWithTag("Player");
        }
        controls = playActor.GetComponent<InputSystem>();
        actorCheck = playActor.GetComponent<PlayerController>();
    }
    void Update()
    {
        if (shifting && !gotten)
        {
            actorCheck.shifting = true;
            if (right)
            {
                switch (vert)
                {
                    case true:
                    actorCheck.dumStick.x = 0.0f;
                    actorCheck.dumStick.y = 1.0f;
                    break;
                    case false:
                    actorCheck.dumStick.x = 1.0f;
                    actorCheck.dumStick.y = 0.0f;
                    break;
                }
                
            }
            else
            {
                switch (vert)
                {
                    case true:
                    actorCheck.dumStick.x = 0.0f;
                    actorCheck.dumStick.y = -2.0f;
                    break;
                    case false:
                    actorCheck.dumStick.x = -1.0f;
                    actorCheck.dumStick.y = 0.0f;
                    break;
                }
            }
            DoShift();
        }
        else
        {
            if (actorCheck.gotten)
            {
                gotten = true;
                shifting = true;
                dampTarget = SpawnManager.spawnTarget;
                DoShift();
            }
            else
            {
                gotten = false;
            }
        }
    }


    public void DoShift()
    {
        transform.position = Vector3.SmoothDamp(transform.position, dampTarget.position, ref velocity, dampTime);
        Vector3 tempPos = transform.position; 
        tempPos.x = Mathf.Round(tempPos.x);
        tempPos.y = Mathf.Round(tempPos.y);
        tempPos.z = Mathf.Round(tempPos.z);
        if (tempPos == dampTarget.position)
        {
            transform.position = dampTarget.position;
            actorCheck.shifting = false;
            shifting = false;
        }
    }
}