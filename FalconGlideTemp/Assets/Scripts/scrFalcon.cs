using UnityEngine;
using System.Collections;

public class scrFalcon : MonoBehaviour {

    const float DEFAULT_SPEED_X = 3;
    Rigidbody2D rb;
    Vector2 artificialGravity = new Vector2(0, -8);
    Vector2 gravityModifier = new Vector2(0, 0);
    float ascensionPoints = 0f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update() {
        //If you hold down, your falcon dives. Increase gravity and horizontal velocity.
        //Also, increase ascensionPoints by the amount you move down
        if (Input.GetKey("down")) {
            gravityModifier = new Vector2(5, -5);
            ascensionPoints += .1f + ascensionPoints;
            if (ascensionPoints > 300)
                ascensionPoints = 300;
        }
        //if you are not holding down, your falcon's wings are spread.
        //Decrease gravity, add your ascensionPoints to the y velocity (to have your falcon rise),
        //and clear ascensionPoints
        else {
            gravityModifier = new Vector2(0, 7 + ascensionPoints*3);
            Debug.Log(ascensionPoints);
            ascensionPoints = 0;
        }

        //Total Force
        rb.AddForce(artificialGravity + gravityModifier);

    }
}
