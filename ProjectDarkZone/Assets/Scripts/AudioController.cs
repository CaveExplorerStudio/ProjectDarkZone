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
		this.audioSource.clip = ambient1;
		this.audioSource.Play();
		this.audioSource.loop = true;
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
