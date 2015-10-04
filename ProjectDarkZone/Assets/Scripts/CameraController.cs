using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		this.transform.position = new Vector3(target.position.x,target.position.y,this.transform.position.z);
	}
}
