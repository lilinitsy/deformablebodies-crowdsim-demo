using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDeformation : MonoBehaviour {
	public PointMass pointmass_prefab;	

	public AudioSource audio_source; // DO NOT attach an audiosource to the jelly; attach it here so I can get a distance

	public Vector3 position;
	public Vector3 average_position;
	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles;

	public float rest_length;
	public float dt;
	public int width; // x
	public int height; // y
	public int depth; // z

	public float[] spectrum_samples;
	private float[] frequency_groups;
	private List<PointMass> point_mass_list;
	private PointMass[ , , ] point_mass;
	private Vector3[ , ,] anchor_points;

	void Start() 
	{
		// before making the mesh, 
		// get the vertices from all the child pointmasses or some shit
		// Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
		average_position = new Vector3(0, 0, 0);

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = new_vertices;
		mesh.uv = new_uv; // not gonna fuck with textures for now
		mesh.triangles = new_triangles;

		point_mass_list = new List<PointMass>();
		point_mass = new PointMass[width, height, depth];
		anchor_points = new Vector3[width, height, depth];

		spectrum_samples = new float[512];
		frequency_groups = new float[8];

		init();
	}

	void Update() 
	{
		// Debug.Log("Fps: " + 1 / Time.deltaTime);
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;

		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += normals[i] * Mathf.Sin(Time.time); // just to test
		}

		mesh.vertices = vertices;

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					average_position += point_mass[i, j, k].position;
				}
			}
		}

		average_position /= width * height * depth;
		gameObject.transform.GetChild(0).transform.position = average_position;
		
		audio_source.GetSpectrumData(spectrum_samples, 0, FFTWindow.Rectangular);
		frequency_groups = calculate_frequency_groups();
		
		for(int i = 0; i < 512; i++)
		{
			/*
				get the unit vector e
					e = Vector3.Normalize(audio_source.transform.position - point_masslist[i])
			 */
		}

		sound_deformation();
	}

	private void init()
	{
		for(int i = -1 * width / 2; i < width / 2; i++)
		{
			for(int j = -1 * width / 2; j < height / 2; j++)
			{
				for(int k = -1 * width / 2; k < depth / 2; k++)
				{
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
					point_mass[i, j, k] = point_mass_list[pm_count];
					anchor_points[i, j, k] = point_mass_list[pm_count].position;
					pm_count++;
				}
			}
		}
	}

	void sound_deformation()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < width; k++)
				{
					Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
					float length = Vector3.Magnitude(e);
					e.Normalize();

					point_mass[i, j, k].position = anchor_points[i, j, k] + e * length * frequency_groups[0];
				}
			}
		} 
	}
	
	private float[] calculate_frequency_groups()
	{
		float[] f_group = new float[8];
		int count = 0;

		for(int i = 0; i < f_group.Length; i++)
		{
			float average = 0;
			int sample_count = (int) Mathf.Pow(2, i) * 2;

			if(i == 3)
			{
				sample_count += 2;
			}

			for(int j = 0; j < sample_count; j++)
			{
				average += spectrum_samples[count] * (count + 1);
				count++;
			}

			average /= count;
			f_group[i] = average * 10;
		}


		return f_group;
	}

}
