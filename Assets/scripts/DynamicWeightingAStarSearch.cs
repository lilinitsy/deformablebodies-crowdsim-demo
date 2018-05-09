using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWeightingAStarSearch
{ 
	public float default_weight;
	public DynamicWeightingAStarSearch()
	{

	}

	public List<GraphNode> find_path(List<GraphNode> graph, GraphNode start, GraphNode goal)
	{
		PriorityQueue fringe = new PriorityQueue();
		GraphQueue explored = new GraphQueue();
		GraphQueue path = new GraphQueue();

		fringe.push(start, start, 0.0f, default_weight);

		while(fringe.child.Count > 0)
		{
			GraphNode current = fringe.child[0];
			float current_g = fringe.gscore[0]; // mothafucka this is always 0
			float weight = calculate_weight();

			if(current == goal)
			{
				GraphNode parent = fringe.parent[0];
				path.push(goal, parent, current_g, weight);
				return reconstruct_path(path);
			}

			if(explored.child.Count == graph.Count - 1)
			{
				if(path.child[path.child.Count - 1] != start)
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

		return reconstruct_path(path);
	}

	public List<GraphNode> reconstruct_path(GraphQueue path)
	{
		List<GraphNode> final_path = new List<GraphNode>();

		for(int i = 0; i < path.child.Count; i++)
		{
			final_path.Add(path.child[i]);
		}

		// final_path is correct, but it isn't returning properly...
		return final_path;
	}

	private float calculate_weight()
	{
		return 1.4f;
	} 
}
