using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraplingHook : MonoBehaviour {
    public List<GameObject> ropeSegments = new List<GameObject>();
    private List<int> ropeSegmentCodes = new List<int>();
    private GameObject playerPosition;
    private GameObject prefab;
    private int frameCounter = 0;
    private bool creatingRope;
    private int segmentsCreated = 0;
    public int maxSegments = 50;
    private DistanceJoint2D tempHinge;
    private int newStartIndex = 0;
    private float damper = 1;
    public int frameSpacing = 12;
    public LayerMask rope_layers;
    private int ropesCreated = 0;

    private bool freeze = false;
    private int freezeTimer = 120;

    // Use this for initialization
    void Start () {
        playerPosition = GameObject.Find("FirePoint");

        prefab = GameObject.Find("RopeSegment") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        playerPosition = GameObject.Find("FirePoint");
        GameObject part = GameObject.Find("RopeSegment");
      
        if (Input.GetMouseButtonDown(0) && creatingRope == false) //Remove the first false pls
        {
            CreateGraplingHook(true);
            frameCounter = 0;
            creatingRope = true;
        }

        if (creatingRope && frameCounter % frameSpacing == 0 && frameCounter != 0 && segmentsCreated < maxSegments)
        {
            CreateGraplingHook(false);
            segmentsCreated++;
            damper -= .01f;

            tempHinge = ropeSegments[segmentsCreated - 1 + newStartIndex].AddComponent<DistanceJoint2D>();
            tempHinge.connectedBody = ropeSegments[segmentsCreated + newStartIndex].GetComponent<Rigidbody2D>();
            tempHinge.anchor = new Vector2(1f, 0f);
            tempHinge.distance = 0.01f;
            tempHinge.enableCollision = true;

            
        }

        frameCounter++;

        if (segmentsCreated == maxSegments)
        {
            creatingRope = false;
            newStartIndex += 1;
            newStartIndex += segmentsCreated;
            segmentsCreated = 0;
            damper = 1;
            ropeSegmentCodes.Add(0);
            freeze = true;
            ropesCreated++;
            //FreezeRope();
        }

        if (freeze)
        {
            if (freezeTimer != 0)
                freezeTimer--;
            else
                FreezeRope();
        }

        if (ropeSegments.Count >= 2)
        {
            UpdateTraces();
        }

     
    }

    void CreateGraplingHook(bool firstSegment)
    {
        Material newMat = Resources.Load("Rope", typeof(Material)) as Material;
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = GameObject.Find("FirePoint").transform.position;
        projectile.transform.rotation = GameObject.Find("FirePoint").transform.rotation;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(1, 4) * (1.2f * damper));
        projectile.AddComponent<LineRenderer>();
        projectile.GetComponent<LineRenderer>().SetWidth(.1f, .1f);
        projectile.GetComponent<LineRenderer>().material = newMat;
        if (!firstSegment)
            projectile.GetComponent<BoxCollider2D>().isTrigger = false;

        ropeSegments.Add(projectile);
        ropeSegmentCodes.Add(1);

        if (firstSegment)
        {
            ropeSegments[ropeSegments.IndexOf(projectile)].AddComponent<CheckCollide>();
            ropeSegments[ropeSegments.IndexOf(projectile)].GetComponent<CheckCollide>().segmentIndex = ropeSegments.IndexOf(projectile);
            ropeSegments[ropeSegments.IndexOf(projectile)].name = "head";
            //ropeSegments[ropeSegments.IndexOf(projectile)].GetComponent<BoxCollider2D>().isTrigger = true;
        }

    }

    void UpdateTraces()
    {
        bool foundOnCurrentIteration = false;
        for (int i = 1; i < ropeSegments.Count; i++)
        {
            if (ropeSegmentCodes[i] == 0) //(i-1) % maxSegments == 0
                i++;
            else if (i < ropeSegments.Count)
            {
                ropeSegments[i].GetComponent<LineRenderer>().SetPosition(1, new Vector3(ropeSegments[i - 1].transform.position.x,
                                                                                    ropeSegments[i - 1].transform.position.y,
                                                                                    ropeSegments[i - 1].transform.position.z));

                ropeSegments[i].GetComponent<LineRenderer>().SetPosition(0, new Vector3(ropeSegments[i].transform.position.x,
                                                                                        ropeSegments[i].transform.position.y,
                                                                                        ropeSegments[i].transform.position.z));
                
                if (!(Physics2D.OverlapArea(new Vector2(ropeSegments[i - 1].transform.position.x, ropeSegments[i - 1].transform.position.y), 
                                            new Vector2(ropeSegments[i].transform.position.x, ropeSegments[i].transform.position.y), 
                                            rope_layers) == null) && !creatingRope) //Rope or player??
                {
                    this.gameObject.GetComponent<Climbing>().isClimbing = true;
                    foundOnCurrentIteration = true;
                    Debug.Log("I got to the thing boss.");
                }

                //if (!(Physics2D.OverlapCircle(ropeSegments[i].transform.position, 1) == null) && !creatingRope)
                //{
                //    Debug.Log("Circle collided!!!");
                //}

                //if (this.gameObject.GetComponent<Climbing>().isClimbing)

            }
            
        }
        if (!foundOnCurrentIteration)
            this.gameObject.GetComponent<Climbing>().isClimbing = false;
    }

    void FreezeRope()
    {
        int maxIndex = ropesCreated * maxSegments;
        for (int i = 0; i < maxIndex; i++)
        {
            ropeSegments[i].GetComponent<Rigidbody2D>().isKinematic = true;
            ropeSegments[i].GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
   
}
