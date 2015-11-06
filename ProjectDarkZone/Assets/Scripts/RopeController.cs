using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeController : MonoBehaviour
{
    private List<Rope> ropes;
    public int numOfRopes = 0;
    public bool canClimbRope = false;
    private int climables = 0;
    private GameObject thePlayer;
    private Climbing playerClimbing;
    private GrapplingHookController playerHookController;

    // Use this for initialization
    void Start()
    {
        ropes = new List<Rope>();
        thePlayer = this.gameObject;
        playerClimbing = this.GetComponent<Climbing>();
        playerHookController = this.GetComponent<GrapplingHookController>();
    }

    // Update is called once per frame
    void Update()
    {
        climables = 0;

        if (numOfRopes == 0)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ropes.Add(new Rope(thePlayer, numOfRopes));
                ropes[numOfRopes].SetFrameSpacing(4);
                ropes[numOfRopes].facingRight = this.GetComponent<PlayerController>().facingRight;
                ropes[numOfRopes].CreateRope();

                numOfRopes++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H) && !ropes[numOfRopes - 1].isCreatingRope())
        {
            ropes.Add(new Rope(this.gameObject, numOfRopes));
            ropes[numOfRopes].SetFrameSpacing(4);
            ropes[numOfRopes].facingRight = this.GetComponent<PlayerController>().facingRight;
            ropes[numOfRopes].CreateRope();

            numOfRopes++;
        }

        foreach (Rope rp in ropes)
        {
            rp.Update();
            rp.facingRight = this.GetComponent<PlayerController>().facingRight;

            if (rp.isClimbable)
                climables++;
        }

        if (climables > 0)
            canClimbRope = true;
        else
            canClimbRope = false;

        playerClimbing.isClimbingRope = canClimbRope;
    }

}