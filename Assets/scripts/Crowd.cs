using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SHOULD HAVE AN AIBehaviour
// SHOULD HAVE WHATEVER C#'s VECTOR IS OF AGENTS

public class Crowd : MonoBehaviour 
{

	// Use this for initialization
	void Start() 
    {
        foreach(Transform child in transform)
        {
            Debug.Log(child.transform.position);
        }
	}
	
	// Update is called once per frame
	void Update() 
    {
		
	}
}
