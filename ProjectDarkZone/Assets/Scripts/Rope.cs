using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Rope
{
    public List<GameObject> ropeSegments = new List<GameObject>();
    public List<int> segmentCounters = new List<int>();
    public int maxSegments = 40;
    public int frameSpacing = 4;
    public LayerMask rope_layers;
    public LayerMask ground_layers;
    public bool facingRight = true;
    public bool isClimbable = false;


    private List<int> ropeSegmentCodes = new List<int>();
    private GameObject playerPosition;
    private GameObject prefab;
    private GameObject parent;
    private GameObject referencedObject;
    private GameObject head;

    private DistanceJoint2D tempHinge;
    private float damper = 1;
    private int freezeTimer = 200;
    private int newStartIndex = 0;
    private int ropesCreated = 0;
    private int ropeNumber = 0;
    private int segmentsCreated = 0;
    private int frameCounter = 0;
    private bool creatingRope;
    private bool foundHead = false;
    private bool createRope = false;
    private bool freeze = false;


    public Rope(GameObject reference, int number)
    {
        referencedObject = reference;
        ropeNumber = number;
        Start();
    }

    // Use this for initialization
    void Start()
    {
        playerPosition = GameObject.Find("FirePoint");
        prefab = Resources.Load("RopeSegment", typeof(GameObject)) as GameObject;
        parent = new GameObject();
        parent.name = "Rope";
        ground_layers = 1 << LayerMask.NameToLayer("Cave");
        rope_layers = 1 << LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    public void Update()
    {
        if (createRope && creatingRope == false) //Remove the first false pls
        {
            CreateGraplingHook(true);
            frameCounter = 0;
            creatingRope = true;
            createRope = false;
            //freeze = true;
        }

        if (creatingRope && frameCounter % frameSpacing == 0 && frameCounter != 0 && segmentsCreated < maxSegments)
        {
            CreateGraplingHook(false);
            segmentsCreated++;
            damper += .02f;

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
        }

        for (int i = 0; i < segmentCounters.Count; i++)
        {
            if (segmentCounters[i] > 0)
                segmentCounters[i] -= 1;
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
            if (!(GameObject.Find("RHead" + ropeNumber) == null))
            {
                head = GameObject.Find("RHead" + ropeNumber);
                foundHead = true;
            }
        }

        if (foundHead && !(Physics2D.OverlapCircle(new Vector2(head.transform.position.x, head.transform.position.y), .2f, ground_layers) == null))
        {
            head.GetComponent<Rigidbody2D>().isKinematic = true;
        }

    }

    private void NewFreezeRope()
    {
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            if (segmentCounters[i] == 0)
            {
                ropeSegments[i].GetComponent<Rigidbody2D>().isKinematic = true;
                ropeSegments[i].GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
    }

    void CreateGraplingHook(bool firstSegment)
    {
        Material newMat = Resources.Load("Rope", typeof(Material)) as Material;
        GameObject projectile = MonoBehaviour.Instantiate(prefab) as GameObject;
        GameObject firePoint = GameObject.Find("FirePoint");
        projectile.transform.position = firePoint.transform.position;
        projectile.transform.rotation = firePoint.transform.rotation;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (facingRight)
            rb.AddForce(new Vector2(.2f, 1f) * (1.2f * damper)); //Edit to change launch angle of grappling hook
        else
            rb.AddForce(new Vector2(-.2f, 1f) * (1.2f * damper)); //Edit to change launch angle of grappliing hook

        LineRenderer rnd = projectile.AddComponent<LineRenderer>();
        rnd.SetWidth(.1f, .1f);
        rnd.material = newMat;

        if (!firstSegment)
            projectile.GetComponent<BoxCollider2D>().isTrigger = false;



        ropeSegments.Add(projectile);
        segmentCounters.Add(120);
        ropeSegmentCodes.Add(1);

        if (firstSegment)
        {
            CheckCollide chk = ropeSegments[ropeSegments.IndexOf(projectile)].AddComponent<CheckCollide>();
            chk.segmentIndex = ropeSegments.IndexOf(projectile);
            ropeSegments[ropeSegments.IndexOf(projectile)].name = "RHead" + ropeNumber;
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
                LineRenderer rnd = ropeSegments[i].GetComponent<LineRenderer>();
                rnd.SetPosition(1, new Vector3(ropeSegments[i - 1].transform.position.x,
                                                                                    ropeSegments[i - 1].transform.position.y,
                                                                                    ropeSegments[i - 1].transform.position.z));

                rnd.SetPosition(0, new Vector3(ropeSegments[i].transform.position.x,
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
