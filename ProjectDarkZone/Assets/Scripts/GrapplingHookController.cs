using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookController : MonoBehaviour {
    private List<GraplingHook> grapplingHooks;
    public int numOfGrapplingHooks = 0;
    private bool canClimb = false;
    private int climables = 0;
    private GameObject thePlayer;

    // Use this for initialization
    void Start () {
	    grapplingHooks = new List<GraplingHook>();
        thePlayer = this.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        climables = 0;

        if(numOfGrapplingHooks == 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                grapplingHooks.Add(new GraplingHook(thePlayer, numOfGrapplingHooks));
                grapplingHooks[numOfGrapplingHooks].SetFrameSpacing(4);
                grapplingHooks[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
                grapplingHooks[numOfGrapplingHooks].CreateRope();

                numOfGrapplingHooks++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && !grapplingHooks[numOfGrapplingHooks - 1].isCreatingRope())
        {
            grapplingHooks.Add(new GraplingHook(this.gameObject, numOfGrapplingHooks));
            grapplingHooks[numOfGrapplingHooks].SetFrameSpacing(4);
            grapplingHooks[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
            grapplingHooks[numOfGrapplingHooks].CreateRope();

            numOfGrapplingHooks++;
        }
	    
        foreach(GraplingHook grp in grapplingHooks)
        {
            grp.Update();
            grp.facingRight = this.GetComponent<PlayerController>().facingRight;

            if (grp.isClimbable)
                climables++;
        }

        if (climables > 0)
            canClimb = true;
        else
            canClimb = false;

        thePlayer.GetComponent<Climbing>().isClimbing = canClimb;
    }
   
}
