using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableBody : MonoBehaviour {
	public PointMass pointmass_prefab;

	public Vector3 position;
	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles;

	public float rest_length;
	public float k_spring;
	public float k_dampening;
	public float mass;
	public float dt;
	public int width; // x
	public int height; // y
	public int depth; // z

	private List<PointMass> point_mass_list;
	PointMass[ , , ] tmp_intermediate_pointmass;
	PointMass[ , , ] old_pointmass;
	PointMass[ , , ] point_mass;

/*****************************************************************************
http://wiki.roblox.com/index.php?title=Verlet_integration
*****************************************************************************/
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
		tmp_intermediate_pointmass = new PointMass[width, height, depth];
		old_pointmass = new PointMass[width, height, depth];
		point_mass = new PointMass[width, height, depth];

		init();
	}
	
	// Update is called once per frame
	void Update() 
	{
		Debug.Log("Fps: " + 1 / Time.deltaTime);
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;

		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += normals[i] * Mathf.Sin(Time.time); // just to test
		}

		mesh.vertices = vertices;

		// like cloth now...

		spring_force_calculations(dt);
	}

	private void init()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					/* 	Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
						tmp_agent.transform.parent = transform;
						tmp_agent.initialize(vision_distance, agent_radius);
			
						agents.Add(tmp_agent);
					*/
					PointMass point_mass = Instantiate(pointmass_prefab, 
											new Vector3(0, 0, 0),
											Quaternion.identity) as PointMass;
					point_mass.transform.parent = transform;
					point_mass.position = new Vector3(2 * i * rest_length, 2 * j * rest_length, 2 * k * rest_length);
					point_mass.to_string();
					point_mass_list.Add(point_mass);
				}
			}
		}
		
		int pm_count = 0;

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					tmp_intermediate_pointmass[i, j, k] = point_mass_list[pm_count];
					old_pointmass[i, j, k] = point_mass_list[pm_count];
					point_mass[i, j, k] = point_mass_list[pm_count];
					pm_count++;
				}
			}
		}
	}


	/* Helpful links:
		https://www.scss.tcd.ie/Michael.Manzke/CS7057/cs7057-1516-14-MassSpringSystems-mm.pdf
		https://graphics.stanford.edu/~mdfisher/cloth.html
		http://hugi.scene.org/online/hugi28/hugi%2028%20-%20coding%20corner%20uttumuttu%20implementing%20the%20implicit%20euler%20method%20for%20mass-spring%20systems.htm
		https://github.com/lilinitsy/hackrender/blob/master/cloth_sim/src/Cloth.h
	*/
	private void spring_force_calculations(float delta_time)
	{
		Debug.Log("fps: " + 1 / Time.deltaTime);
		/*
			This triple for loop block will assign the tmp_intermediate pointmass
			to the normal pointmass; this will need to be done so that after the computations
			to modify point_mass[i, j, k], old_pointmass can be reassigned to the old pointmass value.
		*/

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					tmp_intermediate_pointmass[i, j, k].position = point_mass[i, j, k].position;
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
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize(); // confirmed functioning

					float v1 = Vector3.Dot(e, old_pointmass[i, j, k].rb.velocity);
					float v2 = Vector3.Dot(e, old_pointmass[i + 1, j, k].rb.velocity);
					float force = -1.0f * k_spring * (rest_length - l) - k_dampening * (v1 - v2);

					point_mass[i, j, k].rb.velocity += (force / mass) * e * delta_time;
					point_mass[i + 1, j, k].rb.velocity -= (force / mass) * e * delta_time;
				}
			}
		} 

		// all the y forces
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height - 1; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					Vector3 e = old_pointmass[i, j + 1, k].position - old_pointmass[i, j, k].position;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize(); // confirmed functioning

					float v1 = Vector3.Dot(e, old_pointmass[i, j, k].rb.velocity);
					float v2 = Vector3.Dot(e, old_pointmass[i, j + 1, k].rb.velocity);
					float force = -1.0f * k_spring * (rest_length - l) - k_dampening * (v1 - v2);

					point_mass[i, j, k].rb.velocity += (force / mass) * e * delta_time;
					point_mass[i, j + 1, k].rb.velocity -= (force / mass) * e * delta_time;
				}
			}
		}



		// all the z forces
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth - 1; k++)
				{
					Vector3 e = old_pointmass[i, j, k + 1].position - old_pointmass[i, j, k].position;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize(); // confirmed functioning

					float v1 = Vector3.Dot(e, old_pointmass[i, j, k].rb.velocity);
					float v2 = Vector3.Dot(e, old_pointmass[i, j, k + 1].rb.velocity);
					float force = -1.0f * k_spring * (rest_length - l) - k_dampening * (v1 - v2);

					point_mass[i, j, k].rb.velocity += (force / mass) * e * delta_time;
					point_mass[i, j, k + 1].rb.velocity -= (force / mass) * e * delta_time;
				}
			}
		}


		// assign positions based on velocities
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					point_mass[i, j, k].position += point_mass[i, j, k].rb.velocity * delta_time;
				}
			}
		}

		// update the old_pointmass array
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_pointmass[i, j, k].position = tmp_intermediate_pointmass[i, j, k].position;
				}
			}
		}

	}

	void to_string()
	{

	}
}
