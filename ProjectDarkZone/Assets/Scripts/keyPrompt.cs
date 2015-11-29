using UnityEngine;
using System.Collections;

public class keyPrompt : MonoBehaviour {

	public string levelToLoad;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			StartCoroutine (StartGame ());
		}
	}

	IEnumerator StartGame(){
		yield return new WaitForSeconds (.1f);
		Application.LoadLevel (levelToLoad);
	}
}
