using UnityEngine;
using System.Collections;

public class TorchController : MonoBehaviour {
	
	//Audio
	AudioSource audioSource;
	public AudioClip igniteSound;
	public AudioClip burningSound;
	public AudioClip extinguishSound;
	
	//Timing
	float startTime;
	
	//Durations
	float burnTime = 10.0f; //Guaranteed time the torch will be lit (assuming a direct method to extinguish is not called)
	float extinguishProbability = 0.0001f; //Probability of torch going out by shadow after burnTime
	
	TorchState state = TorchState.Unlit;
	public bool shouldFollowPlayer = false;
	
	//References
	public Transform player;
	PlayerController playerController;
	TorchPlacer torchPlacer;
	Light light;
	
	//BlowOut Variables
	bool willBlowOut = false;
	float blowOutStartTime;
	
	
	//Dim Variables
	bool isDim = false;
	bool willDim = false;
	float initialLightIntensity;
	float dimPercentage = 0.5f;
	float dimStartTime;
	float dimDuration;
	
	//Flicker Variables
	bool isFlickering = false;
	float flickerFrequency = 0.01f;
	float flickerStartTime;
	float flickerDuration;
	float flickerDelay;
	
	
	void Start () {
		startTime = Time.time;
		
		this.light = this.GetComponentInChildren<Light>();
		
		audioSource = this.GetComponent<AudioSource>();
		
		if (shouldFollowPlayer) {
//			if (this.player == null) {
//				this.player = GameObject.Find ("Player").transform;
//			}
//			playerController = this.player.GetComponent<PlayerController>();
//			torchPlacer = this.player.GetComponent<TorchPlacer>();
//			this.transform.parent = player.transform;
		}
	}
	
	void Update() {
		
		if (this.state == TorchState.Unlit) {
			Ignite();
		}
		else if (this.state == TorchState.Lit) {
			Burn ();
		}
		
		//Blow Out
		if (willBlowOut) {
			if (Time.time > blowOutStartTime) {
				this.BlowOut();
				willBlowOut = false;
			}
		}
		
		//Flicker
		if (isFlickering) {
			Flicker ();
		}
		
		//Dim
		if (willDim) {
			if (Time.time >= dimStartTime) {
				this.isDim = true;
				this.willDim = false;
			}
		}
		else if (isDim) {
			float timeSinceStart = Time.time-dimStartTime;
			if (timeSinceStart >= dimDuration) {
				EndDim ();
			}
			else {
				float currentDimPercentage = 1-(1-dimPercentage) * (1-Mathf.Abs((dimDuration/2)-timeSinceStart)/(dimDuration/2));
				this.light.intensity = initialLightIntensity*currentDimPercentage;
			}
		}
		
		
	}
	
	void Flip()
	{
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	public void PopOffWall() {
		GetComponent<PolygonCollider2D>().enabled = true;
		Debug.Log ("Enabled Collider");
	}
	
	public void BeginDim(float percentage, float duration, float delay) {
		if (isDim == false) {
			this.dimDuration = duration + delay;
			this.dimPercentage = percentage;
			this.dimStartTime = Time.time + delay;
			this.willDim = true;
			this.initialLightIntensity = this.light.intensity;
		}
	}
	
	void EndDim() {
		this.isDim = false;
		this.willDim = false;
		this.light.intensity = initialLightIntensity;
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
	
	public void BeginBlowOut(float delay) {
		this.willBlowOut = true;
		this.blowOutStartTime = Time.time + delay;
	}
	
	public void BlowOut() {
		audioSource.PlayOneShot(extinguishSound,0.2f);
		this.state = TorchState.Extinguished;
		if (player != null && this.transform.parent == player.transform) {
			torchPlacer.isHoldingTorch = false;
		}
		Destroy(this.gameObject,1.0f);
	}
	
	public void BeginFlicker(float delay, float duration) {
		if (this.isFlickering == false) {
			this.flickerStartTime = Time.time;
			this.isFlickering = true;
			this.flickerDuration = duration;
			this.flickerDelay = delay;
		}
	}
	
	private void Flicker() {
		float timePassed = Time.time - this.flickerStartTime;
		if (timePassed > flickerDuration) {
			Debug.Log("Ending Flicker");
			EndFlicker();
		}
		else if (timePassed > flickerDelay) {
			if (this.isDim == false) {
				this.BeginDim (0.25f,0.05f,0.0f);
			}
		}
	}
	
	private void EndFlicker() {
		this.isFlickering = false;
	}
	
	
	
	public enum TorchState {
		Unlit,
		Lit,
		Extinguished
	}
}
