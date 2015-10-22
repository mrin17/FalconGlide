using UnityEngine;
using System.Collections;

public class PowerUpManage : MonoBehaviour {

	//Stores the current held powerup (can only store 1 at a time)
	// -1 = no powerup
	//  1 = Poop powerup 
	int currentHeldPowerup = -1;
	public GameObject PoopProjectile;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (currentHeldPowerup == 1) {
				currentHeldPowerup = -1; //Clear out inventory and set to empty
				Instantiate(PoopProjectile, transform.position, transform.rotation);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (currentHeldPowerup == -1) { //Don't continue unless we are empty
			//List powerups below in if blocks
			if (col.gameObject.CompareTag("PoopPowerup")) {
				currentHeldPowerup = 1; //Sets our inventory to hold this powerup
				Destroy (col.gameObject); //Remove the powerup from the game
			}
		}
	}
}
