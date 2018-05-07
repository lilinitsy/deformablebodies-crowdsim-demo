using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{

	// Use this for initialization
	void Start() 
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		float x = Input.GetAxis("Vertical") * Time.deltaTime * 100.0f;
		float y = Input.GetAxis("Horizontal") * Time.deltaTime * 100.0f;

	}
}
