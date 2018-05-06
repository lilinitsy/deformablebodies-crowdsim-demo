using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphQueue
{
	public List<GraphNode> child;
	public List<GraphNode> parent;
	public List<float> gscore;
	public List<float> fscore;

	// Use this for initialization
	public GraphQueue()
	{
		child = new List<GraphNode>();
		parent = new List<GraphNode>();
		gscore = new List<float>();
		fscore = new List<float>();
	}

	public void push(GraphNode c, GraphNode p, float g, float f, float w)
	{
		child.Add(c);
		parent.Add(p);
		gscore.Add(g);
		f *= w;
		fscore.Add(f);
	}

	public void push(GraphNode c, GraphNode p, float g, float w)
	{
		float f = g + w * c.heuristic;
		child.Add(c);
		parent.Add(p);
		gscore.Add(g);
		fscore.Add(f);
	}

	public void pop()
	{
		child.RemoveAt(0);
		parent.RemoveAt(0);
		gscore.RemoveAt(0);
		fscore.RemoveAt(0);
	}

	public void swap_child(GraphNode node_in_queue, GraphNode node_to_be_placed)
	{
		for(int i = 0; i < child.Count; i++)
		{
			if(child[i] == node_in_queue)
			{
				child[i] = node_to_be_placed;
				return;
			}
		}
	}

	public void swap_parent_of_child(GraphNode c, GraphNode p)
	{
		for(int i = 0; i < child.Count; i++)
		{
			if(child[i] == c)
			{
				parent[i] = p;
				return;
			}
		}
	}

	public bool node_in_queue(GraphNode n)
	{
		for(int i = 0; i < child.Count; i++)
		{
			if(child[i] == n)
			{
				return true;
			}
		}

		return false;
	}

}
