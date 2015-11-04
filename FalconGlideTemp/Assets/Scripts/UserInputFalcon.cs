using UnityEngine;
using System.Collections;

public class UserInputFalcon : MonoBehaviour {

    public float gravity = -4;
    public float divingYVel = -2;
    public float xDrag = -.2f;
    public string keyForMovement = "down";
    public float maxAngle = 60;
    public float downRotationSpeed = 3;
    public float upRotationSpeed = 2;
    public float initUpRotationSpeed = 4;
    public float movementSpeed = 15;
    public float maxSpeedX = 20;
    public float minSpeedX = 5;
    public float xSpeedDefault = 10;
    public float maxSpeedY = 100;
    public float ascendInitVerticalMultiplier = 4f;
    public float ascendInitAngle = 30;
    public float ascendAngleMax = 60;
    public float yAscendMult = -.9f;
    public float angleMult = 2f;
    public float timeInInitAscend = .4f;
    public float releaseBoostMultiplier = 1f;


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
    public falconStates CurState {
        get {
            return curState;
        }
        set {
            if (value == falconStates.ascending && curState != falconStates.ascending)
            {
                ascendState = ascendingStates.ascendingInit;
            }
            curState = value;
        }
    }

    public enum ascendingStates
    {
        ascendingInit,
        ascending
    }
    ascendingStates ascendState = ascendingStates.ascendingInit;

    bool downKeyDown = false;
    bool downKeyPressed = false;
    bool downKeyReleased = false;
    float ascendAngle = 0;
    float yPosDiveStart = 0;
    float yPosDiveEnd = 0;
    float yOfMaxRise = 0;
    GameObject debugLine;
    //float maxMagnitude = 0;
    Rigidbody2D rb;

    // add initial x force
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(xSpeedDefault, 0));
        debugLine = (GameObject)Instantiate(Resources.Load("preDebugLine"), new Vector3(-1000, -1000, 0), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update ()
    {
        downKeyDown = Input.GetKeyDown(keyForMovement);
        downKeyPressed = Input.GetKey(keyForMovement);
        downKeyReleased = Input.GetKeyUp(keyForMovement);
	//if you pressed down, record the y position
        if (downKeyDown)
        {
            yPosDiveStart = transform.position.y;
            DisplayDebugLine();
        }
	//if youre holding down, youre diving. If you released, you are in release
        if (downKeyPressed)
        {
            CurState = falconStates.diving;
        }
        else if (downKeyReleased)
        {
            CurState = falconStates.release;
        }
        switch(CurState)
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
	//Control max x, min x, max y velocities
        if (rb.velocity.x > maxSpeedX)
        {
            rb.velocity = new Vector2(maxSpeedX, rb.velocity.y);
        }
        if (rb.velocity.x < minSpeedX)
        {
            rb.velocity = new Vector2(minSpeedX, rb.velocity.y);
        }
        if (rb.velocity.x > xSpeedDefault)
        {
            rb.AddForce(new Vector2(xDrag, 0));
        }
        if (rb.velocity.y > maxSpeedY)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxSpeedY);
        }
    }

    //slerp the angle downwards, and add force to your transform.right
    void DivingControl()
    {
        rb.AddForce(new Vector2(xDrag, divingYVel) * movementSpeed, ForceMode2D.Force);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -maxAngle), Time.deltaTime * downRotationSpeed);
    }

    //track the max ascending angle, track the y position at which you should stop ascending, start the ascend init timer
    void ReleaseControl()
    {
        rb.AddForce(new Vector2(-rb.velocity.y, 0) * movementSpeed * releaseBoostMultiplier, ForceMode2D.Force);
        ascendAngle = (yPosDiveStart - transform.position.y) * angleMult;
        if (ascendAngle < ascendInitAngle)
        {
            ascendAngle = ascendInitAngle;
        }
        if (ascendAngle > ascendAngleMax)
        {
            ascendAngle = ascendAngleMax;
        }
        yPosDiveEnd = transform.position.y;
        yOfMaxRise = yPosDiveStart + (yPosDiveStart - yPosDiveEnd) * yAscendMult;
        CurState = falconStates.ascending;
        StartCoroutine(initAscendTimer());
    }

    //add force of your transform.right to ONLY your y, slerp the angle based on state in ascending, if you're higher than maxRise, go back to gliding
    void AscendingControl()
    {
        float rotationSpeed = 0;
        float angle = 0;
        switch(ascendState)
        {
            case ascendingStates.ascendingInit:
                rotationSpeed = initUpRotationSpeed;
                angle = ascendInitAngle;
                if (rb.velocity.y < 0)
                {
                    rb.AddForce(new Vector2(0, ascendInitVerticalMultiplier * rb.velocity.y * -1));
                }
                break;
            case ascendingStates.ascending:
                rotationSpeed = upRotationSpeed;
                angle = ascendAngle;
                float yRatio = (yPosDiveStart - transform.position.y) / (yPosDiveStart - yPosDiveEnd);
                yRatio = yRatio / 2;
                rb.AddForce(new Vector2(0, yRatio) * movementSpeed, ForceMode2D.Force);
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        if (transform.position.y > yOfMaxRise)
        {
            CurState = falconStates.gliding;
        }
    }

    //constantly apply gravity and slerp to 0 rotation
    void GlidingControl()
    {
        rb.AddForce(new Vector2(0, gravity), ForceMode2D.Force);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * downRotationSpeed);
    }

    //wait for a certain amount of time in initAscend, then ascend
    //angle math was getting inconsistent
    IEnumerator initAscendTimer()
    {
        yield return new WaitForSeconds(timeInInitAscend);
        ascendState = ascendingStates.ascending;
    }

    void DisplayDebugLine()
    {
        debugLine.transform.position = transform.position;
    }

}
