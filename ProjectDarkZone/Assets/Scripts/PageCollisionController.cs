using UnityEngine;
using System.Collections;

public class PageCollisionController : MonoBehaviour {
    public LayerMask player_layer;
    public MeshRenderer myRenderer;
    private int deathTimer = 10;
    private bool isAlive = true;

    // Use this for initialization
    void Start () {
        myRenderer = this.GetComponentInChildren<MeshRenderer>();
        player_layer = 1 << LayerMask.NameToLayer("Player");
    }
	
	// Update is called once per frame
	void Update () {
        if (!(Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), .7f, player_layer) == null))
        {
            myRenderer.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                isAlive = false; //Probably will need to add to inventory or something here.
            }
        }
        else
        {
            myRenderer.enabled = false;
        }

        if(!isAlive)
        {
            if (deathTimer == 0)
                Destroy(this.gameObject);
            else
                deathTimer--;
        }
        
            
    }
}
