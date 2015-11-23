using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrapplingHookController : MonoBehaviour, IItem {
    public string Name { get; set; }
    public Sprite Image { get; set; }
    public bool IsConsumable { get; set; }
    public GameObject Prefab { get; set; }
    private List<GraplingHook> grapplingHooks;
    public int numOfGrapplingHooks = 0;
    public bool canClimbGrapplingHook = false;
    private int climables = 0;
    private GameObject thePlayer;
    private Climbing playerClimbing;
    private RopeController playerRopeController;

    // Use this for initialization
    void Start () {
	    grapplingHooks = new List<GraplingHook>();
        thePlayer = GameObject.Find("Player");
        playerClimbing = thePlayer.GetComponent<Climbing>();
        playerRopeController = thePlayer.GetComponent<RopeController>();
        Name = "GrapplingHook";
        Image = Resources.Load<Sprite>("torch");
        IsConsumable = true;
        Prefab = null;
    }
	
    public void Use()
    {
        if (numOfGrapplingHooks == 0)
        {
            grapplingHooks.Add(new GraplingHook(thePlayer, numOfGrapplingHooks));
            grapplingHooks[numOfGrapplingHooks].SetFrameSpacing(4);
            grapplingHooks[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
            grapplingHooks[numOfGrapplingHooks].CreateRope();

            numOfGrapplingHooks++;
        }
        else if (!grapplingHooks[numOfGrapplingHooks - 1].isCreatingRope())
        {
            grapplingHooks.Add(new GraplingHook(this.gameObject, numOfGrapplingHooks));
            grapplingHooks[numOfGrapplingHooks].SetFrameSpacing(4);
            grapplingHooks[numOfGrapplingHooks].facingRight = this.GetComponent<PlayerController>().facingRight;
            grapplingHooks[numOfGrapplingHooks].CreateRope();

            numOfGrapplingHooks++;
        }
    }

	// Update is called once per frame
	void Update () {
        climables = 0;

        foreach(GraplingHook grp in grapplingHooks)
        {
            grp.Update();
            grp.facingRight = this.GetComponent<PlayerController>().facingRight;

            if (grp.isClimbable)
                climables++;
        }

        if (climables > 0)
            canClimbGrapplingHook = true;
        else
            canClimbGrapplingHook = false;

        playerClimbing.isClimbingGrapplingHook = canClimbGrapplingHook;
    }
   
}
