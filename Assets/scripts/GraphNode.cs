using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public List<GraphNode> neighbours;
    public List<float> costs;
    public Vector3 position;
    public float heuristic;
    public float g;
    public bool is_goal;

    public GraphNode()
    {
        position = new Vector3(0, 0, 0);
        heuristic = 1.0f;
        g = Mathf.Infinity;
        is_goal = false;

        neighbours = new List<GraphNode>();
        costs = new List<float>();
    }

    public GraphNode(Vector3 p, float h)
    {
        position = p;
        heuristic = h;
        g = Mathf.Infinity;
        is_goal = false;

        neighbours = new List<GraphNode>();
        costs = new List<float>();
    }

    public void to_string()
    {
        Debug.Log("\tNode position: " + position.ToString("F8"));
        Debug.Log("\tHeuristic: " + heuristic);
        Debug.Log("\tg: " + g);
    }

    public static bool operator==(GraphNode a, GraphNode b)
    {
        if(a.position == b.position)
        {
            return true;
        }

        return false;
    }

    public static bool operator!=(GraphNode a, GraphNode b)
    {
        if(a.position == b.position)
        {
            return false;
        }

        return true;
    }
}
