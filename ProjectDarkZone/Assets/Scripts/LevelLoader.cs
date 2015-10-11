using UnityEngine;
using System.Collections;


public class LevelLoader : MonoBehaviour {

	private bool enter;

	public string levelToLoad;

	void Start(){
		enter = false;
	}

	void Update(){
		if (enter){
			StartCoroutine(ChangeLevel());
		}
	}

	IEnumerator ChangeLevel(){
		float fadeTime = GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (1.5f);
		Application.LoadLevel (levelToLoad);
	}
	
	// Use this for initialization
	void OnTriggerEnter2D(Collider2D other) {
		if (other.name == "Player") {
			enter = true;
		}
	}
	

}
