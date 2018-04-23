using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableBody : MonoBehaviour {
	public Vector3 position;
	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles;


	// Use this for initialization
	void Start() 
	{
		transform.position = position;

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = new_vertices;
		mesh.uv = new_uv; // not gonna fuck with textures for now
		mesh.triangles = new_triangles;
	}
	
	// Update is called once per frame
	void Update() 
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;

		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += normals[i] * Mathf.Sin(Time.time); // just to test
		}

		mesh.vertices = vertices;
	}

	void to_string()
	{

	}
}
