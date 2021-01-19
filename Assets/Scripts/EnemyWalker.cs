using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : MonoBehaviour
{
    public LayerMask groundLayer;
    public int groundID = 8;
    private Vector2 rightSensor;    //Right sensor
    private Vector2 leftSensor; //Left sensor
    private Vector2 mainSensor; //Main sensor
    public float moveSpeed = 2.0f;  //Movement speed
    public bool edgeDetect = true;  //Untick this to make the enemy walk off of ledges rather than turning around
    public bool grounded = true;    //Are we on the ground?
    public bool direction = false; //true = right, false = left
    private Rigidbody2D badActor;   //Rigidbody for Physix
    public float castDistance = 0.5f;   //Distance from sensor's center.
    void Start()
    {
        badActor = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        leftSensor = new Vector2(transform.position.x-0.5f,transform.position.y); //Left and right sensors are... as you'd expect, offset to the left and right by a bit.
        mainSensor = (Vector2)transform.position;
        rightSensor = new Vector2(transform.position.x+0.5f,transform.position.y);
        bool leftHit = GroundCast(leftSensor, castDistance+1.0f);   //Left and right casts always go down lower because we might be on a slope.
        grounded = GroundCast(mainSensor, castDistance);
        bool rightHit = GroundCast(rightSensor, castDistance+1.0f);
        Vector2 moveVec = new Vector2(moveSpeed, badActor.velocity.y);  //Movement vector, we don't really jump so eh, just horizontal.
        badActor.velocity = moveVec;   
        //Do the edge detection code if that option's been flagged.
        switch (edgeDetect)     
        {
            case true:
            switch (direction)
            {
                case false:
                EdgeCheck(leftHit);
                break;
                case true:
                EdgeCheck(rightHit);
                break;
            }
            
            break;
            case false:
            break;
        }
        WallCheck();    //Do the wall code check regardless.
    }

    //Turn around if we're about to go off a cliff.
    public void EdgeCheck(bool hit)
    {
        
            if (!hit && grounded)
            {
                moveSpeed *= -1.0f;
                direction = !direction;
            }
            
        
    }
    //Turn around if we're about to hit a wall.
    public void WallCheck()
    {
        bool wall = false;
        switch (direction)
        {
            case false:
            wall = WallCast(mainSensor,-castDistance);
            if (wall)
            {
                moveSpeed *=-1.0f;
                direction = true;
            }
            break;
            case true:
            wall = WallCast(mainSensor,castDistance);
            if (wall)
            {
                moveSpeed *=-1.0f;
                direction = false;
            }
            break;
        }

    }

    //Check for ground, this is slightly more complex than the player's since we need to account for multiple sensors.
    public bool GroundCast(Vector2 sensor, float castLength)
    {
        groundLayer = 1 << groundID;
        RaycastHit2D hit = Physics2D.Raycast(sensor, Vector2.down, castLength, groundLayer); //Raycast down from origin at a specific distance.
        if (hit)
        {
            return true;
        }
        return false; 
    }
    //Check for walls.
    public bool WallCast(Vector2 sensor, float castLength)
    {
        groundLayer = 1 << groundID;
        RaycastHit2D hit = Physics2D.Raycast(sensor, Vector2.left, castLength, groundLayer); //Raycast down from origin at a specific distance.
        if (hit)
        {
            return true;
        }
        return false; 
    }
}