using UnityEngine;
using System.Collections;

public class RockController : MonoBehaviour {

	AudioSource audioSource;
	public AudioClip[] audioClips;

	// Use this for initialization
	void Start () {
		this.audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.relativeVelocity.magnitude > 9.0f) {
			AudioClip randAudioClip = this.audioClips[UnityEngine.Random.Range(0,this.audioClips.Length-1)];
			audioSource.PlayOneShot (randAudioClip);
		}
		
	}

}
