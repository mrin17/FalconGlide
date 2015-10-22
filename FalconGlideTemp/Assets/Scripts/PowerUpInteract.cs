using UnityEngine;
using System.Collections;

public class PowerUpInteract : MonoBehaviour {

	Rigidbody2D falconBody;
	public float slowDownModifier = 1.0f;
	bool isPoopSlowed = false;

	void Start() {
		falconBody = this.gameObject.GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (isPoopSlowed) { //If the player is being slowed by poop
			float currentXVelocity = falconBody.velocity.x;
			falconBody.AddForce(new Vector2(-slowDownModifier * currentXVelocity * Time.deltaTime, 0));
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		//List powerups below in if blocks
		if (col.gameObject.CompareTag("PoopProjectile")) {
			Destroy (col.gameObject); //Remove the projectile from the game
			StartCoroutine(PoopSlowTimer(1.0f)); //Sets how long the slow effect should last
		}
	}

	//Timer for slowing down a falcon hit by poop
	IEnumerator PoopSlowTimer (float seconds)
	{
		isPoopSlowed = true;
		yield return new WaitForSeconds(seconds);
		isPoopSlowed = false;
	}
}
