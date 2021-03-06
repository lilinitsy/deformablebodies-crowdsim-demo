﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This method does not look as good as SoundDeformation(1)
public class SoundDeformation2 : MonoBehaviour 
{
	public PointMass pointmass_prefab;	

	public AudioSource audio_source; // DO NOT attach an audiosource to the jelly; attach it here so I can get a distance

	public Vector3 position;
	public Vector3 average_position;
	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles; // ibo

	public float rest_length;
	public float dt;
	public float k_dampening;
	public int width; // x
	public int height; // y
	public int depth; // z

	public float[] spectrum_samples;
	private float[] frequency_groups;
	private List<PointMass> point_mass_list;
	private PointMass[ , , ] point_mass;
	private Vector3[ , , ] old_point_mass_positions;
	private Vector3[ , , ] anchor_points;


	void Start() 
	{
		average_position = new Vector3(0, 0, 0);
		new_vertices = new Vector3[width * height * depth];
		new_uv = new Vector2[width * height * depth];
		new_triangles = new int[width * height * depth * 18 * 2];

		point_mass_list = new List<PointMass>();
		point_mass = new PointMass[width, height, depth];
		old_point_mass_positions = new Vector3[width, height, depth];
		anchor_points = new Vector3[width, height, depth];

		spectrum_samples = new float[512];
		frequency_groups = new float[8];

		init();

		int iterator = 0;
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					new_vertices[iterator] = point_mass[i, j, k].position;
					iterator++;
				}
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.Clear();
		mesh.vertices = new_vertices;
		mesh.uv = new_uv; // not gonna fuck with textures for now
		mesh.triangles = new_triangles;

	}

	void Update() 
	{
		// Modifying vertex attributes
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		mesh.Clear();

		int iterator = 0;
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					vertices[iterator] = point_mass[i, j, k].position;
					point_mass_list[iterator] = point_mass[i, j, k];
					iterator++;
				}
			}
		}

		// trying front face, k = 0;
		for(int i = 0; i < width-1; i++)
		{
			for(int j = 0; j < height-1; j++)
			{
				for (int k = 0; k < depth-1; k++) 
				{
					int index = (k * width * height) + (j * width) + i;
					new_triangles [iterator] = index;
					new_triangles [iterator + 1] = index + width + 1;
					new_triangles [iterator + 2] = index + width;

					new_triangles [iterator + 3] = index;
					new_triangles [iterator + 4] = index + 1;
					new_triangles [iterator + 5] = index + width + 1;

					new_triangles [iterator + 6] = index;
					new_triangles [iterator + 7] = index + (width*height);
					new_triangles [iterator + 8] = index + (width*height) + 1;

					new_triangles [iterator + 9] = index;
					new_triangles [iterator + 10] = index + (width*height) + 1;
					new_triangles [iterator + 11] = index + 1;

					new_triangles [iterator + 12] = index;
					new_triangles [iterator + 13] = index + (width*height) + width;
					new_triangles [iterator + 14] = index + (width*height);

					new_triangles [iterator + 15] = index;
					new_triangles [iterator + 16] = index + width;
					new_triangles [iterator + 17] = index + (width*height) + width;
					iterator += 18;
				}
			}
		}
		for(int i = width-1; i > 0; i--)
		{
			for(int j = height-1; j > 0; j--)
			{
				for (int k = depth-1; k > 0; k--) 
				{
					int index = (k * width * height) + (j * width) + i;
					new_triangles [iterator] = index;
					new_triangles [iterator + 1] = index - width;
					new_triangles [iterator + 2] = index - width - 1;

					new_triangles [iterator + 3] = index;
					new_triangles [iterator + 4] = index - width - 1;
					new_triangles [iterator + 5] = index - 1;

					new_triangles [iterator + 6] = index;
					new_triangles [iterator + 7] = index - (width*height) - 1;
					new_triangles [iterator + 8] = index - (width*height);

					new_triangles [iterator + 9] = index;
					new_triangles [iterator + 10] = index - 1;
					new_triangles [iterator + 11] = index - (width*height) - 1;

					new_triangles [iterator + 12] = index;
					new_triangles [iterator + 13] = index - (width*height);
					new_triangles [iterator + 14] = index - (width*height) - width;

					new_triangles [iterator + 15] = index;
					new_triangles [iterator + 16] = index - (width*height) - width;
					new_triangles [iterator + 17] = index - width;
					iterator += 18;
				}
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = new_triangles;
		mesh.RecalculateNormals();


		for(int i = 0; i < vertices.Length; i++)
		{
			Debug.Log("Mesh vertices " + i + ": " + mesh.vertices[i].ToString("F4"));
		}

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					old_point_mass_positions[i, j, k] = point_mass[i, j, k].position;
				}
			}
		}

		if(Input.GetKey("p"))
		{
			render_line_mesh();
			// render_mesh();
		}

		iterator = 0;
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
					point_mass.position = new Vector3(2 * i * rest_length, 2 * j * rest_length, 2 * k * rest_length) + transform.position;
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

	private void render_line_mesh()
	{
		for(int i = 0; i < width - 1; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					Debug.DrawLine(point_mass[i, j, k].position, point_mass[i + 1, j, k].position, Color.black);
				}
			}
		}

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height - 1; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					Debug.DrawLine(point_mass[i, j, k].position, point_mass[i, j + 1, k].position, Color.blue);
				}
			}
		}

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth - 1; k++)
				{
					Debug.DrawLine(point_mass[i, j, k].position, point_mass[i, j, k + 1].position, Color.gray);
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
					if(frequency_groups[0] > 0.15f)
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

		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					Vector3 e = audio_source.transform.position - old_point_mass_positions[i, j, k];
					float l = Vector3.Magnitude(e);
					e.Normalize();
					float v1 = Vector3.Dot(e, anchor_points[i, j, k]);
					float v2 = Vector3.Dot(e, point_mass[i, j, k].position);
					float force = -1.0f * k_dampening * (v1 - v2);
					point_mass[i, j, k].position += e * (Time.deltaTime * Time.deltaTime);
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
			f_group[i] = average;
		}


		return f_group;
	}

}