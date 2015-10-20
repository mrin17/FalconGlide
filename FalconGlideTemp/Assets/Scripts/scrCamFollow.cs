using UnityEngine;
using System.Collections;

public class scrCamFollow : MonoBehaviour {

    public GameObject falcon;
    readonly Vector3 zOff = new Vector3(0, 0, -10);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = falcon.transform.position + zOff;
	}
}
