using UnityEngine;
using System.Collections;

public class UserInputFalcon : MonoBehaviour {

    public float gravity = -4;
    public string keyForMovement = "down";
    public float maxAngle = 60;
    public float downRotationSpeed = 3;
    public float upRotationSpeed = 2;
    public float initUpRotationSpeed = 4;
    public float movementSpeed = 15;
    //public float swoopYNegateMultiplier = 0;
    public float maxSpeedX = 20;
    public float minSpeedX = 5;
    public float maxSpeedY = 10;
    public float ascendInitVerticalMultiplier = 4f;
    public float ascendInitAngle = 30;
    public float yAscendMult = -.5f;
    public float angleMult = 2f;
    public float timeInInitAscend = .5f;
    //public float maxMagRatio = 1.1f;

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
    float maxAscendingAngle = 0;
    float yPosDiveStart = 0;
    float yOfMaxRise = 0;
    //float maxMagnitude = 0;
    Rigidbody2D rb;

    // add initial x force
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(10, 0));
	}
	
	// Update is called once per frame
	void Update ()
    {
        //rb.AddForce(new Vector2(transform.right.x, transform.right.y) * movementSpeed, ForceMode2D.Force);
        downKeyDown = Input.GetKeyDown(keyForMovement);
        downKeyPressed = Input.GetKey(keyForMovement);
        downKeyReleased = Input.GetKeyUp(keyForMovement);
	//if you pressed down, record the y position
        if (downKeyDown)
        {
            yPosDiveStart = transform.position.y;
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
        if (rb.velocity.y > maxSpeedY)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxSpeedY);
        }
    }

    //slerp the angle downwards, and add force to your transform.right
    void DivingControl()
    {
        rb.AddForce(new Vector2(transform.right.x, transform.right.y) * movementSpeed, ForceMode2D.Force);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -maxAngle), Time.deltaTime * downRotationSpeed);
    }

    //track the max ascending angle, track the y position at which you should stop ascending, start the ascend init timer
    void ReleaseControl()
    {
        //maxAscendingAngle = (360 - transform.rotation.eulerAngles.z);
        rb.AddForce(new Vector2(0, transform.right.y) * movementSpeed, ForceMode2D.Force);
        maxAscendingAngle = (yPosDiveStart - transform.position.y) * angleMult;
        if (maxAscendingAngle < ascendInitAngle)
        {
            maxAscendingAngle = ascendInitAngle;
        }
        yOfMaxRise = yPosDiveStart + (yPosDiveStart - transform.position.y) * yAscendMult;
        //maxMagnitude = rb.velocity.magnitude / maxMagRatio;
        //rb.AddForce(new Vector2(0, rb.velocity.y * swoopNegateMultiplier), ForceMode2D.Impulse);
        CurState = falconStates.ascending;
        StartCoroutine(initAscendTimer());
    }

    //add force of your transform.right to ONLY your y, slerp the angle based on state in ascending, if you're higher than maxRise, go back to gliding
    void AscendingControl()
    {
        rb.AddForce(new Vector2(0, transform.right.y) * movementSpeed, ForceMode2D.Force);
        float rotationSpeed = 0;
        float angle = 0;
        switch(ascendState)
        {
            case ascendingStates.ascendingInit:
                rotationSpeed = initUpRotationSpeed;
                angle = ascendInitAngle;
                rb.AddForce(new Vector2(0, ascendInitVerticalMultiplier * rb.velocity.y * -1));
                break;
            case ascendingStates.ascending:
                rotationSpeed = upRotationSpeed;
                angle = maxAscendingAngle;
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        if (transform.position.y > yOfMaxRise)//rb.velocity.magnitude >= maxMagnitude && ascendState == ascendingStates.ascending)//transform.position.y > yOfMaxRise)
        {
            CurState = falconStates.gliding;
        }
        /*
        if (Mathf.Abs(transform.rotation.eulerAngles.z - maxAscendingAngle) < 5)
        {
            CurState = falconStates.gliding;
        }
        */
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

}
