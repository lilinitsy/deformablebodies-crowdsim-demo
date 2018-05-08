using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour {

	private float life = 0.0f;
	private float speed = 0.0f;

	// Use this for initialization
	void Start () {
		transform.Rotate (90, 0, 0);
		speed = transform.localPosition.y + 0.2f;
		transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
		transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (0, speed, 0);
		life += Time.deltaTime;
		if (life > 5) {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		// Debug.Log (collision.collider.name);
		if(collision.collider.name == "Jelly1") 
		{
			Debug.Log("Collided with jelly1");
			Destroy(gameObject);
		}

		if(collision.collider.name == "Jelly2") 
		{
			Debug.Log("Collided with jelly2");
			Destroy(gameObject);
		}
	}
}
