using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        if (maxAddSpeed > 0)
            maxAddSpeed -= .001f;
        else
            maxAddSpeed = 0;
        for (int i = 0; i < falcons.Count; i++)
        {
            if (falcons[i].transform.position.x > transform.position.x + orthoProj)
            {
                float vel = falcons[i].GetComponent<Rigidbody2D>().velocity.x / 30;
                if (vel > maxAddSpeed)
                {
                    maxAddSpeed = vel;
                }
            }
        }
        transform.Translate(new Vector3(SPEED + maxAddSpeed, 0, 0));
    }
}
