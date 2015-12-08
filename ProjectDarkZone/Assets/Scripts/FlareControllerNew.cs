using UnityEngine;
using System.Collections;

public class FlareControllerNew : MonoBehaviour {


	Light light;
	AudioSource audioSource;
	float initialTime;
	float lifeTime = 20.0f;

	float maxIntensity = 8.0f;
	bool shouldGoOut = false;

	// Use this for initialization
	void Start () {
		this.light = GetComponentInChildren<Light>();
		this.audioSource = GetComponent<AudioSource>();
		initialTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.light != null) {
			if (shouldGoOut || Time.time - initialTime > lifeTime) {
				shouldGoOut = true;
				maxIntensity -= 0.05f;
				audioSource.volume -= 0.01f;
			}
			if (maxIntensity < 0.1f) {
				Destroy(this.light);
				Destroy(this.audioSource);
			}
			
			if (Random.value < 0.2f) {
				this.light.intensity = maxIntensity/8;
			}
			else {
				this.light.intensity = maxIntensity;
			}
		}
	}
}
