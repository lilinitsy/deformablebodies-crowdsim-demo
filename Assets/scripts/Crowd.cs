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
		make_graph_neighbours(goal_position);

		/*
			the make graph neighbours function is too slow for C#

			so to get around this, each agent, when it samples, will have
			to make its own mini graph, with the average_position
			passed in to make a node, and to have each agent's position also
			be a node. Then, cfan make a mini graph, only evaluate those neighbours,
			and do it for each eagent. Then, the whole graph can be merged,
			but don't have nodes from Agent n be neighbours of nodes from Agent m.
			Then, LPA* can be run on this graph, and that should be fast enough...

		*/
		to_string();







		calculate_avoidance_forces();
	}


	public void construct_graph(int num_nodes, Vector3 global_goal_position)
	{
		Vector3 average_position = new Vector3(0.0f, 0.0f, 0.0f);

		for(int i = 0; i < agents.Count; i++)
		{
			List<GraphNode> agent_graph = agents[i].sample_points(num_nodes, goal_position);

			for(int j = 0; j < agent_graph.Count; j++)
			{
				graph.Add(agent_graph[j]);
			}
		}
	}

	private void make_graph_neighbours(Vector3 global_goal_position)
	{
		for(int i = 0; i < graph.Count; i++)
		{
			Vector3 node_position = graph[i].position;
			
			for(int j = 0; j < graph.Count; j++)
			{
				if(graph[i] != graph[j])
				{
					RaycastHit hit;

					if(!Physics.SphereCast(graph[i].position, agent_radius,
										(node_position - graph[i].position).normalized,
										out hit))
					{
						graph[i].neighbours.Add(graph[j]);
					}
				}
			}
		}
	}

	// TODO: if time
	private void calculate_avoidance_forces()
	{

	}

	public void to_string()
	{
		for(int i = 0; i < graph.Count; i++)
		{
			Debug.Log("graph " + i + ": " + graph[i].position);
			
			for(int j = 0; j < graph[i].neighbours.Count; j++)
			{
				Debug.Log("Neighbour " + i + ": " + graph[i].neighbours[j].position);
			}
		}
	}
}


/*
	Identifies:
		TV monitor
		TV stand
		Blue barrel: 2
		Red barrel: 4
		Fir tree: 2

	Comments:
		In a featureless gray room

 */