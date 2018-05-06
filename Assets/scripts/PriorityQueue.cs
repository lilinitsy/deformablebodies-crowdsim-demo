using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
	public List<GraphNode> child;
	public List<GraphNode> parent;
	public List<float> gscore;
	public List<float> fscore;
	// Use this for initialization
	public PriorityQueue()
	{
		child = new List<GraphNode>();
		parent = new List<GraphNode>();
		gscore = new List<float>();
		fscore = new List<float>();
	}

	public void push(GraphNode c, GraphNode p, float g, float w)
	{
		float f = g + w * c.heuristic;

		for(int i = 0; i < child.Count; i++)
		{
			if(f < fscore[i])
			{
				child.Add(c);
				parent.Add(p);
				gscore.Add(g);
				fscore.Add(f);
				return;
			}
		}
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

	public bool node_in_queue(GraphNode n)
	{
		for(int i = 0; i < child.Count; i++)
		{
			if(n == child[i])
			{
				return true;
			}
		}

		return false;
	}
}
