using UnityEngine;
using System.Collections;

public class ChestTrigger : MonoBehaviour {

	private bool open;
	public Texture2D fadeOutTexture;

	// Use this for initialization
	void Start () {
		open = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (open && Input.GetKeyDown("e")) {
			StartCoroutine(openChest());
		}
	}

	IEnumerator openChest(){
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (1.5f);
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (-1);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.name == "Player") {
			open = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.name == "Player") {
			open = false;
		}
	}
}
