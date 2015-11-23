using UnityEngine;
using System.Collections;

public class WallDetector : MonoBehaviour {
    private PlayerController pc;

    void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag.Equals("Cave") || collider.tag.Equals("Overworld"))
            pc.SetOnWall(true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag.Equals("Cave") || collider.tag.Equals("Overworld"))
            pc.SetOnWall(false);
    }
}
