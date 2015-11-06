using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class bgTextureScroll : MonoBehaviour {

	public List<GameObject> layers = new List<GameObject>();
	public List<float> speeds = new List<float>();
    private float scrollSpeedX;
    private Renderer rend;
    private float offsetX;
    private float initWorldSpaceX;

    // Use this for initialization
    void Start () {
        offsetX = scrollSpeedX * Time.time;
        initWorldSpaceX = transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		for(int i = 0; i < layers.ToArray().Length; i++)
		{
			rend = layers[i].GetComponent<Renderer>();
			offsetX = speeds[i] * (transform.position.x - initWorldSpaceX) * .001f;
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (offsetX, 0));
		}
	}
}
