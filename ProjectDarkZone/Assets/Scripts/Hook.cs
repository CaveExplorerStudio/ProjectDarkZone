using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {
    private bool foundHead = false;
    private GameObject head;
    public LayerMask ground_layers;

    void Update()
    {
        if (!foundHead)
        {
            if (!(GameObject.Find("head") == null))
            {
                head = GameObject.Find("head");
                foundHead = true;
            }
        }

        if (foundHead && !(Physics2D.OverlapCircle(new Vector2(head.transform.position.x, head.transform.position.y), .2f, ground_layers) == null))
        {    
                GameObject.Find("head").GetComponent<Rigidbody2D>().isKinematic = true;
                //Debug.Log("Yo, I hit something!!");            
        }
    }
}
