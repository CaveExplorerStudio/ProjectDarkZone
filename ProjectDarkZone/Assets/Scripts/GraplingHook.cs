using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraplingHook : MonoBehaviour {

    private Vector2 position;
    private Vector2 velocity;
    private List<GameObject> ropeSegments = new List<GameObject>();
    private GameObject playerPosition;
    private GameObject prefab;
    private int frameCounter = 0;
    private bool creatingRope;
    private int segmentsCreated = 0;
    public int maxSegments = 4;
    private DistanceJoint2D tempHinge;
    private int newStartIndex = 0;
    private float damper = 1;

	// Use this for initialization
	void Start () {
        playerPosition = GameObject.Find("FirePoint");

        prefab = GameObject.Find("RopeSegment") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        playerPosition = GameObject.Find("FirePoint");
        GameObject part = GameObject.Find("RopeSegment");
      
        

        if (Input.GetMouseButtonDown(0))
        {
            CreateGraplingHook();
            frameCounter = 0;
            creatingRope = true;
        }

        if (creatingRope && frameCounter % 4 == 0 && frameCounter != 0 && segmentsCreated < maxSegments)
        {
            CreateGraplingHook();
            segmentsCreated++;
            damper -= .01f;

            tempHinge = ropeSegments[segmentsCreated - 1 + newStartIndex].AddComponent<DistanceJoint2D>();
            tempHinge.connectedBody = ropeSegments[segmentsCreated + newStartIndex].GetComponent<Rigidbody2D>();
            tempHinge.anchor = new Vector2(1f, 0f);
            tempHinge.distance = 0.01f;


        }

        frameCounter++;

        if (segmentsCreated == maxSegments)
        {
            creatingRope = false;
            newStartIndex += 1;
            newStartIndex += segmentsCreated;
            segmentsCreated = 0;
            damper = 1;
        }


            
    }

    void CreateGraplingHook()
    {
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = GameObject.Find("FirePoint").transform.position;
        projectile.transform.rotation = GameObject.Find("FirePoint").transform.rotation;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(1, 4) * (12.0f * damper));
        ropeSegments.Add(projectile);
    }
}
