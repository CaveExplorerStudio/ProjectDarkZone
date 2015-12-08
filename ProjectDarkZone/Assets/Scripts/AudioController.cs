using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	AudioSource audioSource;

	public AudioClip ambient1;


	public AudioClip shakeSound;
	public AudioClip sweepSound;
	public AudioClip bewareSound;
	public AudioClip landingSound;
	public AudioClip heartBeatSound;
	public AudioClip grapplingThrowSound;
	public AudioClip grapplingHitSound;
	public AudioClip[] footSteps;

	//Delays
	private float timeOfLastFootStep = 0.0f;
	private float footStepDelay = 0.4f;

	private float timeOfLastHeartBeat = 0.0f;
	private float heartBeatDelay = 7.0f;

	private float timeOfLastGrappleHit = 0.0f;
	private float grappleHitDelay = 1.0f;

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

	public void PlayLandingSound() {
		audioSource.PlayOneShot(landingSound,0.3f);
	}

	public void PlayHeartBeatSound(float volume) {
		if (Time.time - timeOfLastHeartBeat >= heartBeatDelay) {
			audioSource.PlayOneShot(heartBeatSound,volume);
			timeOfLastHeartBeat = Time.time;
		}
	}
 
	public void PlayFootStepSound() {
		if (this.footSteps.Length > 0 && Time.time - timeOfLastFootStep >= footStepDelay) {

			audioSource.PlayOneShot(footSteps[UnityEngine.Random.Range(0,footSteps.Length-1)],0.1f);
			timeOfLastFootStep = Time.time;
		}
	}

	public void PlayGrapplingThrowSound() {
		audioSource.PlayOneShot(grapplingThrowSound);
	}

	public void PlayGrapplingHitSound() {
		if (Time.time - timeOfLastGrappleHit > grappleHitDelay) {
			timeOfLastGrappleHit = Time.time;
			audioSource.PlayOneShot(grapplingHitSound);
		}
	}


}
