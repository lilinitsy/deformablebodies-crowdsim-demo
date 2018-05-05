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
		average_position = new Vector3(0, 0, 0);
		new_vertices = new Vector3[width * height * depth];
		new_uv = new Vector2[width * height * depth];

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
		

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		/*
		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += normals[i] * Mathf.Sin(Time.time); // just to test
		}
		*/
		int iterator = 0;
		foreach(Transform child in transform)
		{
			vertices[iterator] = child.transform.position;
		}

		mesh.vertices = vertices;

		audio_source.GetSpectrumData(spectrum_samples, 0, FFTWindow.Rectangular);
		frequency_groups = calculate_frequency_groups();
		audio_source.transform.position = transform.position;
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
		// octant 0: lower front left
		for(int i = 0; i < width / 2; i++)
		{
			for(int j = 0; j < height / 2; j++)
			{
				for(int k = 0; k < depth / 2; k++)
				{
					if(frequency_groups[0] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[0]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		// octant 1, back lower left
		for(int i = 0; i < width / 2; i++)
		{
			for(int j = 0; j < height / 2; j++)
			{
				for(int k = depth / 2; k < depth; k++)
				{
					if(frequency_groups[1] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[1]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		 
		// octant 2, front lower right
		for(int i = width / 2; i < width; i++)
		{
			for(int j = 0; j < height / 2; j++)
			{
				for(int k = 0; k < depth / 2; k++)
				{
					if(frequency_groups[2] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[2]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		
		// octant 3, back lower right
		for(int i = width / 2; i < width; i++)
		{
			for(int j = 0; j < height / 2; j++)
			{
				for(int k = depth / 2; k < depth; k++)
				{
					if(frequency_groups[3] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[3]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		
		// octant 4, front top left
		for(int i = 0; i < width / 2; i++)
		{
			for(int j = height / 2; j < height; j++)
			{
				for(int k = 0; k < depth / 2; k++)
				{
					if(frequency_groups[4] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[4]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		
		// octant 5, back top left
		for(int i = 0; i < width / 2; i++)
		{
			for(int j = height / 2; j < height; j++)
			{
				for(int k = depth / 2; k < depth; k++)
				{
					if(frequency_groups[5] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[5]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		
		// octant 6, front lower right
		for(int i = width / 2; i < width; i++)
		{
			for(int j = height / 2; j < height; j++)
			{
				for(int k = 0; k < depth / 2; k++)
				{
					if(frequency_groups[6] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[6]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
				}
			}
		}

		
		// octant 7, back lower right
		for(int i = width / 2; i < width; i++)
		{
			for(int j = height / 2; j < height; j++)
			{
				for(int k = depth / 2; k < depth; k++)
				{
					if(frequency_groups[7] > 0.2f)
					{
						Vector3 e = anchor_points[i, j, k] - audio_source.transform.position;
						float length = Vector3.Magnitude(e);
						e.Normalize();
						float intensity_fudge = Random.Range(0.8f, 1.2f);
						point_mass[i, j, k].position = (anchor_points[i, j, k] + e * length * frequency_groups[7]) * intensity_fudge;
					}

					else
					{
						point_mass[i, j, k].position = anchor_points[i, j, k];
					}
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
			f_group[i] = average * 2;
		}


		return f_group;
	}

}
