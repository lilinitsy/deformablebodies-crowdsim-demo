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
	public List<Agent> agents;
	public int number_agents_to_spawn;
	public int num_nodes_sample;

	public float vision_distance;
	public float agent_radius;
	public float random_distance_for_agent_spawns;
	public Vector3 goal_position;

	private List<GraphNode> graph;


	// Use this for initialization
	void Start() 
    {
		agents = new List<Agent>();
		graph = new List<GraphNode>();

		for(int i = 0; i < number_agents_to_spawn; i++)
		{
			Vector3 agent_position = new Vector3(transform.position.x + random_distance_for_agent_spawns * Random.value,
												transform.position.y + random_distance_for_agent_spawns * Random.value,
												transform.position.z + random_distance_for_agent_spawns * Random.value);
			
			Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
			tmp_agent.transform.parent = transform;
			tmp_agent.initialize(vision_distance, agent_radius);
			
			agents.Add(tmp_agent);
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
		construct_graph(num_nodes_sample, goal_position);









		calculate_avoidance_forces();
	}


	public void construct_graph(int num_nodes, Vector3 global_goal_position)
	{
		Vector3 average_position = new Vector3(0.0f, 0.0f, 0.0f);

		for(int i = 0; i < agents.Count; i++)
		{
			List<GraphNode> agent_graph = agents[i].sample_points(num_nodes, goal_position);
		}
	}


	// TODO: if time
	private void calculate_avoidance_forces()
	{

	}
}
