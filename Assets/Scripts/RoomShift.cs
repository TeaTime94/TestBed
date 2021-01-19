using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomShift : MonoBehaviour
{
    public Transform dampTarget;    //Target for the camera to shift to when this is triggered
    public Camera targetCam;        //Aforementioned camera
    private CameraHandler dolly;    //Aforementioned camera's main script
    public bool vAxis = false;      //Flag this for a vertical shift, otherwise it'll always be horizontal.
    
    
    void Start()
    {
        dampTarget = dampTarget.GetComponent<Transform>();
        targetCam = targetCam.GetComponent<Camera>();
        dolly = targetCam.GetComponent<CameraHandler>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
       if (!dolly.shifting && other.gameObject.CompareTag("Player"))
       {
           //Determine direction in which the camera should go by guessing based on the relative position of the dampening target.
            switch (vAxis)
            {
                //if vertical
                case true:
                if (dampTarget.position.y > transform.position.y)
                {
                    dolly.vert = true;
                    dolly.right = true;
                }
                else
                {
                    dolly.vert = true;
                    dolly.right = false;
                }
                break;
                case false:
                if (dampTarget.position.x > transform.position.x)
                {
                    dolly.vert = false;
                    dolly.right = true;
                }
                else
                {
                    dolly.vert = false;
                    dolly.right = false;
                }
                break;
            }
            dolly.dampTarget = dampTarget;
            dolly.shifting = true;
            dolly.DoShift();
       }
        
    }
}