using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMass : MonoBehaviour 
{
	public Vector3 position;

	private Rigidbody rigidbody;

	// Use this for initialization
	void Start() 
	{
		transform.position = position;
		rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update() 
	{
		
	}

	void to_string()
	{
		Debug.Log("Position: " + transform.position.ToString("F8") + "\t Velocity: " + rigidbody.velocity.ToString());
	}
}
