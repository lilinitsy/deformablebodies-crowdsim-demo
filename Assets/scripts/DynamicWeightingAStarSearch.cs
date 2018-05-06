using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWeightingAStarSearch
{ 
	public float default_weight;
	public List<GraphNode> graph;
	public DynamicWeightingAStarSearch()
	{
		graph = new List<GraphNode>();
	}

	public List<GraphNode> find_path(List<GraphNode> graph, GraphNode start, GraphNode goal)
	{
		PriorityQueue fringe = new PriorityQueue();
		GraphQueue explored = new GraphQueue();
		GraphQueue path = new GraphQueue();

		fringe.push(start, start, 0.0f, default_weight);

		while(fringe.child.Count > 0)
		{
			Debug.Log("Pass of weighted A*");
			GraphNode current = fringe.child[0];
			float current_g = fringe.gscore[0];
			float weight = calculate_weight();

			if(current == goal)
			{
				GraphNode parent = fringe.parent[0];
				Debug.Log("goal found, yay!");
				Debug.Log("Goal position: " + goal.position.ToString("F4"));
				Debug.Log("Parent of goal position: " + parent.position.ToString("F4"));
				path.push(goal, parent, current_g, weight);
				return reconstruct_path(path);
			}

			/*
				If everything else has been explored,
				and there is no path to goal,
				maybe just use the best path AS LONG
				as it doesn't end in the original node

				If it does end in the original start node,
				then use the next best
			*/
			if(explored.child.Count == graph.Count - 1)
			{
				Debug.Log("explored.child.Count was 1 less than graph size?");
				if(path.child[path.child.Count - 1] == start)
				{
					path.push(fringe.child[0], fringe.parent[0], current_g, weight);
				}

				return reconstruct_path(path);
			}

			explored.push(fringe.child[0], fringe.parent[0], fringe.gscore[0], weight);
			path.push(fringe.child[0], fringe.parent[0], fringe.gscore[0], weight);
			fringe.pop();

			for(int i = 0; i < current.neighbours.Count; i++)
			{

				if(explored.node_in_queue(current.neighbours[i]))
				{
					continue;
				}

				float g = Vector3.Magnitude(current.position - current.neighbours[i].position) + current_g;

				if(!fringe.node_in_queue(current.neighbours[i]))
				{
					fringe.push(current.neighbours[i], current, g, weight);
				}

				if(g < current_g)
				{
					if(path.node_in_queue(current.neighbours[i]))
					{
						path.swap_parent_of_child(current.neighbours[i], current);
					}

					else
					{
						path.push(current.neighbours[i], current, g, weight);
					}
				}
			}
		}

		Debug.Log("Reached smth it shouldn't have");
		// this shouldn't ever be reached
		return reconstruct_path(path);
	}

	public List<GraphNode> reconstruct_path(GraphQueue path)
	{
		List<GraphNode> final_path = new List<GraphNode>();

		for(int i = 0; i < path.child.Count; i++)
		{
			final_path.Add(path.child[i]);
			//Debug.Log("Path child " + i);
			//Debug.Log("\t" + path.child[i].position.ToString("F4"));
		}

	
		for(int i = 0; i < final_path.Count; i++)
		{
			Debug.Log("Path child " + i);
			Debug.Log("\t" + path.child[i].position.ToString("F4"));
		}

		// final_path is correct, but it isn't returning properly...
		return final_path;
	}

	private float calculate_weight()
	{
		return 1.0f;
	} 
}
