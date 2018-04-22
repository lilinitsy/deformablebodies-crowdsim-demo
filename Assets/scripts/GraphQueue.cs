using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphQueue 
{

	// child[0] will have parent[0]
	// child[n] will have parent[n]
	public List<GraphNode> parent;
	public List<GraphNode> child;

	public GraphQueue()
	{
		child = new List<GraphNode>();
		parent = new List<GraphNode>();
	}

	public void push(GraphNode c, GraphNode p)
	{
		child.Add(c);
		parent.Add(p);
	}

	public void pop()
	{
		child.RemoveAt(0);
		parent.RemoveAt(0);	
	}
}
