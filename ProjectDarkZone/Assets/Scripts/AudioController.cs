using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	AudioSource audioSource;

	public AudioClip ambient1;


	public AudioClip shakeSound;
	public AudioClip sweepSound;
	public AudioClip bewareSound;

	void Start() {
		this.audioSource = this.GetComponent<AudioSource>();
	}

	public void PlayShakeSound() {
		audioSource.PlayOneShot(shakeSound);
	}

	public void PlaySweepSound() {
		audioSource.PlayOneShot(sweepSound);
	}

	public void PlayBewareSound() {
		audioSource.PlayOneShot(bewareSound);
	}

}
