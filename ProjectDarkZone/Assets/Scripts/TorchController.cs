using UnityEngine;
using System.Collections;

public class TorchController : MonoBehaviour {
	
	
	public AudioClip igniteSound;
	public AudioClip burningSound;
	public AudioClip extinguishSound;
	
	float burnTime = 10.0f;
	float extinguishProbability = 0.0001f;
	
	TorchState state = TorchState.Unlit;
	
	public float startTime;
	
	public bool shouldFollowPlayer = false;
	
	Light light;
	
	public Transform player;
	PlayerController playerController;
	TorchPlacer torchPlacer;
	
	bool previousPlayerFacingRight;
	
	float initialXOffset;
	float initialYOffset;
	
	AudioSource audioSource;
	
	bool isDim = false;
	float dimFactor = 2.25f;
	float dimStartTime;
	float dimDuration;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		
		this.light = this.GetComponent<Light>();
		
		audioSource = this.GetComponent<AudioSource>();
		
		if (shouldFollowPlayer) {
			if (this.player == null) {
				this.player = GameObject.Find ("Player").transform;
			}
			playerController = this.player.GetComponent<PlayerController>();
			torchPlacer = this.player.GetComponent<TorchPlacer>();
			this.transform.parent = player.transform;
		}
	}
	
	void Update() {
		if (this.state == TorchState.Unlit) {
			Ignite();
		}
		else if (this.state == TorchState.Lit) {
			Burn ();
		}
		
		if (isDim) {
			if (Time.time-dimStartTime >= dimDuration) {
				EndDim ();
			}
		}
	}
	
	void Flip()
	{
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	public void Dim(float duration) {
		this.dimDuration = duration;
		this.dimStartTime = Time.time;
		this.isDim = true;
		this.light.intensity /= dimFactor;
	}
	
	void EndDim() {
		this.isDim = false;
		this.light.intensity *= dimFactor;
	}
	
	void Burn() {
		if (audioSource.isPlaying == false) {
			audioSource.PlayOneShot(burningSound,1.0f);
		}
		if (Time.time - startTime >= burnTime) {
			if (UnityEngine.Random.value < this.extinguishProbability) {
				this.Extinguish();
			}
		}
	}
	
	void Ignite() {
		audioSource.PlayOneShot(igniteSound,1.0f);
		this.state = TorchState.Lit;
	}
	
	void Extinguish() {
		audioSource.PlayOneShot(extinguishSound,0.2f);
		this.state = TorchState.Extinguished;
		if (player != null && this.transform.parent == player.transform) {
			torchPlacer.isHoldingTorch = false;
		}
		Destroy(this.gameObject,1.0f);
	}
	
	public enum TorchState {
		Unlit,
		Lit,
		Extinguished
	}
}
