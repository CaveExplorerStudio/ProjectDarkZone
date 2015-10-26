using UnityEngine;
using System.Collections;

public class CheckCollide : MonoBehaviour {
    public int segmentIndex;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.ToLower() == "target")
        {
            GameObject.Find("Player").GetComponent<GraplingHook>().ropeSegments[segmentIndex].GetComponent<Rigidbody2D>().mass = 1000;
            Debug.Log("I'm colliding over here!!");
        }

    }
}
