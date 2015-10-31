using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeObjectController : MonoBehaviour
{
    private List<Rope> ropes;
    public int numOfGrapplingHooks = 0;
    private bool canClimb = false;
    private int climables = 0;
    private GameObject thePlayer;

    // Use this for initialization
    void Start()
    {
        ropes = new List<Rope>();
        thePlayer = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        climables = 0;

        if (numOfGrapplingHooks == 0)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ropes.Add(new Rope(thePlayer, numOfGrapplingHooks));
                ropes[numOfGrapplingHooks].SetFrameSpacing(4);
                ropes[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
                ropes[numOfGrapplingHooks].CreateRope();

                numOfGrapplingHooks++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H) && !ropes[numOfGrapplingHooks - 1].isCreatingRope())
        {
            ropes.Add(new Rope(this.gameObject, numOfGrapplingHooks));
            ropes[numOfGrapplingHooks].SetFrameSpacing(4);
            ropes[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
            ropes[numOfGrapplingHooks].CreateRope();

            numOfGrapplingHooks++;
        }

        foreach (Rope rp in ropes)
        {
            rp.Update();
            rp.facingRight = this.GetComponent<PlayerController>().facingRight;

            if (rp.isClimbable)
                climables++;
        }

        if (climables > 0)
            canClimb = true;
        else
            canClimb = false;

        thePlayer.GetComponent<Climbing>().isClimbing = canClimb;
    }

}

