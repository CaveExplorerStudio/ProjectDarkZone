using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraplingHook{
    public List<GameObject> ropeSegments = new List<GameObject>();
    private List<int> ropeSegmentCodes = new List<int>();
    private GameObject playerPosition;
    private GameObject prefab;
    private GameObject parent;
    private int frameCounter = 0;
    private bool creatingRope;
    private int segmentsCreated = 0;
    public int maxSegments = 20;
    private DistanceJoint2D tempHinge;
    private int newStartIndex = 0;
    private float damper = 1;
    public int frameSpacing = 4;
    public LayerMask rope_layers;
    private int ropesCreated = 0;
    private bool createRope = false;
    private GameObject referencedObject;
    private int ropeNumber = 0;
    private bool foundHead = false;
    private GameObject head;
    public LayerMask ground_layers;
    public bool facingRight = true;
    public bool isClimbable = false;


    private bool freeze = false;
    private int freezeTimer = 120;

    public GraplingHook(GameObject reference, int number)
    {
        referencedObject = reference;
        ropeNumber = number;
        Start();
    }

    // Use this for initialization
    void Start () {
        playerPosition = GameObject.Find("FirePoint");

        prefab = Resources.Load("RopeSegment", typeof(GameObject)) as GameObject;
        parent = new GameObject();
        parent.name = "Grappling Hook";

        ground_layers = 1 << LayerMask.NameToLayer("Cave");
        rope_layers = 1 << LayerMask.NameToLayer("Player");
    }
	
	// Update is called once per frame
	public void Update () {
        playerPosition = GameObject.Find("FirePoint");
        GameObject part = GameObject.Find("RopeSegment");
      
        if (createRope && creatingRope == false) //Remove the first false pls
        {
            CreateGraplingHook(true);
            frameCounter = 0;
            creatingRope = true;
            createRope = false;
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

        if (!foundHead)
        {
            if (!(GameObject.Find("head" + ropeNumber) == null))
            {
                head = GameObject.Find("head" + ropeNumber);
                foundHead = true;
            }
        }

        if (foundHead && !(Physics2D.OverlapCircle(new Vector2(head.transform.position.x, head.transform.position.y), .2f, ground_layers) == null))
        {
            GameObject.Find("head" + ropeNumber).GetComponent<Rigidbody2D>().isKinematic = true;
            //Debug.Log("Yo, I hit something!!");            
        }

    }

    void CreateGraplingHook(bool firstSegment)
    {
        Material newMat = Resources.Load("Rope", typeof(Material)) as Material;
        GameObject projectile = MonoBehaviour.Instantiate(prefab) as GameObject;
        projectile.transform.position = GameObject.Find("FirePoint").transform.position;
        projectile.transform.rotation = GameObject.Find("FirePoint").transform.rotation;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (facingRight)
            rb.AddForce(new Vector2(1, 4) * (1.2f * damper));
        else
            rb.AddForce(new Vector2(-1, 4) * (1.2f * damper));
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
            ropeSegments[ropeSegments.IndexOf(projectile)].name = "head" + ropeNumber;
            //ropeSegments[ropeSegments.IndexOf(projectile)].GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else
        {
            projectile.transform.SetParent(parent.transform);
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
                    isClimbable = true;
                    foundOnCurrentIteration = true;
                }


            }
            
        }
        if (!foundOnCurrentIteration)
            isClimbable = false;
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

    public void CreateRope()
    {
        createRope = true;
    }

    public bool isCreatingRope()
    {
        return creatingRope;
    }

    public void SetFrameSpacing(int space)
    {
        frameSpacing = space;
    }
   
}
