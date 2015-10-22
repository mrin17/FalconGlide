using UnityEngine;
using System.Collections;

public class BouncyScript : MonoBehaviour {

	[Range(0, 1)]
	public float yBouncePercent = 1;

	[Range(0, 1)]
	public float xBouncePercent = 1;

	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Rigidbody2D rigid = coll.gameObject.GetComponent<Rigidbody2D>();
		rigid.AddForce(new Vector2(Mathf.Abs(rigid.velocity.magnitude) * xBouncePercent, Mathf.Abs(rigid.velocity.magnitude) * yBouncePercent), ForceMode2D.Impulse);
	}
}
