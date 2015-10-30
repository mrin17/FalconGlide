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
    public float maxSpeedY = 15;
    public float ascendInitVerticalMultiplier = 1.5f;
    public float yAscendMult = -.8f;
    public float angleMult = 2f;
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

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rb.AddForce(new Vector2(transform.right.x, transform.right.y) * movementSpeed, ForceMode2D.Force);
        downKeyDown = Input.GetKeyDown(keyForMovement);
        downKeyPressed = Input.GetKey(keyForMovement);
        downKeyReleased = Input.GetKeyUp(keyForMovement);
        if (downKeyDown)
        {
            yPosDiveStart = transform.position.y;
        }
        else if (downKeyPressed)
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

    void DivingControl()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -maxAngle), Time.deltaTime * downRotationSpeed);
    }

    void ReleaseControl()
    {
        //maxAscendingAngle = (360 - transform.rotation.eulerAngles.z);
        maxAscendingAngle = (yPosDiveStart - transform.position.y) * angleMult;
        yOfMaxRise = yPosDiveStart + (yPosDiveStart - transform.position.y) * yAscendMult;
        //maxMagnitude = rb.velocity.magnitude / maxMagRatio;
        //rb.AddForce(new Vector2(0, rb.velocity.y * swoopNegateMultiplier), ForceMode2D.Impulse);
        CurState = falconStates.ascending;
    }

    void AscendingControl()
    {
        float rotationSpeed = 0;
        float angle = 0;
        switch(ascendState)
        {
            case ascendingStates.ascendingInit:
                rotationSpeed = initUpRotationSpeed;
                angle = 0;
                rb.AddForce(new Vector2(0, ascendInitVerticalMultiplier * rb.velocity.y * -1));
                break;
            case ascendingStates.ascending:
                rotationSpeed = upRotationSpeed;
                angle = maxAscendingAngle;
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        if (transform.rotation.eulerAngles.z < 5 || 360 - transform.rotation.eulerAngles.z < 5)
        {
            ascendState = ascendingStates.ascending;
        }
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

    void GlidingControl()
    {
        rb.AddForce(new Vector2(0, gravity), ForceMode2D.Force);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * downRotationSpeed);
    }

}
