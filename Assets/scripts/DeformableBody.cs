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
	public float k;
	public float k_dampening;
	public float mass;
	public float dt;
	public int width; // x
	public int height; // y
	public int depth; // z

	private List<PointMass> point_mass_list;
	PointMass[ , , ] tmp_value_pointmass;
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
		tmp_value_pointmass = new PointMass[width, height, depth];
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
					point_mass.position = new Vector3(i * rest_length, j * rest_length, k * rest_length);
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
					tmp_value_pointmass[i, j, k] = point_mass_list[pm_count];
					old_pointmass[i, j, k] = point_mass_list[pm_count];
					point_mass[i, j, k] = point_mass_list[pm_count];
					pm_count++;
				}
			}
		}
	}


	// the old/old_old is wrong, check the roblox link above tomorrow to fix
	private void spring_force_calculations(float delta_time)
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_pointmass[i, j, k].position = point_mass[i, j, k].position;
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
					float force_physical = -0.5f * k * (rest_length - l) - k_dampening * 
								(Vector3.Dot(e, old_pointmass[i, j, k].position - tmp_value_pointmass[i, j, k].position) / 
								delta_time);
					float force_sound = 0.0f; // todo later
					float force = force_physical + force_sound;
//					Debug.Log("Force: " + force.ToString("F8"));

					Vector3 new_position = 2.0f * point_mass[i, j, k].position
													- old_pointmass[i, j, k].position
													+ (force / point_mass[i, j, k].rb.mass) * e * delta_time * delta_time;
					point_mass[i, j, k].position += new_position;
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
					tmp_value_pointmass[i, j, k].position = old_pointmass[i, j, k].position;
				}
			}
		}

	}

	void to_string()
	{

	}
}
