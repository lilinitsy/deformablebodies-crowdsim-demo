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
	public float velocity;
	public float vision_distance;
	public float agent_radius;
	public float random_distance_for_agent_spawns;
	public Vector3 global_goal_position;

	public DynamicWeightingAStarSearch search;

	private GraphNode goal;
	private GraphNode start;
	private List<GraphNode> graph;
	private List<GraphNode> path;	

	// Use this for initialization
	void Start() 
    {
		agents = new List<Agent>();
		graph = new List<GraphNode>();
		path = new List<GraphNode>();
		goal = new GraphNode(global_goal_position, 0.0f);
		start = new GraphNode(transform.position, Vector3.Magnitude(transform.position - global_goal_position));
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
		path = search.find_path(graph, start, goal); 

		Debug.Log("Path after search with size: " + path.Count);
		for(int i = 0; i < path.Count; i++)
		{
		//	Debug.Log("\tPath " + i + ": " + path[i].position.ToString("F4"));
		}
	}
	
	// Update is called once per frame
	void Update() 
    {
		graph.Clear();
		Debug.Log("Path 0 position: " + path[0].position.ToString("F3"));
		Vector3 average_position = new Vector3(0.0f, 0.0f, 0.0f);
		foreach(Transform child in transform)
		{
			average_position += child.transform.position; 
		}

		average_position /= agents.Count;
		
		for(int i = 0; i < path.Count - 1; i++)
		{
			Debug.DrawLine(path[i].position, path[i + 1].position, Color.blue);
		}

		if(Vector3.Magnitude(average_position - path[0].position) < 0.2f && path.Count > 1)
		{

			Debug.Log("Removed!");
			path.RemoveAt(0);
		}

		if(path.Count > 1)
		{
			smooth_path();
		}

		if(path.Count > 0)
		{
			Vector3 heading = path[0].position - average_position;
			transform.position += velocity * heading * Time.deltaTime;
		}

		if(path.Count == 1 && !path.Contains(goal))
		{
			construct_graph(num_nodes_sample);
			make_graph_neighbours(global_goal_position);
			start.position = transform.position;
			start.heuristic = Vector3.Magnitude(transform.position - global_goal_position);
			path = search.find_path(graph, start, goal); 
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

	
	private void smooth_path()
	{
		if(can_see(path[0]))
		{
			path.RemoveAt(0);
			return;
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

	private bool can_see(GraphNode node_in_path)
	{
		RaycastHit hit;
		if(Physics.SphereCast(transform.position, agent_radius,
										(node_in_path.position - transform.position).normalized,
										out hit))
		{
			return false;
		}

		return true;
	}
}

/*
if(!Physics.SphereCast(graph[i].position, agent_radius,
										(graph[j].position - node_position).normalized,
										out hit))
 */