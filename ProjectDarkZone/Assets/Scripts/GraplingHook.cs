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
    public int maxSegments = 20;
    private HingeJoint2D tempHinge;

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

            tempHinge = ropeSegments[segmentsCreated - 1].AddComponent<HingeJoint2D>();
            tempHinge.connectedBody = ropeSegments[segmentsCreated].GetComponent<Rigidbody2D>();
        }

        frameCounter++;

        if (segmentsCreated == maxSegments)
        {
            creatingRope = false;
            segmentsCreated = 0;
        }


            
    }

    void CreateGraplingHook()
    {
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = GameObject.Find("FirePoint").transform.position;
        projectile.transform.rotation = GameObject.Find("FirePoint").transform.rotation;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(1, 4) * 120);
        ropeSegments.Add(projectile);
    }
}
