using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var a = Random.Range (0.0f, 2.0f * Mathf.PI);
		var r = 2000.0f*Mathf.Sqrt (Random.Range (0.1f, 1.0f));
		transform.position = new Vector3(r*Mathf.Sin(a), 0.0f, r*Mathf.Cos(a));
		var s = Random.Range (15.0f, 55.0f);
		transform.localScale = new Vector3 (s, s, s);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
