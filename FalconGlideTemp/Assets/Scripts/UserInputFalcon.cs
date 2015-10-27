﻿using UnityEngine;
using System.Collections;

public class UserInputFalcon : MonoBehaviour {

    public float gravity = -4;
    public string keyForMovement = "down";
    public float maxAngle = 45;
    public float downRotationSpeed = 1;
    public float upRotationSpeed = 1;
    public float initUpRotationSpeed = 4;
    public float movementSpeed = 5;
    public float swoopYNegateMultiplier = -.9f;
    public float maxSpeedX = 20;
    public float minSpeedX = 5;

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

    bool downKeyPressed = false;
    bool downKeyReleased = false;
    float maxAscendingAngle = 0;
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
        downKeyPressed = Input.GetKey(keyForMovement);
        downKeyReleased = Input.GetKeyUp(keyForMovement);
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
        if (rb.velocity.x > maxSpeedX)
        {
            rb.velocity = new Vector2(maxSpeedX, rb.velocity.y);
        }
        if (rb.velocity.x < minSpeedX)
        {
            rb.velocity = new Vector2(minSpeedX, rb.velocity.y);
        }
    }

    void DivingControl()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -maxAngle), Time.deltaTime * downRotationSpeed);
    }

    void ReleaseControl()
    {
        maxAscendingAngle = (360 - transform.rotation.eulerAngles.z);
        rb.AddForce(new Vector2(0, rb.velocity.y * swoopYNegateMultiplier), ForceMode2D.Impulse);
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
                break;
            case ascendingStates.ascending:
                rotationSpeed = upRotationSpeed;
                angle = maxAscendingAngle;
                break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        if (Mathf.Abs(transform.rotation.eulerAngles.z) < 2)
        {
            ascendState = ascendingStates.ascending;
        }
        if (Mathf.Abs(transform.rotation.eulerAngles.z - maxAscendingAngle) < 2)
        {
            CurState = falconStates.gliding;
        }

    }

    void GlidingControl()
    {
        rb.AddForce(new Vector2(0, gravity), ForceMode2D.Force);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * downRotationSpeed);
    }

}
