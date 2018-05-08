using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndGame : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(GameObject.FindWithTag("Jelly") == null) 
		{
			Debug.Log("WIIIIIIIIN");	
			SceneManager.LoadScene("Win Scene");
		 }

		if(GameObject.FindWithTag("play") == null)
		{
			Debug.Log("YOU LOOOOOSE");
			SceneManager.LoadScene("Lose Scene");
		}

	}
}
