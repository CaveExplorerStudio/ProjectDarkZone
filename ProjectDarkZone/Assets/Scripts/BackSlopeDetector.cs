using UnityEngine;
using System.Collections;

public class BackSlopeDetector : MonoBehaviour {
    private PlayerController pc;

    void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
    }

	void OnTriggerEnter2D(Collider2D collider)
    {
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.NameToLayer("Cave")))
            pc.SetDownSlope(true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.NameToLayer("Cave")))
            pc.SetDownSlope(false);
    }
}
