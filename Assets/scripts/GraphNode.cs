using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      @brief The class that represents each node of the PRM-produced graph.
 *      
 *      @params neighbours List of GraphNode neighbours. Ie, what this node is connected to.
 *      @params heuristic Heuristic of this node (estimated cost from here to goal). Defaults to euclidean distance.
 *      @params is_goal Dictates whether this node is the goal.
 *      @params g g value used in D* Lite and LPA*
 *      @params rhs rhs value used in D* Lite and LPA*
 *      @params key key pair used in D* Lite and LPA*. pair.first = min(g, rhs + heuristic). pair.second = min(g, rhs).
 *                  It's represented as Vector2 because Unity's pair and tuple support isn't well documented, so I'd
 *                  like to avoid them.
 * 
*/

public class GraphNode
{
    public List<GraphNode> neighbours;
    public List<float> costs;
    public Vector3 position;
    public float heuristic;
    public float g;
    public float rhs;

    // key.x is the first value
    // key.y is the second
    // this gets around Unity's lack of Tuples.
    public Vector2 key;
    public bool is_goal;

    public GraphNode()
    {
        position = new Vector3(0, 0, 0);
        heuristic = 1.0f;
        g = Mathf.Infinity;
        rhs = Mathf.Infinity;
        key = new Vector2(
                        Mathf.Min(g, rhs + heuristic),
                        Mathf.Min(g, rhs));
        is_goal = false;

        neighbours = new List<GraphNode>();
        costs = new List<float>();
    }

    public GraphNode(Vector3 p, float h)
    {
        position = p;
        heuristic = h;
        g = Mathf.Infinity;
        rhs = Mathf.Infinity;
        key = new Vector2(
                        Mathf.Min(g, rhs + heuristic),
                        Mathf.Min(g, rhs));
        is_goal = false;

        neighbours = new List<GraphNode>();
        costs = new List<float>();
    }

    void Start()
    {
        // transform.position = position;
    }

	// Update is called once per frame
	// not sure I need this
    void Update() 
    {
		
	}
    
    public void calculate_key()
    {
        key = new Vector2(
                        Mathf.Min(g, rhs + heuristic),
                        Mathf.Min(g, rhs));
    }

    public void to_string()
    {
        Debug.Log("Node position: " + position.ToString("F8"));
        Debug.Log("\tHeuristic: " + heuristic);
        Debug.Log("\tg: " + g);
        Debug.Log("\trhs: " + rhs); 
        Debug.Log("\tKey: " + key.ToString("F8"));
    }
}
