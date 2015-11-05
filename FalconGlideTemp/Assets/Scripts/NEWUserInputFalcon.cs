using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NEWUserInputFalcon : MonoBehaviour {

    /// <summary>
    /// list of other falcon colliders to disable collisions between them
    /// </summary>
    public List<Collider2D> otherFalconColliders;
    /// <summary>
    /// the gravity added when gliding
    /// </summary>
    public float gravity = -4;
    /// <summary>
    /// y velocity that is added every tick when diving
    /// </summary>
    public float divingYVel = -2;
    /// <summary>
    /// x Drag that is added every tick when diving, and when gliding if you are moving faster than default
    /// </summary>
    public float xDrag = -.2f;
    /// <summary>
    /// the key that this falcon uses to dive
    /// </summary>
    public string keyForMovement = "down";
    /// <summary>
    /// multiplies every vector added to the falcon
    /// </summary>
    public float movementSpeed = 15;
    /// <summary>
    /// starting x speed, falcon slows down to this speed when gliding
    /// </summary>
    public float xSpeedDefault = 10;
    /// <summary>
    /// The velocity multiplier when you are ascending. Increase to have falcon rise more after dive.
    /// </summary>
    public float ascendVelMult = 4.7f;
    /// <summary>
    /// how long the upward force is applied when you are in ascend state
    /// </summary>
    float timeInAscendMult = 1f;


    //MODES:
    //-rapidly tapping 'down' should only very marginally increase your x speed
    //-small swoops and large swoops have the same/a similar effect on your x velocity and y position in the end

    //FALCON STATES:
    //GLIDE - constant gravity
    //DIVE - x speed slows, y speed increases downward
    //ASCEND INIT - x boost based on dive time, 
    //y boost decays to 0
    //ASCEND - length based on dive speed, 
    //y increases upwards (y force decays over time), 
    //x speed decreases over time but larger than pre dive speed
    //at the end of the boost you are slightly lower than you started
    public enum falconStates
    {
        diving,
        release,
        ascending,
        gliding
    }
    falconStates curState = falconStates.gliding;
    public falconStates CurState
    {
        get
        {
            return curState;
        }
        set
        {
            curState = value;
        }
    }

    bool downKeyDown = false;
    bool downKeyPressed = false;
    bool downKeyReleased = false;
    float yVelEndDive = 0;
    float timeInAscend = 0f;
    GameObject debugLine;
    Rigidbody2D rb;

    // add initial x force
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(xSpeedDefault, rb.velocity.y);
        debugLine = (GameObject)Instantiate(Resources.Load("preDebugLine"), new Vector3(-1000, -1000, 0), Quaternion.identity);
        for (int i = 0; i < otherFalconColliders.Count; i++)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), otherFalconColliders[i]);
        }
    }

    void Update()
    {
        downKeyDown = Input.GetKeyDown(keyForMovement);
        downKeyPressed = Input.GetKey(keyForMovement);
        downKeyReleased = Input.GetKeyUp(keyForMovement);
        //if you pressed down, record the y position
        if (downKeyDown)
        {
            //DisplayDebugLine();
        }
        //if youre holding down, youre diving. 
        if (downKeyPressed)
        {
            CurState = falconStates.diving;
        }
        //If you released, you are in release
        else if (downKeyReleased)
        {
            CurState = falconStates.release;
        }
        switch (CurState)
        {
            case falconStates.diving:
                DivingControl();
                break;
            case falconStates.release:
                ReleaseControl();
                break;
            case falconStates.ascending:
                AscendingControl();
                break;
            case falconStates.gliding:
                GlidingControl();
                break;
        }
        ApplyXDrag();
    }

    /// <summary>
    /// when diving, apply x drag
    /// also apply less x drag to slow yourself down to default speed when gliding
    /// </summary>
    void ApplyXDrag()
    {
        switch (CurState)
        {
            case falconStates.diving:
                rb.AddForce(new Vector2(xDrag, 0) * movementSpeed, ForceMode2D.Force);
                break;
            case falconStates.release:
                break;
            case falconStates.ascending:
                break;
            case falconStates.gliding:
                if (rb.velocity.x > xSpeedDefault)
                    rb.AddForce(new Vector2(xDrag / 2, 0) * movementSpeed, ForceMode2D.Force);
                break;
        }
    }

    /// <summary>
    /// Diving Control
    /// add downward force and x drag
    /// </summary>
    void DivingControl()
    {
        rb.AddForce(new Vector2(0, divingYVel) * movementSpeed, ForceMode2D.Force);
    }

    /// <summary>
    /// Release control
    /// track the y velocity you're at, and give yourself an x boost based on that velocity
    /// go to ascending state and set the amount of time you'll be there
    /// </summary>
    void ReleaseControl()
    {
        yVelEndDive = rb.velocity.y;
        rb.AddForce(new Vector2(-rb.velocity.y, 0) * movementSpeed, ForceMode2D.Force);
        timeInAscend = timeInAscendMult;
        CurState = falconStates.ascending;
    }

    /// <summary>
    /// Ascending Control
    /// add force equal to a multiple of yVelEndDive (the velocity you were at when you released) to your y
    /// if you run out of time, go back to gliding
    /// </summary>
    void AscendingControl()
    { 
        timeInAscend -= Time.deltaTime;
        rb.AddForce(new Vector2(0, - yVelEndDive) * movementSpeed * ascendVelMult * Time.deltaTime, ForceMode2D.Force);
        if (timeInAscend <= 0)
        {
            timeInAscend = 0;
            CurState = falconStates.gliding;
        }
    }
 
    /// <summary>
    /// Gliding Control
    /// constantly apply gravity
    /// </summary>
    void GlidingControl()
    {
        rb.AddForce(new Vector2(0, gravity), ForceMode2D.Force);
    }


    //displays the debug line for testing
    void DisplayDebugLine()
    {
        debugLine.transform.position = transform.position;
    }
}
