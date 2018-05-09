using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndGame : MonoBehaviour 
{
	private float timer = 0.0f;
	// Use this for initialization
	void Start() 
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(GameObject.FindWithTag("Jelly") == null) 
		{
			SceneManager.LoadScene("Win Scene");
		 }

		if(GameObject.FindWithTag("play") == null)
		{
			timer += Time.deltaTime;
		}

		if(timer > 3.0f)
		{
			SceneManager.LoadScene("Lose Scene");
		}

	}
}
