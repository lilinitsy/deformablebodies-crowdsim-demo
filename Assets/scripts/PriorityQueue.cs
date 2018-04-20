using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue : MonoBehaviour 
{
	// child[0] will have parent[0]
	// child[n] will have parent[n]
	List<GraphNode> child;
	List<GraphNode> parent;

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
		float key1 = c.key.x;
		float key2 = c.key.y;
		Vector2 key = new Vector2(key1, key2);

		// For each node, compare based on the LPA* key guidelines
		for(int i = 0; i < child.Count; i++)
		{
			if(key.x < child[i].key.x ||
				(key.x == child[i].key.x &&
				key.y < child[i].key.y))
			{
				child.Insert(i, c);
				parent.Insert(i, p);
				return;
			}
		}

		/*
			If it needs to be inserted at the end, insert
			at count - 1 to alias as the end
		*/
		int count = child.Count;
		child.Insert(count - 1, c);
		child.Insert(count - 1, p);
	}

	public void pop()
	{
		child.RemoveAt(0);
		parent.RemoveAt(0);
	}

	public void remove(GraphNode n)
	{
		/* 
			can't really use built-in list remove/remove_at
			because I have to do it with two lists

			Could maybe just use Remove? but I need to get the parent
			index, and the built-ins are all voids
		*/

		for(int i = 0; i < child.Count; i++)
		{
			if(n == child[i])
			{
				child.RemoveAt(i);
				parent.RemoveAt(i);
			}
		}
	}

	public bool node_in_queue(GraphNode n)
	{
		if(child.Contains(n))
		{
			return true;
		}

		return false;
	}
}
