using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableBody : MonoBehaviour {
	public PointMass pointmass_prefab;

	public AudioSource audio_source; // DO NOT attach an audiosource to the jelly; attach it here so I can get a distance
	public AudioSource audio_source2;
	public AudioSource audio_source3;
	public AudioSource audio_source4;

	public Vector3 position;
	public Vector3 average_position;
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

	private float[] spectrum_samples;
	private float[] frequency_groups;
	private List<PointMass> point_mass_list;
	private PointMass[ , , ] tmp_intermediate_pointmass;
	private PointMass[ , , ] old_pointmass;
	private PointMass[ , , ] point_mass;

/*****************************************************************************
http://wiki.roblox.com/index.php?title=Verlet_integration
could be useful if I want to try Verlet again...
Also might want to consider Implicit Euler instead.
*****************************************************************************/
	// Use this for initialization
	void Start() 
	{
				Debug.Log("-: " + -1 * width / 2);
		Debug.Log("+: " + width / 2);

		transform.position = position;
		// before making the mesh, 
		// get the vertices from all the child pointmasses or some shit
// 			Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
		average_position = new Vector3(0, 0, 0);

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = new_vertices;
		mesh.uv = new_uv; // not gonna fuck with textures for now
		mesh.triangles = new_triangles;

		point_mass_list = new List<PointMass>();
		tmp_intermediate_pointmass = new PointMass[width, height, depth];
		old_pointmass = new PointMass[width, height, depth];
		point_mass = new PointMass[width, height, depth];

		spectrum_samples = new float[256];
		frequency_groups = new float[8];

		init();
	}
	
	// check out http://www.kaappine.fi/tutorials/using-microphone-input-in-unity3d/
	// Update is called once per frame
	// Thiiiiink the outtput data is what is needed... but not sure.
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
		// gameObject.transform.GetChild(0).transform.position = average_position;


		// like cloth now...

		spring_force_calculations(dt);
	}

	private void init()
	{
		for(int i = -1 * width / 2; i < width / 2 + 1; i++)
		{
			for(int j = -1 * width / 2; j < height / 2 + 1; j++)
			{
				for(int k = -1 * width / 2; k < depth / 2 + 1; k++)
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
					Debug.Log("Sample z force: " + force.ToString("F8"));

					point_mass[i, j, k].rb.velocity += (force / mass) * e * delta_time;
					point_mass[i, j, k + 1].rb.velocity -= (force / mass) * e * delta_time;
				}
			}
		}

		// do this here so it's done only once per frame
		// I think GetSpectrumData holds frequencies? ie notes
		/* 	44100 hz song
			256 samples

			Can make different bands?

		
		*/
		audio_source.GetSpectrumData(spectrum_samples, 0, FFTWindow.Rectangular);
		determine_frequency_groups();
		

		
		// sound forces - will do by octants, I suppose?

		// octant 0, front lower left
		for(int i = 0; i < width / 2; i++)
		{
			for(int j = 0; j < height / 2; j++)
			{
				for(int k = 0; k < depth / 2; k++)
				{
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[0], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[1], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[2], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[3], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[0], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[1], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[2], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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
					Vector3 e1 = audio_source.transform.position - point_mass[i, j, k].position;
					Vector3 e2 = audio_source2.transform.position - point_mass[i, j, k].position;
					Vector3 e3 = audio_source3.transform.position - point_mass[i, j, k].position;
					Vector3 e4 = audio_source4.transform.position - point_mass[i, j, k].position;
					Vector3 e = e1 + e2 + e3 + e4;
					float l = Vector3.Magnitude(e); // confirmed functioning
					e.Normalize();

					float force = sound_force_calculations(point_mass[i, j, k].position, frequency_groups[3], l);
					point_mass[i, j, k].rb.velocity +=  (force / mass) * e * delta_time;
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

	/*
		Have to do something with the sound energy force...
		Frequency: mostly determines pitch
		Use AudioClip.clip from https://docs.unity3d.com/ScriptReference/AudioSource.html
		AudioClip functions will then be useful https://docs.unity3d.com/ScriptReference/AudioClip.html
		AudioClip.frequency only gives the frequency everything was recorded in, not the sound data shit...
	*/

	private float sound_force_calculations(Vector3 position, float frequency, float l)
	{
		return -1.0f * frequency / l;
	}

	private void determine_frequency_groups()
	{
		// Probably need to use GetSpectrumData https://www.youtube.com/watch?v=4Av788P9stk
		int count = 0;
		for(int i = 0; i < 4; i++) // maybe need to do i < 5?
		{
			float average = 0.0f;
			int sample_count = (int) Mathf.Pow(2, i) * 16;

			for(int j = 0; j < sample_count; j++)
			{
				average += spectrum_samples[count] * (count + 1);
				count++;
			}

			average /= count;
			frequency_groups[i] = average * 10;
			Debug.Log("frequency group " + i + ": " + frequency_groups[i]);
		}
	}

	void to_string()
	{

	}
}
