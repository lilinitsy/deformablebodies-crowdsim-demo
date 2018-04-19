using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SHOULD HAVE AN AIBehaviour
// SHOULD HAVE WHATEVER C#'s VECTOR IS OF AGENTS

/*
 *  @brief 
 * 
 * 
 * 
*/


public class Crowd : MonoBehaviour 
{
	public Agent agent_prefab;
	public Agent[] agents;
	List<GraphNode> graph;
	public int number_agents_to_spawn;
	public float vision_distance;
	public float agent_radius;
	public float random_distance_for_agent_spawns;

	// Use this for initialization
	void Start() 
    {
		agents = new Agent[number_agents_to_spawn];
		graph = new List<GraphNode>();

		for(int i = 0; i < number_agents_to_spawn; i++)
		{
			Vector3 agent_position = new Vector3(transform.position.x + random_distance_for_agent_spawns * Random.value,
												transform.position.y + random_distance_for_agent_spawns * Random.value,
												transform.position.z + random_distance_for_agent_spawns * Random.value);
			
			agents[i] = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
			agents[i].transform.parent = transform;
			agents[i].initialize(vision_distance, agent_radius);
		//	agents[i].initialize(vision_distance, agent_radius, agent_position);
		} // new isn't the right way to do it, do some shit with prefabs or smth?

        foreach(Transform child in transform)
        {
            Debug.Log(child.transform.position);
        }
	}
	
	// Update is called once per frame
	void Update() 
    {
		










		calculate_avoidance_forces();
	}

	// TODO: if time
	private void calculate_avoidance_forces()
	{

	}
}
