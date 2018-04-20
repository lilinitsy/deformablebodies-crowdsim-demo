using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphQueue : MonoBehaviour 
{

	// child[0] will have parent[0]
	// child[n] will have parent[n]
	List<GraphNode> parent;
	List<GraphNode> child;

	// Use this for initialization
	void Start() 
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		
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
