using UnityEngine;
using System.Collections;

public class scrFalcon : MonoBehaviour {

    const float DEFAULT_SPEED_X = 3;
    public float downButtonMultiplier = 5;
    public float releaseDownMultiplier = 60;
    public float glidingMultiplier = 7;
    Rigidbody2D rb;
    Vector2 artificialGravity = new Vector2(0, -8);
    Vector2 gravityModifier = new Vector2(0, 0);
    float ascensionPoints = 0f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        transform.Translate(5, 0, 0);
	}

    // Update is called once per frame
    void Update() {
        //If you hold down, your falcon dives. Increase gravity and horizontal velocity.
        if (Input.GetKey("down")) {
            gravityModifier = new Vector2(1, -1) * downButtonMultiplier;
        }
        //if you release down, add a multiple of your current downward velocity to your velocity
        else if (Input.GetKeyUp("down"))
        {
            gravityModifier = new Vector2(0, Mathf.Abs(rb.velocity.y) * releaseDownMultiplier);
        }
        //if you are not holding down, your falcon's wings are spread.
        //Decrease gravity, 
        else
        {
            gravityModifier = new Vector2(0, glidingMultiplier);
        }

        //Total Force
        rb.AddForce(artificialGravity + gravityModifier);

    }
}
