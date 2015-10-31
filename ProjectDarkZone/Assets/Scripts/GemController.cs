using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour {
	
	Light light;
	AudioSource audioSource;
	
	bool hasBeenRevealed = false;
	
	float lightStartTime;
	bool lightShouldFadeIn = false;
	bool lightShouldHold = false;
	bool lightShouldFadeOut = false;
	
	Transform player;
	Transform camera;
	
	// Use this for initialization
	void Start () {
		this.light = GetComponentInChildren<Light>();
		this.light.enabled = false;
		this.audioSource = GetComponent<AudioSource>();
		this.player = GameObject.Find ("Player").transform;
		this.camera = GameObject.Find ("Main Camera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasBeenRevealed && IsWithinCameraView()) {
			hasBeenRevealed = true;
			Reveal ();
		}
		
		DoLightLogic();
		
		
	}
	
	bool IsWithinCameraView() {
		
		//TODO: Take into account the camera pan, not just the screen size.
		
		float sightY = Camera.main.orthographicSize;    
		float sightX = (sightY * Screen.width / Screen.height);
		float xDist = Mathf.Abs(camera.position.x - this.transform.position.x);
		float yDist = Mathf.Abs (camera.position.y - this.transform.position.y);
		if (xDist <= sightX && yDist <= sightY) {
			return true;
		}
		else {
			return false;
		}
	}
	
	void Reveal() {
		ShowLight();
		PlayRevealSound();
	}
	
	void PlayRevealSound() {
		audioSource.Play ();
	}
	
	void ShowLight() {
		this.light.enabled = true;
		this.light.intensity = 0;
		lightShouldFadeIn = true;
		lightStartTime = Time.time;
	}
	
	void DoLightLogic() {
		if (lightShouldFadeIn) {
			if (Time.time - lightStartTime >= 2.0f) {
				lightShouldFadeIn = false;
				lightShouldHold = true;
			}
			this.light.intensity += 0.01f;
		}
		if (lightShouldHold) {
			if (Time.time - lightStartTime >= 6.0f) {
				lightShouldHold = false;
				lightShouldFadeOut = true;
			}
		}
		if (lightShouldFadeOut) {
			if (Time.time - lightStartTime >= 7.0f) {
				lightShouldFadeOut = false;
			}
			this.light.intensity -= 0.01f;
		}
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position,2.0f);
	}
	
	public void PlayPickupSound() {
		//TODO
	}
	
	
}
