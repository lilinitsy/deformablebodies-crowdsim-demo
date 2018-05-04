using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPAStarSearch
{
	public List<GraphNode> graph;
	public GraphNode start;
	public GraphNode goal;
	public GraphQueue path;
	public GraphQueue explored;
	public PriorityQueue fringe;	

	public LPAStarSearch()
	{
		path = new GraphQueue();
		graph = new List<GraphNode>();
		explored = new GraphQueue();
		fringe = new PriorityQueue();
		start = new GraphNode();
		goal = new GraphNode();
	}

	public List<GraphNode> update(List<GraphNode> nodes, GraphNode s, GraphNode g)
	{
		start = s;
		goal = g;

		compute_shortest_path();

		return reconstruct_graph();
	}

	public void compute_shortest_path()
	{
		path.child.Clear();
		path.parent.Clear();
		goal.calculate_key();
		// refer to calculate key for instructions on how to compare if a key is less than another.


		while(fringe.child[0].key.x < goal.key.x || 
			(fringe.child[0].key.x == goal.key.x && 
				(fringe.child[0].key.y < goal.key.y)) ||
		 	goal.rhs != goal.g) 
		{
			GraphNode current = fringe.child[0];
			path.push(fringe.child[0], fringe.parent[0]);
			
			if(fringe.child.Count > 1) // k...
			{			
				fringe.pop();
			}

			else
			{
				return;
			}


			if(current.g > current.rhs) // overconsistent
			{
				current.g = current.rhs; // to make this consistent
			}

			else
			{
				current.g = Mathf.Infinity; // make this either overconsistent or consistent
			}

			for(int i = 0; i < current.neighbours.Count; i++)
			{
				// GraphNode neighbour = current.neighbours[i];
				// update_vertex(ref neighbour, current);
				// current.neighbours[i] = neighbour;
				update_vertex(current.neighbours[i], current);
			}
		}
	}

	public void update_vertex(GraphNode node, GraphNode parent)
	{
		if(node != start)
		{
			float min_rhs = Mathf.Infinity;

			for(int i = 0; i < node.neighbours.Count; i++)
			{
				float tmp_rhs = node.neighbours[i].g + Vector2.SqrMagnitude(node.position - node.neighbours[i].position);
				if(tmp_rhs < min_rhs)
				{
					min_rhs = tmp_rhs;
				}
			}
			node.rhs = min_rhs;
		}

		if(fringe.node_in_queue(node))
		{
			fringe.remove(node);
		}

		if(node.g != node.rhs)
		{
			node.calculate_key();
			fringe.push(node, parent);
		}
	}

	public void initialize(List<GraphNode> nodes, GraphNode s, GraphNode g)
	{ /* 
		graph.Clear();
		fringe.child.Clear();
		fringe.parent.Clear();
		path.child.Clear();
		path.parent.Clear();
		explored.child.Clear();
		explored.parent.Clear();
*/
		for(int i = 0; i < nodes.Count; i++)
		{
			if(nodes[i] != s && nodes[i] != g)
			{
				graph.Add(nodes[i]);
			}
		}

		for(int i = 0; i < graph.Count; i++)
		{
			graph[i].g = Mathf.Infinity;
			graph[i].rhs = Mathf.Infinity;
		}

		start = s;
		goal = g;

		start.rhs = 0;
		start.calculate_key();
		graph.Add(start);
		graph.Add(goal);
		fringe.push(start, start);
		//fringe.push(goal, goal);
		path.push(start, start);
	}


	private List<GraphNode> reconstruct_graph()
	{
		List<GraphNode> final_path = new List<GraphNode>();
		for(int i = 0; i < path.child.Count - 1; i++)
		{
			final_path.Add(path.child[i]);
		}
		
		return final_path;
	}
}

