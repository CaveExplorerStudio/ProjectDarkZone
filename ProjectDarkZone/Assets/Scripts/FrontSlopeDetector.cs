using UnityEngine;
using System.Collections;

public class FrontSlopeDetector : MonoBehaviour {
    private PlayerController pc;

    void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        pc.SetUpSlope(true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        pc.SetUpSlope(false);
    }
}
