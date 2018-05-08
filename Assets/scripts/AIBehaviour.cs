using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TO BE ATTACHED TO THE CROWD CLASS
// SHOULD NOT INTERNALLY REFERENCE CROWD CLASS IN ANY WAY 

public class AIBehaviour : MonoBehaviour
{
	public Agent agent_prefab;
	public List<Agent> agents;
	public int number_agents_to_spawn;
	public float velocity;
	public float k_spring;
	public float agent_radius;
	public float distance_between_agents;
	public float starting_velocity;

private float s = 0.1f;
	private float timeSinceShoot = 0.0f;
	public Transform laser;
	
	public GameObject player;
	public GameObject home_base;
	public GameObject enemy_base;

	private Vector3 average_position;

	void Start()
	{
		agents = new List<Agent>();
		average_position = new Vector3(0, 0, 0);

		for(int i = 0; i < number_agents_to_spawn; i++)
		{
			Vector3 agent_position = new Vector3(transform.position.x + i * distance_between_agents * Random.value,
												transform.position.y + i * distance_between_agents * Random.value,
												transform.position.z + i * distance_between_agents * Random.value);
			
			Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
			tmp_agent.transform.parent = transform;
			tmp_agent.initialize(10000, agent_radius);
			tmp_agent.rb.velocity = new Vector3(starting_velocity, starting_velocity, starting_velocity);
			agents.Add(tmp_agent);
		}
	}

	void Update()
	{
		average_position = Vector3.zero;
		for(int i = 0; i < agents.Count; i++)
		{
			average_position += agents[i].transform.position;
		}

		
		calculate_separation_force();
		calculate_cohesion_force();
		calculate_player_attraction_force();
		calculate_obstacle_force();

		for(int i = 0; i < agents.Count; i++)
		{
			if(agents[i].rb.velocity.magnitude > 20.0f)
			{
				agents[i].rb.velocity = 20.0f * Vector3.Normalize(agents[i].rb.velocity);
			}
		}
	}

	private void calculate_separation_force()
	{
		for(int i = 0; i < agents.Count; i++)
		{
			for(int j = 0; j < agents.Count; j++)
			{
				if(agents[i] != agents[j])
				{
					float distance_between_agents = Vector3.Magnitude(agents[i].transform.position - agents[j].transform.position);

					if(distance_between_agents <= 2.0f)
					{
						float force = -1.0f * k_spring * (1.0f / distance_between_agents);
						Vector3 unit_vec_i = Vector3.Normalize(agents[i].transform.position - average_position);
						Vector3 unit_vec_j = Vector3.Normalize(agents[j].transform.position - average_position);
						agents[i].rb.velocity += 20f * (force / agents[i].rb.mass) * unit_vec_i * Time.deltaTime * Time.deltaTime;
						agents[j].rb.velocity -= 20f * (force / agents[j].rb.mass) * unit_vec_j * Time.deltaTime * Time.deltaTime;
					}
				}
			}
		}
	}

	private void calculate_player_attraction_force()
	{
		bool can_see = can_see_player();
		if(can_see)
		{
			for(int i = 0; i < agents.Count; i++)
			{
				float dist_agents_to_player = Vector3.Magnitude(agents[i].transform.position - player.transform.position);
				float force = 1.0f * k_spring * (1.0f / dist_agents_to_player);
				Vector3 unit_vec_i = Vector3.Normalize(player.transform.position - agents[i].transform.position);
				agents[i].rb.velocity += 100f * (force / agents[i].rb.mass) * unit_vec_i * Time.deltaTime;
			}
		}

		else
		{
			for(int i = 0; i < agents.Count; i++)
			{
				float dist_agents_to_enemy_base = Vector3.Magnitude(agents[i].transform.position - enemy_base.transform.position);
				float force = 1.0f * k_spring * (1.0f / dist_agents_to_enemy_base);
				Vector3 unit_vec_i = Vector3.Normalize(enemy_base.transform.position - agents[i].transform.position);
				agents[i].rb.velocity += 12 * (force / agents[i].rb.mass) * unit_vec_i * Time.deltaTime;
			}
		}
		
	}

	private void calculate_cohesion_force()
	{
		for(int i = 0; i < agents.Count; i++)
		{
			float dist_from_agent_to_average_position = Vector3.Magnitude(average_position - agents[i].transform.position);
			
			if(dist_from_agent_to_average_position > 0.6f + agent_radius)
			{
				float force = 1.0f * k_spring * (1.0f / (dist_from_agent_to_average_position - agent_radius));
				Vector3 unit_vec_i = Vector3.Normalize(agents[i].transform.position - average_position);
				agents[i].rb.velocity += 12 * (force / agents[i].rb.mass) * unit_vec_i * Time.deltaTime * Time.deltaTime;
			}
		}
	}

	private void calculate_obstacle_force()
	{

	}


	private void fire_at_player()
	{
		if(can_see_player())
		{
			if(timeSinceShoot > 0.05f)
			{
				timeSinceShoot = 0.0f;
				Transform obj1 = Instantiate(laser, transform.position, transform.rotation);
				Transform p = gameObject.transform;
				obj1.transform.parent = p;
				obj1.transform.localPosition = new Vector3 (2.3f, s, 2);
				Transform obj2 = Instantiate(laser, transform.position, transform.rotation);
				obj2.transform.parent = p;
				obj2.transform.localPosition = new Vector3 (-2.3f, s, 2);
			}
		}

		timeSinceShoot += Time.deltaTime;
	}

	private void perform_rotations()
	{
		if(can_see_player())
		{
			Vector3 new_dir = Vector3.RotateTowards(transform.forward, player.transform.position, 0.2f, 0.0f);
			transform.rotation = Quaternion.LookRotation(new_dir);
		}

		else
		{
			Vector3 new_dir = Vector3.RotateTowards(transform.forward, enemy_base.transform.position, 0.2f, 0.0f);
			transform.rotation = Quaternion.LookRotation(new_dir);
		}

	}

	private bool can_see_player()
	{
		RaycastHit hit;

		for(int i = 0; i < agents.Count; i++)
		{
			if(!Physics.SphereCast(agents[i].transform.position, agent_radius,
								(agents[i].transform.position - player.transform.position).normalized,
								out hit))
			{
				//Debug.Log("I CAN SEEEEEE");
				return true;
			}
		}
		//Debug.Log("i cannot see");
		return false;
	}
}
