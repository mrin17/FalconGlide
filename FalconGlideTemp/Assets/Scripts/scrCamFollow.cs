using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class scrCamFollow : MonoBehaviour {

    const float SPEED = .03f;
    float maxAddSpeed = 0;
    public List<GameObject> falcons = new List<GameObject>();
    float orthoProj = 0;

    // Use this for initialization
    void Start () {
        orthoProj = GetComponent<Camera>().orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {

		if (falcons.Where 
		    (f => f.transform.position.x > transform.position.x + orthoProj).Any ()) {

			maxAddSpeed += .001f;
			transform.Translate (new Vector3 (SPEED + maxAddSpeed, 0, 0));	
		}

		else {
			maxAddSpeed = maxAddSpeed > 0 ? maxAddSpeed - .001f : 0;
			transform.Translate (new Vector3 (SPEED, 0, 0));	
		}
    }
}
