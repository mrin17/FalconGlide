using UnityEngine;
using System.Collections;

public class scrFalcon : MonoBehaviour {

	public Collider2D otherFalcon;
    public float defaultSpeedX = 25;
    public Vector2 downButtonMultiplier = new Vector2(0, -5);
    public Vector2 impulseMultiplier = new Vector2(.2f, 1.3f);
    public string keyForMovement = "down";
    public float glidingMultiplier = 7f;
    public float releaseDownTimerMax = .5f;
    public float delayBeforeImpulseUp = .2f;
    float releaseDownTimer = 0;
    Rigidbody2D rb;
	Animator anim;
    Vector2 artificialGravity = new Vector2(0, -8);
    Vector2 gravityModifier = new Vector2(0, 0);
    float ascensionPoints = 0f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        transform.Translate(new Vector2(7, 0));
        rb.AddForce(new Vector2(defaultSpeedX, 0));
		anim = GetComponent<Animator> ();
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), otherFalcon);
	}

    // Update is called once per frame
    void Update() {
        releaseDownTimer -= Time.deltaTime;
		if (Input.GetKeyDown (keyForMovement)) {
			anim.SetBool("isDiving", true);
		}
        //If you hold down, your falcon dives. Increase gravity and horizontal velocity.
        if (Input.GetKey(keyForMovement)) {
            gravityModifier = downButtonMultiplier;
        }
        //if you release down, add a multiple of your current downward velocity to your velocity
        else if (Input.GetKeyUp(keyForMovement)) {
			if (releaseDownTimer < 0 && rb.velocity.y < 0)   {
                StartCoroutine(addForceUpwards());
			}
			anim.SetBool("isDiving", false);
        }
        //if you are not holding down, your falcon's wings are spread. Decrease gravity
        else
        {
            gravityModifier = new Vector2(0, glidingMultiplier);
        }

        //Total Force
        rb.AddForce(artificialGravity + gravityModifier);

    }

    IEnumerator addForceUpwards()
    {
        yield return new WaitForSeconds(delayBeforeImpulseUp);
        rb.AddForce(impulseMultiplier * Mathf.Abs(rb.velocity.y), ForceMode2D.Impulse);
        releaseDownTimer = releaseDownTimerMax;
    }
}
