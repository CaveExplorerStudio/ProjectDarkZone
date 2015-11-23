using UnityEngine;
using System.Collections;

public class ExitController : MonoBehaviour {

	GameObject player;


	// Use this for initialization
	void Start () {
		this.player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.Return) && NearPlayer())
			StartCoroutine(ChangeLevel());
	}

	bool NearPlayer() {
		if (Vector2.Distance(this.transform.position, player.transform.position) < 1.5f) {
			return true;
		}
		else {
			return false;
		}
	}

	IEnumerator ChangeLevel(){
		float fadeTime = GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (1.5f);
		Application.LoadLevel ("Overworld");
	}
}
