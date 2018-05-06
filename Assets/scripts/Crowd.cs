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
	public float astar_weight;
	
	public float vision_distance;
	public float agent_radius;
	public float random_distance_for_agent_spawns;
	public Vector3 global_goal_position;

	private GraphNode goal;
	private List<GraphNode> graph;
	private List<GraphNode> path;	
	private DynamicWeightingAStarSearch search;

	// Use this for initialization
	void Start() 
    {
		agents = new List<Agent>();
		graph = new List<GraphNode>();
		path = new List<GraphNode>();
		goal = new GraphNode(global_goal_position, 0.0f);
		search = new DynamicWeightingAStarSearch();
		search.default_weight = astar_weight;

		for(int i = 0; i < number_agents_to_spawn; i++)
		{
			Vector3 agent_position = new Vector3(transform.position.x + random_distance_for_agent_spawns * Random.value,
												transform.position.y + random_distance_for_agent_spawns * Random.value,
												transform.position.z + random_distance_for_agent_spawns * Random.value);
			
			Agent tmp_agent = Instantiate(agent_prefab, agent_position, Quaternion.identity) as Agent;
			tmp_agent.transform.parent = transform;
			tmp_agent.initialize(vision_distance, agent_radius);
			
			agents.Add(tmp_agent);
		}

		construct_graph(num_nodes_sample);
		make_graph_neighbours(global_goal_position);
		
		Debug.Log("Path to begin with size: " + path.Count);
		for(int i = 0; i < path.Count; i++)
		{
			Debug.Log("Path " + i + ": " + path[0].position.ToString("F4"));
		}

		GraphNode start = new GraphNode(transform.position, Vector2.Distance(transform.position, goal.position));
		search.graph = graph;

		path = search.find_path(start, goal);

		Debug.Log("Path after with size: " + path.Count);
		for(int i = 0; i < path.Count; i++)
		{
			Debug.Log("Path " + i + ": " + path[0].position.ToString("F4"));
		}

		for(int i = 0; i < search.graph.Count; i++)
		{
			Debug.Log("search Graph position " + search.graph[i].position.ToString("F4") +
						" neighbour num: " + search.graph[i].neighbours.Count);
		} 
	}
	
	// Update is called once per frame
	void Update() 
    {
		Vector3 average_position = new Vector3(0.0f, 0.0f, 0.0f);
		foreach(Transform child in transform)
		{
			average_position += child.transform.position; 
		}

		average_position /= agents.Count;
	//	transform.position = average_position;
		
		for(int i = 0; i < graph.Count; i++)
		{
			for(int j = 0; j < graph[i].neighbours.Count; j++)
			{
				Debug.DrawRay(graph[i].position, graph[i].neighbours[j].position, Color.blue);
			}
		}



		calculate_avoidance_forces();
	}


	public void construct_graph(int num_nodes)
	{

		for(int i = 0; i < agents.Count; i++)
		{
			List<GraphNode> agent_graph = agents[i].sample_points(num_nodes, global_goal_position);
			for(int j = 0; j < agent_graph.Count; j++)
			{
				graph.Add(agent_graph[j]);
			}
		}

		GraphNode start = new GraphNode(transform.position, Vector2.Distance(transform.position, goal.position));
		graph.Add(start);
		graph.Add(goal);
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
										(graph[j].position - node_position).normalized,
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
				Debug.Log("\tNeighbour " + j + ": " + graph[i].neighbours[j].position);
			}
		}
	}
}