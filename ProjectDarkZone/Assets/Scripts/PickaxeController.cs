using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickaxeController : MonoBehaviour {
    //References
    GameObject leftSpike;
    GameObject rightSpike;

    List<Pickaxe> axes;


	// Use this for initialization
	void Start () {
        axes = new List<Pickaxe>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Pickaxe pick = new Pickaxe();
            pick.ThrowPickaxe();
            axes.Add(pick);
        }

        foreach(Pickaxe p in axes)
        {
            p.checkCollision();
        }
        
    }
}
