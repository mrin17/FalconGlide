using UnityEngine;
using System.Collections;

public class scrFalcon : MonoBehaviour {

    float rotation = 90;
    const float DEFAULT_SPEED_X = 3;
    Rigidbody2D rb;
    Vector2 artificialGravity = new Vector2(0, -5);
    float ascensionPoints = 0f;
    const float ASCENSION_PTS_MAX = 100;
    const float ASCENSION_PTS_MIN = 0;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKey("down")) {
            if (rotation > 40)
            rotation = Mathf.Max(40, rotation - 4);
        }
        if (Input.GetKey("up")) {
            if (rotation < 140)
                rotation = Mathf.Min(140, rotation + 4);
        }
        //Rotation Calcs
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        float ascension = 4 * Mathf.Sin(Mathf.Deg2Rad * (rotation - 90));
        float vectorY = ascension - 2;
        float vectorX = Mathf.Cos(Mathf.Deg2Rad * (rotation - 90)) * DEFAULT_SPEED_X;
        Vector2 totalForce = new Vector2(vectorX, vectorY);

        //Ascension Calcs
        //you gain/lose ascension points based on your angle
        //if you are facing down, your horizontal velocity increases and max downward velocity increases (as long as points < MAX)
        //if you are facing up, your upward velocity increases and horizontal velocity decreases (as long as points > MIN)
        //(for now)
        if (rotation == 40)
        {
            ascensionPoints -= ascension;
            if (ascensionPoints < ASCENSION_PTS_MAX)
                totalForce += new Vector2(DEFAULT_SPEED_X / 2, 0);
            else
                ascensionPoints = ASCENSION_PTS_MAX;
        }
        else if (rotation == 140)
        {
            ascensionPoints -= ascension;
            if (ascensionPoints > ASCENSION_PTS_MIN)
                totalForce += new Vector2(-10, 30);
            else
                ascensionPoints = ASCENSION_PTS_MIN;
        }
        Debug.Log(ascensionPoints);

        //Total Force
        totalForce += artificialGravity;
        rb.AddForce(totalForce * .5f);
        float maxRise = 30;
        float maxFall = -4;
        if (ascension < 0)
            maxFall += ascension;
        if (rb.velocity.y > maxRise)
            rb.velocity = new Vector2(rb.velocity.x, maxRise);
        if (rb.velocity.y < maxFall)
            rb.velocity = new Vector2(rb.velocity.x, maxFall);
        //Debug.Log(rb.velocity);
    }
}
