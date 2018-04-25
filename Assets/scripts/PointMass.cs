using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMass : MonoBehaviour 
{
	public Vector3 position;

	public Rigidbody rb;

	// Use this for initialization
	void Start() 
	{
		transform.position = position;
		rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update() 
	{
		transform.position = position;
	}

	public void to_string()
	{
		Debug.Log("Position: " + transform.position.ToString("F8") + "\t Velocity: " + rb.velocity.ToString());
	}
}
