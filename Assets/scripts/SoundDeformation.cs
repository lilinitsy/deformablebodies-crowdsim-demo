using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDeformation : MonoBehaviour {
	public PointMass pointmass_prefab;	
	public AudioSource audio_source; // DO NOT attach an audiosource to the jelly; attach it here so I can get a distance


	public float rest_length;
	public float dt;
	public float k_dampening;
	public int width; // x
	public int height; // y
	public int depth; // z

	public float max_time_update;
	private float next_sound_update_time = 0;
	private float average = 0;

	public Vector3[] new_vertices;
	public Vector2[] new_uv;
	public int[] new_triangles; // ibo


	public float[] spectrum_samples;
	private float[] frequency_groups;
	private List<PointMass> point_mass_list;
	private PointMass[ , , ] point_mass;
	private Vector3[ , , ] old_point_mass_positions;
	private Vector3[ , , ] anchor_points;
	private Vector3[ , , ] goal_position;

	MeshRenderer mesh_renderer;
	MeshFilter mesh_filter;
	Mesh mesh;


	void Start() 
	{

		new_vertices = new Vector3[width * height * depth];
		new_uv = new Vector2[width * height * depth];
		new_triangles = new int[width * height * depth * 6];

		point_mass_list = new List<PointMass>();
		point_mass = new PointMass[width, height, depth];
		old_point_mass_positions = new Vector3[width, height, depth];
		anchor_points = new Vector3[width, height, depth];
		goal_position = new Vector3[width, height, depth];

		spectrum_samples = new float[512];
		frequency_groups = new float[8];

		mesh_renderer = gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		mesh_filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;

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
		iterator = 0;
		for(int i = 0; i < width-1; i++)
		{
			for(int j = 0; j < height-1; j++)
			{
				for (int k = 0; k < depth; k++) 
				{
					new_triangles [iterator] = (k*width*height)+(j * width) + i;
					new_triangles [iterator + 1] = (k*width*height)+(j * width) + i + width;
					new_triangles [iterator + 2] = (k*width*height)+(j * width) + i + width + 1; 
					new_triangles [iterator + 3] = (k*width*height)+(j * width) + i;
					new_triangles [iterator + 4] = (k*width*height)+(j * width) + i + width + 1;
					new_triangles [iterator + 5] = (k*width*height)+(j * width) + i + 1;
					iterator += 6;
				}
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = new_triangles;

		audio_source.GetSpectrumData(spectrum_samples, 0, FFTWindow.Rectangular);
		audio_source.transform.position = transform.position;

		if(next_sound_update_time < max_time_update / 2.0f)
		{
			for(int i = 0; i < width; i++)
			{
				for(int j = 0; j < height; j++)
				{
					for(int k = 0; k < depth; k++)
					{
						float step = Vector3.Magnitude(point_mass[i, j, k].position - goal_position[i, j, k]) * Time.deltaTime;
						point_mass[i, j, k].position = Vector3.MoveTowards(point_mass[i, j, k].position, goal_position[i, j, k], step);
					}
				}
			}
		}

		else
		{
			for(int i = 0; i < width; i++)
			{
				for(int j = 0; j < height; j++)
				{
					for(int k = 0; k < depth; k++)
					{
						float step = Vector3.Magnitude(point_mass[i, j, k].position - anchor_points[i, j, k]) * Time.deltaTime;
						point_mass[i, j, k].position = Vector3.MoveTowards(point_mass[i, j, k].position, anchor_points[i, j, k], step);
					}
				}
			}

		}

		if(next_sound_update_time > max_time_update)
		{
			frequency_groups = calculate_frequency_groups();
			average = 0.0f;
			next_sound_update_time = 0.0f;
		}
		// Debug.Log("Fps: " + 1 / Time.deltaTime);
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

		sound_deformation();

		next_sound_update_time += Time.deltaTime;
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

	private void render_mesh()
	{
		
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

	private void sound_deformation()
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[0]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[1]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[2]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[3]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[4]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[5]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[6]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
						goal_position[i, j, k] = (anchor_points[i, j, k] + e * length * frequency_groups[7]) * intensity_fudge;
					}

					else
					{
						goal_position[i, j, k] = anchor_points[i, j, k];
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
					
					float v2 = Vector3.Dot(e, goal_position[i, j, k]);
					
					float force = -1.0f * k_dampening * (v1 - v2);
					
					goal_position[i, j, k] += e * (Time.deltaTime * Time.deltaTime) * force;

					/* 
					if(v1 != v2)
					{
						Debug.Log("v1: " + v1);
						Debug.Log("v2: " + v2);
						Debug.Log("force: " + force.ToString("F8"));	
					} */
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
			f_group[i] = average * 3;
		}


		return f_group;
	}
}
