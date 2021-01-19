using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private float moveHorz; //Left stick vector's horizontal axis
    private float moveVert; //Left stick vector's vertical axis
    private bool button1;   //Jump button
    private PlayerController mainActor; //Player actor
    public float getHorz;   //Canned left stick horizontal vector axis    
    public float getVert;   //Canned left stick vertical vector axis
    public bool getButton;  //Canned button
    public bool locked;          //This determines if we can use the controller to move or not.
    void Start()
    {
        mainActor = GetComponent<PlayerController>();
    }

    void Update()
    {
        switch(locked)
        {
            case true:
            moveHorz = getHorz;
            moveVert = getVert;
            button1 = getButton;
            mainActor.GetInput(moveHorz,moveVert,button1);
            break;

            case false:
            moveHorz = Input.GetAxis("Horizontal");
            moveVert = Input.GetAxis("Vertical");
            button1 = Input.GetButtonDown("Jump");
            mainActor.GetInput(moveHorz,moveVert,button1);
            break;
        }
    }

}