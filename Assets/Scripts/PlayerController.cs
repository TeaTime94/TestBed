using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    public int groundID = 8;
    public float rotSpeed = 0.125f;       //Speed at which POKE's model will rotate.
    public float castDistance = 0.0f;   //Distance from center to ground
    public float gScale = 2.5f;     //Gravity scale, gets passed to rigidbody
    public float runSpeed = 4.0f;   //Running speed
    public float jumpStrength = 2.0f;   //Jumping strength
    public float respawnSpeed = 0.5f;   //Dampening speed 
    public bool onClimb = false;    //Are we climbing?
    public bool touchClimbable = false; //Are we touching a ladder like a filthy degenerate?
    public bool gotten; //Did we just eat shit and get hit by a monster?
    public bool shifting;   //Are we switching between rooms?
    public bool jumping;   //Did the jump button just get pressed?
    public bool grounded;   //Are we touching ground?
    public Vector2 dumStick;    //Dummy stick that other things control
    public Transform nearestClimb;  //Transform position for the nearest ladder we're touching.
    public Transform myModel;
    //public Vector3 lastPoint; //No longer necessary, I keep it around because it has abandonment issues and might get lonely.
    private CapsuleCollider2D cap;  //Capsule collider, since we need to turn that shit on and off.
    private Vector3 respawnVel = Vector3.zero;  //Velocity initator for respawn code
    private Vector2 leftStick;  //Left stick/D-Pad on the controller
    private Rigidbody2D actorBody;  //Rigidbody for Physix
    public Transform rRot;
    public Transform lRot;
    private float timeCount = 0.0f;
    public bool facing = true; //Flag this as true to face right.

    void Start()
    {
        actorBody = GetComponent<Rigidbody2D>(); //Need rigidbody for physix to work
        cap = GetComponent<CapsuleCollider2D>();    //get the collider so we can turn it on and off
        SpawnManager.spawnPosition = transform.position;    //Set the spawn point
        myModel = myModel.GetComponent<Transform>();
        rRot = rRot.GetComponent<Transform>();
        lRot = lRot.GetComponent<Transform>();
    }
    
    void Update()
    {
        
        if (shifting)
        {  
            leftStick = dumStick;
        }
        else
        {
            leftStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            
            //jumping = Input.GetButtonDown("Jump");
            if (Input.GetButtonDown("Jump") && grounded)
            {
                jumping = true;
                //Debug.Log("I pressed the jump button.");
            }
            //jumping = button1;  //This line feels somewhat redundant but I guess it is kind of important, considering other things change it.
        }
            if (leftStick.x > 0)
            {
                facing = true;
            }
            else if (leftStick.x < 0)
            {
                facing = false;
            }
            //Do the rotation here
            if (onClimb)
            {
                Quaternion rotVec = Quaternion.Euler(0,0,0);
                myModel.rotation = rotVec;
            }
            else
            {
                if (facing)
                {
                    if (myModel.rotation.y != rRot.rotation.y)
                    {
                        myModel.rotation = Quaternion.Slerp(myModel.rotation, rRot.rotation, timeCount);
                        timeCount = (timeCount) + Time.deltaTime*rotSpeed;
                    }
                    else
                    {
                        myModel.rotation = rRot.rotation;
                        timeCount = 0;
                    }
                }
                else if (!facing)
                {
                    if (myModel.rotation.y != lRot.rotation.y)
                    {
                    myModel.rotation = Quaternion.Slerp(myModel.rotation, lRot.rotation, timeCount);
                    timeCount = (timeCount) + Time.deltaTime*rotSpeed;
                    }
                    else
                    {
                        myModel.rotation = lRot.rotation;
                        timeCount = 0;
                    }
                }
            }

        if (touchClimbable)
            {
                if (leftStick.y > 0.5f)   //If we're touching a ladder and press up, we wanna stick to the ladder. 
                {
                    onClimb = true;
                    actorBody.gravityScale = 0.0f;
                    grounded = false;
                    if (nearestClimb != null)   //Make sure that there hasn't been a mistake or anything
                    {
                        Vector3 tempTransform = new Vector3(nearestClimb.position.x, transform.position.y, transform.position.z);
                        transform.position = tempTransform; //Snap the player to the ladder's X position.
                    }
                }
            }
        if (onClimb)
        {
            if ((Mathf.Abs(leftStick.x) > 0.5f)||(leftStick.y < -0.5f && grounded))        //Any movement on the horizontal axis will end Ladder mode
                {
                    onClimb = false;
                    actorBody.gravityScale = gScale;
                }
        }
        if (gotten)
        {
            RespawnMove();
        }
    }
    


    void FixedUpdate()
    {
        if (actorBody.velocity.y >= 0.0f)
        {
            grounded = GroundCast();
        }
        if (!gotten)     //If we've been hit we need to send the player hurtling back to the last spawn location
        {
            GetInput(leftStick.x,leftStick.y);  //Do the movement code
        }
        
    }
    void LateUpdate()
    {
        Debug.DrawRay(transform.position, Vector2.down*castDistance, Color.green);
    }


    public void GetInput(float x_vec,float y_vec)
    {
            
            
            //Set up movement vectors depending on if we're on a ladder or not.
            if (onClimb)    
            {
                Vector2 ladVector = new Vector2(0.0f, (y_vec*(runSpeed/1.5f)));    //Ladder mode only cares about vertical movement
                actorBody.velocity = ladVector;
                
            }
            else
            {
                if (grounded && jumping)
                {
                    DoJump();
                }
                Vector2 movVector = new Vector2((x_vec*runSpeed), actorBody.velocity.y);   //Not-Ladder mode only cares about horizontal movement
                actorBody.velocity = movVector;    //Apply the movement vector to the rigidbody, making it move along the X axis.
            }
        
    }
    
    public bool GroundCast()
    {
        groundLayer = 1 << groundID;
        Vector2 tempPosition = new Vector2 (transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(tempPosition, Vector2.down, castDistance, groundLayer); //Raycast down from origin at a specific distance.
        if (hit)
        {
            return true;
        }
        return false; 
    }
    
    //Commented this out to see if I putting the code directly there would work. It did not. Don't know what I'm gonna do with this now.
    void DoJump()
    {
        
            //Debug.Log("I actually jumped.");
            actorBody.velocity += ((jumpStrength) * Vector2.up);
            grounded = false;
            jumping = false;
        
    }
    void RespawnMove()
    {
        transform.position = Vector3.SmoothDamp(transform.position, SpawnManager.spawnPosition, ref respawnVel, respawnSpeed);
        
            if (Vector3.Distance(transform.position, SpawnManager.spawnPosition) <= 0.5f) //If we're close enough, turn "respawn" mode shit off.
            {
                transform.position = SpawnManager.spawnPosition;
                gotten = false;
                cap.enabled = true;
                actorBody.gravityScale = gScale;
            }

    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Monster") || other.gameObject.CompareTag("Harmful") )&& !gotten)
        {
            Vector3 newVel = new Vector3 (0.0f,0.0f,0.0f);
            actorBody.velocity = newVel;
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