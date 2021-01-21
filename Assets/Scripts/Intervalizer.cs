using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intervalizer : MonoBehaviour
{
    //This will deactivate/reactivate all children after a set amount of seconds. (seconds * 60.0f) Then it will reset the timer and do it again until the heat death of the universe.
    public float seconds = 0.0f;    //This value gets multiplied by sixty 
    private float realTimer = 0.0f;  //...and gets placed into this.
    private bool activate = false; //If true, this means we need to deactivate, vice versa for false.

    void Start()
    {
        realTimer = seconds * 60.0f;    //Initialize the timer
    }
    void Update()
    {
        if (realTimer > 0.0f)
        {
            realTimer --;  //Decrement the timer if not zero
        }
        else
        {
            activate = !activate;   //Flag/unflag the boolean
            Intervalize();  //Activate/deactivate all children
            realTimer = seconds * 60.0f;    //Reset the timer
        }
    }
    void Intervalize()
    {
            foreach (Transform child in transform)
            {
                if (activate)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

    }
}