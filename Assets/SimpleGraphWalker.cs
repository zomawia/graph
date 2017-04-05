using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGraphWalker : MonoBehaviour {

    public GraphManager gm;
    public Transform target;
    Rigidbody rb;

    private List<Vector3> pathToWalk;
    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start () {
        pathToWalk = gm.FindPathBetween(transform, target);
	}
	
	// Update is called once per frame
	void Update () {

        if (pathToWalk.Count == 0) return;
        
        // what direction to walk
        Vector3 dir = (pathToWalk[0] - transform.position).normalized;
        transform.position += dir * Time.deltaTime * 2;     

        if (Vector3.Distance(pathToWalk[0], transform.position) < .5f)
        {
            pathToWalk.RemoveAt(0);
        }
    }

    private void OnValidate()
    {
        if (target != null)
            pathToWalk = gm.FindPathBetween(transform, target);
    }

    private void OnDrawGizmos()
    {
        foreach (var t in pathToWalk)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(t, .3f);
        }
    }
}
