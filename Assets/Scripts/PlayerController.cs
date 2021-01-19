using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    public int groundID = 8;
    public float runSpeed = 4.0f;   //Running speed
    public float jumpStrength = 2.0f;   //Jumping strength
    private Rigidbody2D actorBody;  //Rigidbody for Physix
    public bool grounded;   //Are we touching ground?
    public float castDistance = 0.0f;   //Distance from center to ground
    public float gScale = 2.5f;     //Gravity scale, gets passed to rigidbody
    public bool onClimb = false;    //Are we climbing?
    public bool touchClimbable = false; //Are we touching a ladder like a filthy degenerate?
    public Transform nearestClimb;  //Transform position for the nearest ladder we're touching.
    //public Vector3 lastPoint; //No longer necessary, I keep it around because it has abandonment issues and might get lonely.
    private CapsuleCollider2D cap;  //Capsule collider, since we need to turn that shit on and off.
    private Vector3 respawnVel = Vector3.zero;  //Velocity initator for respawn code
    public float respawnSpeed = 0.5f;   //Dampening speed
    public bool gotten; //Did we just eat shit and get hit by a monster?
    private Vector2 leftStick;  //Left stick/D-Pad on the controller
    public Vector2 dumStick;    //Dummy stick that other things control
    private bool button1;   //The jump button
    public bool shifting;   //Are we switching between rooms?
    private bool jumping;   //Did the jump button just get pressed?
    
    void Start()
    {
        actorBody = GetComponent<Rigidbody2D>(); //Need rigidbody for physix to work
        cap = GetComponent<CapsuleCollider2D>();    //get the collider so we can turn it on and off
        SpawnManager.spawnPosition = transform.position;    //Set the spawn point
    }
    
    void Update()
    {
        switch (shifting)
        {   
            case false: //The stick should be read if we aren't shifting
            leftStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            button1 = Input.GetButtonDown("Jump");
            jumping = button1;  //This line feels somewhat redundant but I guess it is kind of important, considering other things change it.
            break;
            case true:  //Otherwise read the dummy stick and cancel out the button.
            leftStick = dumStick;
            button1 = false;
            break;
        }
    }
    
    
    void FixedUpdate()
    {
        GetInput(leftStick.x,leftStick.y,button1);  //Do the movement code
        grounded = GroundCast();
        if (gotten)     //If we've been hit we need to send the player hurtling back to the last spawn location
        {
            
            transform.position = Vector3.SmoothDamp(transform.position, SpawnManager.spawnPosition, ref respawnVel, respawnSpeed);
        
            if (Vector3.Distance(transform.position, SpawnManager.spawnPosition) <= 1.0f) //If we're close enough, turn "respawn" mode shit off.
            {
                gotten = false;
                cap.enabled = true;
                actorBody.gravityScale = gScale;
            }
        }
        
    }
    public void GetInput(float x_vec,float y_vec,bool hop)
    {
        if (!gotten)    //This code should ONLY run if we're "alive"
        {
            switch (touchClimbable)
            {
                case true:
                if (y_vec > 0.5f)   //If we're touching a ladder and press up, we wanna stick to the ladder. 
                {
                    onClimb = true;
                    actorBody.gravityScale = 0.0f;
                    grounded = false;
                    if (nearestClimb != null)   //Make sure that there hasn't been a mistake or anything
                    {
                        Vector2 tempTransform = new Vector2(nearestClimb.position.x, transform.position.y);
                        transform.position = tempTransform; //Snap the player to the ladder's X position.
                    }
                }
                break;
            }
            
            //Set up movement vectors depending on if we're on a ladder or not.
            switch (onClimb)    
            {
                case true:
                Vector2 ladVector = new Vector2(actorBody.velocity.x, (y_vec*runSpeed));    //Ladder mode only cares about vertical movement
                actorBody.velocity = ladVector;
                if (Mathf.Abs(x_vec) > 0.5f)        //Any movement on the horizontal axis will end Ladder mode
                {
                    onClimb = false;
                    actorBody.gravityScale = gScale;
                }
                break;
                case false:
                if (grounded && jumping)
                {
                    actorBody.velocity += ((jumpStrength) * Vector2.up);
                    grounded = false;
                    jumping = false;
                }
                Vector2 movVector = new Vector2((x_vec*runSpeed), actorBody.velocity.y);   //Not-Ladder mode only cares about horizontal movement
                actorBody.velocity = movVector;    //Apply the movement vector to the rigidbody, making it move along the X axis.
                break;
            }
        }
    }
    
    public bool GroundCast()
    {
        groundLayer = 1 << groundID;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, castDistance, groundLayer); //Raycast down from origin at a specific distance.
        if (hit)
        {
            return true;
        }
        return false; 
    }
    
    //Commented this out to see if I putting the code directly there would work. It did not. Don't know what I'm gonna do with this now.
    /*void TryJump()
    {
        if (grounded && jumping)
        {
            actorBody.velocity += ((jumpStrength) * Vector2.up);
            grounded = false;
            jumping = false;
        }
    }*/
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Monster") && !gotten)
        {
            cap.enabled = false;                //Gotta turn off the collider and gravity so that the player can FLY back to spawn.
            actorBody.gravityScale = 0;         
            gotten = true;                      
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Climbable"))   //This is for ladders so that we know what to snap to when we start climbing
        {
            touchClimbable = true;      
            nearestClimb = other.gameObject.GetComponent<Transform>();
        }
    }
    void OnTriggerExit2D(Collider2D other)      //This is so that we can turn off climbing mode when we get to the top of a ladder.
    {
        if (other.gameObject.CompareTag("Climbable"))
        {
            touchClimbable = false;
            if (onClimb)
            {
                onClimb = false;
                actorBody.gravityScale = gScale;
            }
        }
    }
}