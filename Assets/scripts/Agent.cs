using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TO BE ATTACHED TO THE CROWD CLASS
// SHOULD NOT INTERNALLY REFERENCE CROWD CLASS IN ANY WAY 

/*
 * @brief The agent class; each agent will be clumped into some type of Crowd. 
 * 
 * @param vision_distance The max distance the agent can see. This will be used
 *          in the PRM construction.
 * @param radius Radius of bounding sphere
 * @param rb Rigidbody to control velocity
 * 
*/

public class Agent : MonoBehaviour 
{
    public float vision_distance;
    public float radius;

    private Rigidbody rb;

    public void initialize()
    {
        vision_distance = 20;
        radius = 1;
    }

    public void initialize(float vd, float rad)
    {
        vision_distance = vd;
        radius = rad;
    }

    public List<GraphNode> sample_points(int num_nodes, GraphNode goal)
    {
        List<GraphNode> nodes = new List<GraphNode>();

        for(int i = 0; i < num_nodes; i++)
        {
            Vector3 sample_point = new Vector3(
                                        transform.position.x + vision_distance * Random.value, 
                                        transform.position.y + vision_distance * Random.value, 
                                        transform.position.z + vision_distance * Random.value);
            RaycastHit hit;

            if(!Physics.SphereCast(transform.position, radius, 
                                (transform.position - sample_point).normalized,
                                out hit, vision_distance))
            {
                nodes.Add(new GraphNode(sample_point, Vector3.Distance(goal.transform.position, sample_point)));
            }

            else
            {
                i--;
            }

            // I don't think I need a for(int i = 0; i< obstacles.size(); i++) loop
            // because Physics.RayCast does this shit for us? 
            // If I do need it, add List<Collider> obstacles as a parameter.
        }
        
        return nodes;
    }

	// Use this for initialization
	void Start() 
    {
		
	}
	
	// Update is called once per frame
	void Update() 
    {
		
	}
}
