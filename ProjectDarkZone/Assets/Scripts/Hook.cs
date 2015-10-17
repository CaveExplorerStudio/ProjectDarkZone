using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	void onTriggerEnter(Collider other)
    {
        GameObject.Find("RopeSegment").GetComponent<Rigidbody2D>().isKinematic = true;
        Debug.Log("Yo, I hit something!!");
    }
}
