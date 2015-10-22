using UnityEngine;
using System.Collections;

public class scrFalcon : MonoBehaviour {

    public float defaultSpeedX = 3;
    public float downButtonMultiplier = 5;
    public float impulseModifier = 1.25f;
    public float glidingMultiplier = 7f;
    Rigidbody2D rb;
    Vector2 artificialGravity = new Vector2(0, -8);
    Vector2 gravityModifier = new Vector2(0, 0);
    float ascensionPoints = 0f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        transform.Translate(new Vector2(5, 0));
        rb.AddForce(new Vector2(defaultSpeedX, 0));
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
            if (rb.velocity.y < -5)
                rb.AddForce(new Vector2(0, Mathf.Abs(rb.velocity.y) * impulseModifier), ForceMode2D.Impulse);
           
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
