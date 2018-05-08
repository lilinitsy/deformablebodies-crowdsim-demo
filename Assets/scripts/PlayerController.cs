using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private float s = 0.1f;
	private float timeSinceShoot = 0.0f;
	public Transform laser;
	void Update() {
		//Input Movement
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
		if (Input.GetKey ("left shift")) {
			s += 0.01f*Time.deltaTime*100.0f;
		}
		if (Input.GetKey ("left ctrl")) {
			s -= 0.01f*Time.deltaTime*100.0f;
		}
		s = Mathf.Clamp (s, 0.0f, 3.0f);
		transform.Rotate (x, 0, 0);
		transform.Rotate (0, -y, 0);
		transform.Rotate (0, 0, -z);
		transform.Translate (0, 0, s);

		//Shoot Lasers
		if (Input.GetKey ("space") && timeSinceShoot > 0.05f) {
			timeSinceShoot = 0.0f;
			Transform obj1 = Instantiate(laser, transform.position, transform.rotation);
			Transform p = gameObject.transform;
			obj1.transform.parent = p;
			obj1.transform.localPosition = new Vector3 (2.3f, s, 2);
			Transform obj2 = Instantiate(laser, transform.position, transform.rotation);
			obj2.transform.parent = p;
			obj2.transform.localPosition = new Vector3 (-2.3f, s, 2);
		} else {
			timeSinceShoot += Time.deltaTime;
		}

		//Turn around if  OOB
		if (transform.position.magnitude > 2500) {
			transform.Rotate (0, 10, 0);
			transform.Translate (0, 0, 2 * s);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		Debug.Log("I WONDER if I should DIE");


		if(other.gameObject.name == "Projectile_AI(Clone)" ||
			other.gameObject.name == "Agent(Clone)"||
			other.gameObject.name.Contains("EarthSimple"))
		{
			Debug.Log("I Should DIE");
			Destroy(gameObject);
		}

	}
}