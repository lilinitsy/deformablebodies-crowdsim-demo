using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableBody : MonoBehaviour {
	public Vector3 position;
	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles;

	public float rest_length;
	public float k;
	public float k_dampening;
	public float mass;
	public int width; // x
	public int height; // y
	public int depth; // z

	private List<PointMass> point_mass_list;
	PointMass[ , , ] old_old_pointmass;
	PointMass[ , , ] old_pointmass;
	PointMass[ , , ] point_mass;


	// Use this for initialization
	void Start() 
	{
		transform.position = position;

		// before making the mesh, 
		// get the vertices from all the child pointmasses or some shit
// 			Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;


		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = new_vertices;
		mesh.uv = new_uv; // not gonna fuck with textures for now
		mesh.triangles = new_triangles;

		point_mass_list = new List<PointMass>();
		old_old_pointmass = new PointMass[width, height, depth];
		old_pointmass = new PointMass[width, height, depth];
		point_mass = new PointMass[width, height, depth];

		spring_force_calculations();

		init();
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

		// like cloth now...

		// 
	}

	private void init()
	{
		int pm_count = 0;

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_pointmass[i, j, k] = point_mass_list[pm_count];
					pm_count++;
				}
			}
		}
	}

	private void spring_force_calculations(float delta_time)
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_pointmass[i, j, k].position = old_old_pointmass[i, j, k].position;
				}
			}
		}
		// calculate all the x forces
		for(int i = 0; i < width - 1; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					Vector3 e = old_pointmass[i + 1, j, k].position - old_pointmass[i, j, k].position;
					float l = Vector3.Magnitude(e);
					e.Normalize();

					// need to check for self-collisions; will do later


					
					// else (not self-collision), do verlet integration
					// need to calculate force though; have a private function for this, based off sound
					// once we get force, need to equate that to acceleration
					float force = -0.5f * k * (rest_length - l) - k_dampening * (Vector3.Dot(e, old_pointmass[i, j, k].position - old_old_pointmass[i, j, k].position) / delta_time);

					float accel = 1.0f; // default to 1
					Vector3 tmp_position = 2.0f * point_mass[i, j, k].position
													- old_pointmass[i, j, k].position
													+ (force / mass) * e * accel * delta_time * delta_time;
					
					point_mass[i, j, k].position = tmp_position;
				}
			}
		}

		// all the y forces




		// all the z forces




		// update the old_pointmass array
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_old_pointmass[i, j, k].position = point_mass[i, j, k].position;
				}
			}
		}

	}

	void to_string()
	{

	}
}
