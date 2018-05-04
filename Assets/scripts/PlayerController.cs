using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private float s = 0.1f;
	void Update() {
		var x = Input.GetAxis("Vertical") * Time.deltaTime * 100.0f;
		var y = 0.0f;
		var z = Input.GetAxis("Horizontal") * Time.deltaTime * 100.0f;
		var t = Time.deltaTime * 100.0f;
		if (Input.GetKey ("q")) {
			y = Mathf.MoveTowards(y, 0.5f, Time.deltaTime*100.0f);
		}
		if (Input.GetKey ("e")) {
			y = Mathf.MoveTowards(y, -0.5f, Time.deltaTime*100.0f);
		}
		if (Input.GetKey ("space")) {
			s += 0.01f*Time.deltaTime*100.0f;
		}
		if (Input.GetKey ("left shift")) {
			s -= 0.01f*Time.deltaTime*100.0f;
		}
		s = Mathf.Clamp (s, 0.0f, 5.0f);
		transform.Rotate (x, 0, 0);
		transform.Rotate (0, -y, 0);
		transform.Rotate (0, 0, -z);
		transform.Translate (0, 0, s);
	}
}